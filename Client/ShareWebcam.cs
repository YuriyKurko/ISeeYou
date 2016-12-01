using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    class ShareWebcam
    {
        public ShareWebcam(NetworkStream networkStream, bool doubleImage)
        {
            this.networkStream = networkStream;
            this.doubleImage = doubleImage;
        }

        private NetworkStream networkStream { get; set; }

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSourse;
        bool doubleImage;

        public void StopCapturing()
        {
            videoSourse.Stop();
            videoDevices = null;
            videoSourse = null;
        }

        public void CapturePhoto()
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                videoSourse = new VideoCaptureDevice();
                videoSourse = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSourse.NewFrame += new NewFrameEventHandler(videoSourse_NewFrame);
                videoSourse.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                StopCapturing();
            }
        }

        public Image ScreenshotWebcabImage(Image screenshotImage, Image webcamImage)
        {
            Image resultImage = new Bitmap(webcamImage.Width + screenshotImage.Width, Math.Max(webcamImage.Height, screenshotImage.Height));

            webcamImage = ResizeImage(webcamImage, new Size((webcamImage.Width * screenshotImage.Height) / webcamImage.Height / 2, screenshotImage.Height / 2));
            Graphics g = Graphics.FromImage(resultImage);
            g.DrawImage(screenshotImage, 0, 0);
            g.DrawImage(webcamImage, screenshotImage.Width, 0);

            return resultImage;
        }

        public static Image ResizeImage(Image imgToResize, Size size)
        {
            return (new Bitmap(imgToResize, size));
        }

        void videoSourse_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                BinaryWriter writer = new BinaryWriter(networkStream);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    Image webcamImage = eventArgs.Frame;

                    if(!doubleImage)
                    {
                        webcamImage.Save(memoryStream, ImageFormat.Jpeg);
                    }
                    else
                    {
                        ScreenshotWebcabImage(ShareScreen.CaptureDesktopWithCursor(), webcamImage).Save(memoryStream, ImageFormat.Jpeg);
                    }

                    byte[] buffer = new byte[memoryStream.Length];

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    memoryStream.Read(buffer, 0, buffer.Length);

                    writer.Write(buffer.Length);

                    writer.Write(buffer);

                    writer = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                StopCapturing();
            }
        }
    }
}

