using ZXing.Common;
using ZXing;
using ZXing.QrCode;
using System.DrawingCore;
using System.IO;
using System.DrawingCore.Imaging;
using System;
namespace LazyWelfare.ServerCore
{

    public class ImageCreate
    {

        public static Bitmap QRCode(string content, int width, int height)
        {
            EncodingOptions options;
            //包含一些编码、大小等的设置
            //BarcodeWriter :一个智能类来编码一些内容的条形码图像
            options = new QrCodeEncodingOptions
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = width,
                Height = height,
                Margin = 0
            };
            var write = new BarcodeWriter<Bitmap>
            {
                //设置条形码格式
                Format = BarcodeFormat.QR_CODE,
                //获取或设置选项容器的编码和渲染过程。
                Options = options,
                Renderer = new BitmapRenderer()
            };
            //对指定的内容进行编码，并返回该条码的呈现实例。渲染属性渲染实例使用，必须设置方法调用之前。
            return write.Write(content);
        }

        public static Bitmap TextBitmap(string content, int width, int height)
        {
            var font = new Font(FontFamily.GenericSansSerif, 12.0F, FontStyle.Regular);
            var rect = new Rectangle(0, 0, width, height);
            return TextBitmap(content, font, rect, Color.DarkOrange,Color.DarkCyan);
        }

        /// <summary>
        /// 把文字转换才Bitmap
        /// </summary>
        public static Bitmap TextBitmap(string text, Font font, Rectangle rect,Color fontcolor,Color backColor)
        {
            Graphics g;
            Bitmap bmp;
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            if (rect == Rectangle.Empty)
            {
                bmp = new Bitmap(1, 1);
                g = Graphics.FromImage(bmp);
                //计算绘制文字所需的区域大小（根据宽度计算长度），重新创建矩形区域绘图
                SizeF sizef = g.MeasureString(text, font, PointF.Empty, format);

                int width = (int)(sizef.Width + 1);
                int height = (int)(sizef.Height + 1);
                rect = new Rectangle(0, 0, width, height);
                bmp.Dispose();

                bmp = new Bitmap(width, height);
            }
            else
            {
                bmp = new Bitmap(rect.Width, rect.Height);
            }

            g = Graphics.FromImage(bmp);

            //使用ClearType字体功能
            g.TextRenderingHint = System.DrawingCore.Text.TextRenderingHint.ClearTypeGridFit;
            g.FillRectangle(new SolidBrush(backColor), rect);
            g.DrawString(text, font,Brushes.Black, rect, format);
            return bmp;
        }

        public static byte[] Convert(Bitmap image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Bmp);
                byte[] bytes = ms.GetBuffer();  //byte[]   bytes=   ms.ToArray(); 这两句都可以，至于区别么，下面有解释
                return bytes;
            }
        }

      
    }
}
