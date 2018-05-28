
namespace LazyWelfare.AndroidMobile.Loading
{
    using System;
    using Context = Android.Content.Context;

    public class DensityUtil
    {

        public static float dip2px(Context context, float dpValue)
        {
            float scale = context.Resources.DisplayMetrics.Density;
            return dpValue * scale;
        }


        public static double Radian2Degrees(double radian)
        {
            return radian * 180 / Math.PI;
        }
    }

}