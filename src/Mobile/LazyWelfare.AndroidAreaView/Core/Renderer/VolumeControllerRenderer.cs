namespace LazyWelfare.AndroidAreaView.Core.Renderer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Android.App;
    using Android.Content;
    using Android.Graphics;
    using Android.Graphics.Drawables;
    using Android.OS;
    using Android.Runtime;
    using Android.Views;
    using Android.Widget;
    using LazyWelfare.AndroidUtils.Common;

    public class VolumeControllerRenderer : AreaRenderer
    {

        private const float DEFAULT_Width = 200.0f;
        private const float DEFAULT_Height = 200.0f;
        private const float DEFAULT_RADIUS = 100.0f;

        private const float ImageVolumeWidth = 132.0f;
        private const float ImageVolumeHeight = 316.0f;

        private int VolumeWidth { get; }
        private int VolumeHeight { get; }

        private float Center { get; }

        private  Drawable VolumeDrawable { get; }
        private  Paint mPaint { get; } =new Paint();

        public VolumeControllerRenderer(Context context) : base(context)
        {
            VolumeDrawable = context.GetDrawable(Resource.Drawable.offered_vol);
            Width = DensityUtil.Dip2Px(context, DEFAULT_Width);
            Height = DensityUtil.Dip2Px(context, DEFAULT_Height);
            Center = DensityUtil.Dip2Px(context, DEFAULT_RADIUS);
            VolumeHeight = (int) Height - 10;
            VolumeWidth = (int)((ImageVolumeWidth / ImageVolumeHeight) * VolumeHeight);
        }

        public bool IsPressTop { get; set; } = false;

        public bool IsPressBottom { get; set; } = false;

        protected override void Draw(Canvas canvas, Rect bounds)
        {
            var begin = canvas.Save();

            var leftX = (int)((Width / 2) - (VolumeWidth / 2));
            var topY = (int)((Height / 2) - (VolumeHeight / 2));
            var widthX = leftX + VolumeWidth;
            var heightY = topY + VolumeHeight;

            if (IsPressTop)
            {
                var topCircleRadius = VolumeWidth / 2;
                var topCircleX = Center;
                var topCircleY = topY + topCircleRadius;
                mPaint.Color = Color.Green;
                canvas.DrawCircle(topCircleX, topCircleY, topCircleRadius, mPaint);

                mPaint.Color = Color.Green;
                canvas.DrawRect(leftX, topCircleY, widthX, heightY / 2, mPaint);
            }

            if (IsPressBottom)
            {
                var bottomCircleRadius = VolumeWidth / 2;
                var bottomCircleX = Center;
                var bottomCircleY = heightY - bottomCircleRadius;
                mPaint.Color = Color.Green;
                canvas.DrawCircle(bottomCircleX, bottomCircleY, bottomCircleRadius, mPaint);

                mPaint.Color = Color.Green;
                canvas.DrawRect(leftX, Center, widthX, bottomCircleY, mPaint);
            }

            VolumeDrawable.SetBounds(leftX, topY, widthX, heightY);
            VolumeDrawable.Draw(canvas);
            canvas.RestoreToCount(begin);
        }

        public override void TouchArea(MotionEvent act)
        {
            if (act.Action == MotionEventActions.Down)
            {
               
                float x = act.XPrecision;
                float y = act.YPrecision;
                Toast.MakeText(Context, $"x:{x},y{y}", ToastLength.Short).Show();
            }
            else if (act.Action == MotionEventActions.Up)
            {
                float x = act.RawX;
                float y = act.RawY;

                Toast.MakeText(Context, $"x:{x},y{y}", ToastLength.Short).Show();
            }
        }


    }

}