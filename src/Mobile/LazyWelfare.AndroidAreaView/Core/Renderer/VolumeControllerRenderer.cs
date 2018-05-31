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

        private  Drawable VolumeDrawable { get; }
        private  Paint mPaint { get; } =new Paint();

        private float Center { get; }
        private int VolumeWidth { get; set; }
        private int VolumeHeight { get; set; }
        private int VolumeX { get; set; }
        private int VolumeY { get; set; }
        private int VolumeWidthX { get; set; }
        private int VolumeHeightY { get; set; }

        public Action OnClickTop { get; set; }
        public Action OnClickBottom { get; set; }

        public VolumeControllerRenderer(Context context) : base(context)
        {
            VolumeDrawable = context.GetDrawable(Resource.Drawable.offered_vol);
            Width = DensityUtil.Dip2Px(context, DEFAULT_Width);
            Height = DensityUtil.Dip2Px(context, DEFAULT_Height);
            Center = DensityUtil.Dip2Px(context, DEFAULT_RADIUS);
            InitSize();
        }

        private void InitSize()
        {
            VolumeHeight = (int)Height - 10;
            VolumeWidth = (int)((ImageVolumeWidth / ImageVolumeHeight) * VolumeHeight);
            VolumeX = (int)((Width / 2) - (VolumeWidth / 2));
            VolumeY = (int)((Height / 2) - (VolumeHeight / 2));
            VolumeWidthX = VolumeX + VolumeWidth;
            VolumeHeightY = VolumeY + VolumeHeight;
        }

        bool PressTop(float x, float y)
        {
            return x > (VolumeX)
              && x < (VolumeWidthX)
              && y > (VolumeY)
              && y < (VolumeHeightY / 2);
        }

        bool PressBottom(float x, float y)
        {
            return x > (VolumeX)
                && x < (VolumeWidthX)
                && y > (VolumeHeightY / 2)
                && y < (VolumeHeightY);
        }

        public bool IsPressTop { get; set; } = false;

        public bool IsPressBottom { get; set; } = false;

        protected override void Draw(Canvas canvas, Rect bounds)
        {
            var begin = canvas.Save();
            if (IsPressTop)
            {
                var topCircleRadius = VolumeWidth / 2;
                var topCircleX = Center;
                var topCircleY = VolumeY + topCircleRadius;
                mPaint.Color = Color.Green;
                canvas.DrawCircle(topCircleX, topCircleY, topCircleRadius, mPaint);

                mPaint.Color = Color.Green;
                canvas.DrawRect(VolumeX, topCircleY, VolumeWidthX, VolumeHeightY / 2, mPaint);
            }

            if (IsPressBottom)
            {
                var bottomCircleRadius = VolumeWidth / 2;
                var bottomCircleX = Center;
                var bottomCircleY = VolumeHeightY - bottomCircleRadius;
                mPaint.Color = Color.Green;
                canvas.DrawCircle(bottomCircleX, bottomCircleY, bottomCircleRadius, mPaint);

                mPaint.Color = Color.Green;
                canvas.DrawRect(VolumeX, Center, VolumeWidthX, bottomCircleY, mPaint);
            }

            VolumeDrawable.SetBounds(VolumeX, VolumeY, VolumeWidthX, VolumeHeightY);
            VolumeDrawable.Draw(canvas);
            canvas.RestoreToCount(begin);
        }

        public override bool TouchArea(MotionEvent act)
        {
            if (act.Action == MotionEventActions.Down)
            {
                float x = act.GetX();
                float y = act.GetY();
                IsPressTop = PressTop(x, y);
                IsPressBottom = PressBottom(x, y);
                if (IsPressTop) OnClickTop?.Invoke();
                if (IsPressBottom) OnClickBottom?.Invoke();
                return true;
            }
            else if (act.Action == MotionEventActions.Up)
            {
                float x = act.GetX();
                float y = act.GetY();

                IsPressTop =false;
                IsPressBottom = false;
            }
            return false;
        }


    }

}