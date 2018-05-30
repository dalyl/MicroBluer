namespace LazyWelfare.AndroidUtils.Common
{
    using System;
    using Context = Android.Content.Context;

    public static class DensityUtil
    {

        public static float Dip2Px(Context context, float dpValue)
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