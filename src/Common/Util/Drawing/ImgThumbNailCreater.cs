using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Dongbo.Common.Util.Drawing
{
   public class ImgThumbNailCreater
    {

       /// <summary>
       /// 图片缩放
       /// </summary>
       /// <param name="fromFile"></param>
       /// <param name="savePath"></param>
       /// <param name="targetWidth"></param>
       /// <remarks>玄魂@2013-07-23</remarks>
       public static void Zoom(Stream sourceFrom, double mostWidth, ref Stream outPut)
       {
           //if (!File.Exists(saveTo))
           //    File.Create(saveTo);
           Graphics graphics = null;
           sourceFrom.Seek(0, SeekOrigin.Begin);
           Bitmap original = new Bitmap(sourceFrom);
           bool isPng = original.RawFormat.Equals(ImageFormat.Png);

           int newHeight = (int)(original.Height * (mostWidth / original.Width));
           int newWidth = (int)mostWidth;
           //if (original.Width > mostWidth)
           //{
           //    //缩略图宽、高计算

           //    //宽按模版，高按比例缩放
           //    newHeight = 
           //    newWidth = (int)mostWidth;

           //}
           Bitmap bitmap = new Bitmap(newWidth, newHeight);
           try
           {
               graphics = Graphics.FromImage(bitmap);

               // 插值算法的质量
               graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
               graphics.SmoothingMode = SmoothingMode.HighQuality;
               graphics.DrawImage(original, new Rectangle(0, 0, newWidth, newHeight),
                   new Rectangle(0, 0, original.Width, original.Height), GraphicsUnit.Pixel);
               var format = isPng ? ImageFormat.Png : ImageFormat.Jpeg;

               bitmap.Save(outPut, format);
           }
           finally
           {
               if (graphics != null)
               {
                   graphics.Dispose();
                   original.Dispose();
                   bitmap.Dispose();
               }
           }

       }
    }
}
