using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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
        public static Image GrabDesktop()
        {
            System.Drawing.Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap screeshot = new Bitmap(bounds.Width, bounds.Height);
            Graphics graphic = Graphics.FromImage(screeshot);
            graphic.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
            return screeshot;
        }

        public void SendScreenshot()
        {
            try
            {
                BinaryWriter writer = new BinaryWriter(networkStream);

                while (true)
                {

                    Image image = GrabDesktop();

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
