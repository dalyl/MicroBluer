namespace LazyWelfare.AndroidUtils.Common
{
    using System;
    using Android.Content;
    using Android.Graphics;
    using Android.Graphics.Drawables;

    public static class ImageUtils
    {
        public static Bitmap Drawable2Bitmap(this Context context, int drawable)
        {
            var image = context.GetDrawable(drawable);
            return GetBitmapFromDrawable(image);
        }

        private static readonly Bitmap.Config BITMAP_CONFIG = Bitmap.Config.Argb8888;
        private static readonly int COLORDRAWABLE_DIMENSION = 2;
      //  private static int DEFAULT_BORDER_COLOR = Color.Black;
        private static readonly int DEFAULT_CIRCLE_BACKGROUND_COLOR = Color.Transparent;

        static Bitmap GetBitmapFromDrawable(Drawable drawable)
        {
            if (drawable == null)
            {
                return null;
            }

            if (drawable is BitmapDrawable)
            {
                return ((BitmapDrawable)drawable).Bitmap;
            }

            try
            {
                Bitmap bitmap;

                //颜色Drawable
                if (drawable is ColorDrawable)
                {
                    //宽为2, 高为2 ??
                    bitmap = Bitmap.CreateBitmap(COLORDRAWABLE_DIMENSION, COLORDRAWABLE_DIMENSION, BITMAP_CONFIG);
                }
                else
                {
                    bitmap = Bitmap.CreateBitmap(drawable.IntrinsicWidth, drawable.IntrinsicHeight, BITMAP_CONFIG);
                }

                Canvas canvas = new Canvas(bitmap);
                drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
                //将 drawable 的内容绘制到 bitmap的canvas 上面去.
                drawable.Draw(canvas);
                return bitmap;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
                return null;
            }
        }
    }

}