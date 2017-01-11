using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dongbo.Common.Util.Drawing
{
    /// <summary>
    /// ImgClipCtrol.xaml 的交互逻辑
    /// </summary>
    public partial class ImgClipCtrol : UserControl
    {

        public ImgClipCtrol(ImgCliperContext context, byte[] data, BitmapImage imgSource)
        {
                InitializeComponent();

                InitImg(context.SourceImgUri, context.ImgLeft, context.ImgTop, context.MarginLeft, context.MarginTop, data, imgSource,context.FrameWidth,context.FrameHeight);      
        }

        void InitImg(Uri imgUri, double paddingLeft, double paddingTop, double marginLeft, double marginTop, byte[] data, BitmapImage imgSource,double imgWidth,double imgHeight)
        {
          
                    img.BeginInit();
                    img.Source = imgSource;
                    img.Margin = new Thickness(paddingLeft, paddingTop, 0, 0);
                    ImgContainer.Margin = new Thickness(marginLeft, marginTop, marginLeft, marginTop);
                  //  img.Clip = new RectangleGeometry(new Rect(- paddingLeft, -paddingTop, imgSource.Width + paddingLeft, imgSource.Height + paddingTop));
                    img.Width = imgSource.PixelWidth;
                    img.Height = imgSource.PixelHeight;
                    ImgContainer.Width = imgWidth;
                    ImgContainer.Height =imgHeight;
                    img.EndInit();
                    this.Width = imgWidth + marginLeft * 2;
                    this.Height = imgHeight + marginTop * 2;
           
        }


    }
}
