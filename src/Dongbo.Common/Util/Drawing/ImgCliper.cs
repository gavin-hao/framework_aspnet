using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Dongbo.Common.Util.Drawing;

namespace Dongbo.Common
{
    public class ImgCliper
    {
        private volatile ImgCliperContext _context;
        Stream streamResult;
        private int count = 0;
        UpdateStreamResultDel usrd;
        UpdateCountDel ucd;
        public ImgCliper(ImgCliperContext context)
        {
            _context = context;
            ucd = UpdateCount;
            usrd = UpdateStreamResult;         

                Thread th = new Thread(InitStream);
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
                while (count == 0)
                {
                    Thread.Sleep(10);
                    continue;
                   
                }
                th.Abort();
        }

        delegate void UpdateStreamResultDel(Stream stream);

        void UpdateStreamResult(Stream stream)
        {
            this.streamResult = stream;
        }
        delegate void UpdateCountDel();

        void UpdateCount()
        {
            this.count++;
        }
        private void InitStream()
        {
            byte[] data;
          
            var webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.Referer] = "http://www.mbdongbo.com";

            data = webClient.DownloadData(_context.SourceImgUri);

            BitmapImage imgSource=SetImgSource(data);
            double _WHRate = SetWHRate(imgSource);
            ModifyContext(_WHRate);

            UserControl imgClipCtrol;  imgClipCtrol = new ImgClipCtrol(_context, data, imgSource);
            imgClipCtrol.Measure(new Size(imgClipCtrol.Width, imgClipCtrol.Height));
            imgClipCtrol.Arrange(new Rect(new Size(imgClipCtrol.Width, imgClipCtrol.Height)));


            BitmapEncoder imgEncoder = null;
            imgEncoder = GetEncoder(_context.Ext, imgEncoder);
            var stream = new MemoryStream();
            if (imgClipCtrol != null)
            {
                RenderTargetBitmap bmpSource = new RenderTargetBitmap((int)imgClipCtrol.ActualWidth, (int)imgClipCtrol.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                bmpSource.Render(imgClipCtrol);

                imgEncoder.Frames.Add(BitmapFrame.Create(bmpSource));

                imgEncoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
            }

            if (imgSource != null)
            {
                if (imgSource.StreamSource != null)
                {
                    imgSource.StreamSource.Dispose();
                }
            }
            usrd.Invoke(stream);
            ucd.Invoke();
        }
        private BitmapImage SetImgSource(byte[] data)
        {
            BitmapImage imgSource = new BitmapImage();
            imgSource.BeginInit();
            imgSource.StreamSource = new MemoryStream(data);
            imgSource.EndInit();
            return imgSource;
        }
        private double SetWHRate(BitmapImage imgSource)
        {
            double imgWHRate = (double)imgSource.PixelWidth / imgSource.PixelHeight;
            double frameWHRate = _context.FrameWidth / _context.FrameHeight;
            double _WHRate = imgWHRate > frameWHRate ? (imgSource.PixelHeight / _context.FrameHeight) : (imgSource.PixelWidth / _context.FrameWidth);
            return _WHRate;
        }
        private void ModifyContext(double rate)
        {
            _context.ImgLeft *= rate;
            _context.ImgTop *= rate;
            _context.FrameHeight *= rate;
            _context.FrameWidth *= rate;
            _context.MarginTop *= rate;
            _context.MarginLeft *= rate;
        }


        public void SaveImageToLocal(string fileName)
        {
            string imageExtension = new FileInfo(fileName).Extension.ToLower(CultureInfo.InvariantCulture);
            if (streamResult != null)
            {
                var data = SaveImageToBytes(imageExtension);
                using (Stream stream = File.Create(fileName))
                {
                    stream.Write(data, 0, data.Length); 
                  
                }
            }
        }

        public byte[] SaveImageToBytes(string imageExtension)
        {

            if (streamResult != null)
            {
              
            
                byte[] data = new byte[ streamResult .Length];
                streamResult.Read(data, 0, data.Length);
                streamResult.Dispose();
                return data;
            }
            return null;
        }

        public Stream SaveImageToStream()
        {
            return streamResult;
        }

        private  BitmapEncoder GetEncoder(string imageExtension, BitmapEncoder imgEncoder)
        {
            switch (imageExtension)
            {
                case ".bmp":
                    imgEncoder = new BmpBitmapEncoder();
                    break;

                case ".jpg":
                case ".jpeg":
                    imgEncoder = new JpegBitmapEncoder();
                    break;

                case ".png":
                    imgEncoder = new PngBitmapEncoder();
                    break;

                case ".gif":
                    imgEncoder = new GifBitmapEncoder();
                    break;

                case ".tif":
                case ".tiff":
                    imgEncoder = new TiffBitmapEncoder();
                    break;

                case ".wdp":
                    imgEncoder = new WmpBitmapEncoder();
                    break;

                default:
                    imgEncoder = new BmpBitmapEncoder();
                    break;
            }
            return imgEncoder;
        }



    }




    public class ImgCliperContext
    {

        public double FrameHeight { get; set; }
        public double FrameWidth { get; set; }
        public double ImgLeft { get; set; }
        public double ImgTop { get; set; }
        public double MarginLeft { get; set; }
        public double MarginTop { get; set; }
        public long SourceImgID { get; set; }
        public Uri SourceImgUri { get; set; }
        public string Ext { get; set; }
    }
}