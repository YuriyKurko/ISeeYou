﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

namespace Server
{
    class ReceiveScreenImage
    {
        public ReceiveScreenImage(ClientObject client, System.Windows.Controls.Image screenImage)
        {
            this.client = client;
            image = screenImage;
        }

        private System.Windows.Controls.Image image { get; set; }
        private ClientObject client { get; set; }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public void ReceiveImage()
        {
            while (client.tcpClient.Connected)
            {
                try
                {

                    BinaryReader reader = new BinaryReader(client.Stream);

                    int ctBytes = reader.ReadInt32();

                    MemoryStream ms = new MemoryStream(reader.ReadBytes(ctBytes));

                    Bitmap bitmap = new Bitmap(Image.FromStream(ms));

                    Action action = delegate { image.Source = BitmapToImageSource(bitmap); };

                    image.Dispatcher.Invoke(action);

                    reader = null;

                    reader.Dispose();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }        
        }
    }
}


