using ScreenshotCaptureWithMouse.ScreenCapture;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    class ShareScreen
    {
        public ShareScreen(NetworkStream networkStream)
        {
            this.networkStream = networkStream;
        }

        private NetworkStream networkStream { get; set; }
        TcpClient tcpClient { get; set; }

        static Bitmap CaptureCursor(ref int x, ref int y)
        {
            Bitmap bmp;
            IntPtr hicon;
            Win32Stuff.CURSORINFO ci = new Win32Stuff.CURSORINFO();
            Win32Stuff.ICONINFO icInfo;
            ci.cbSize = Marshal.SizeOf(ci);
            if (Win32Stuff.GetCursorInfo(out ci))
            {
                if (ci.flags == Win32Stuff.CURSOR_SHOWING)
                {
                    hicon = Win32Stuff.CopyIcon(ci.hCursor);
                    if (Win32Stuff.GetIconInfo(hicon, out icInfo))
                    {
                        x = ci.ptScreenPos.x - ((int)icInfo.xHotspot);
                        y = ci.ptScreenPos.y - ((int)icInfo.yHotspot);
                        Icon ic = Icon.FromHandle(hicon);
                        bmp = ic.ToBitmap();

                        return bmp;
                    }
                }
            }
            return null;
        }

        public static Bitmap GrabDesktop()
        {
            System.Drawing.Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap screeshot = new Bitmap(bounds.Width, bounds.Height);
            Graphics graphic = Graphics.FromImage(screeshot);
            graphic.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
            return screeshot;
        }

        public static Image CaptureDesktopWithCursor()
        {
            int cursorX = 0;
            int cursorY = 0;
            Bitmap desktopBMP;
            Bitmap cursorBMP;
            Graphics g;
            Rectangle r;
            desktopBMP = GrabDesktop();
            cursorBMP = CaptureCursor(ref cursorX, ref cursorY);
            if (desktopBMP != null)
            {
                if (cursorBMP != null)
                {
                    r = new Rectangle(cursorX, cursorY,
                            cursorBMP.Width, cursorBMP.Height);
                    g = Graphics.FromImage(desktopBMP);
                    g.DrawImage(cursorBMP, r);
                    g.Flush();
                    return desktopBMP;
                }
                else
                    return desktopBMP;
            }
            return null;
        }

        public void SendScreenshot()
        {
            try
            {
                BinaryWriter writer = new BinaryWriter(networkStream);

                while (true)
                {

                    Image image = CaptureDesktopWithCursor();

                    MemoryStream ms = new MemoryStream();
                    image.Save(ms, ImageFormat.Jpeg);

                    byte[] buffer = new byte[ms.Length];

                    ms.Seek(0, SeekOrigin.Begin);

                    ms.Read(buffer, 0, buffer.Length);

                    writer.Write(buffer.Length);

                    writer.Write(buffer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
