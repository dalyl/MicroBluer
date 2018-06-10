namespace LazyWelfare.AndroidCtrls.ImageAction.Renderer
{
    using System;
    using Android.Content;
    using Android.Graphics;
    using Android.Graphics.Drawables;
    using Android.Views;

    public class VolumeControllerRenderer : ActionImageRenderer
    {
        private const int ImageVolumeWidth = 132;
        private const int ImageVolumeHeight = 316;
        private const int TextWidth = 330;
        private const int TextHeight = 120;

        private  Drawable VolumeDrawable { get; }
        private  Paint mPaint { get; } =new Paint();

        private int VolumeWidth { get; set; }
        private int VolumeHeight { get; set; }
        private int VolumeX { get; set; }
        private int VolumeY { get; set; }
        private int VolumeCenterX { get; set; }
        private int VolumeCenterY { get; set; }
        private int VolumeWidthX { get; set; }
        private int VolumeHeightY { get; set; }


        private int TextX { get; set; }
        private int TextY { get; set; }
        private int TextWidthX { get; set; }
        private int TextHeightY { get; set; }

        public Action OnClickTop { get; set; }
        public Action OnClickBottom { get; set; }

        public Func<string> FetchText { get; set; }

        public VolumeControllerRenderer(Context context) : base(context)
        {
            VolumeDrawable = context.GetDrawable(Resource.Drawable.offered_vol);
            mPaint.TextSize = 60;
            mPaint.TextAlign = Paint.Align.Center;
            InitSize();
        }

        private void InitSize()
        {
            VolumeCenterX = (int)(Width / 2);
            VolumeCenterY = (int)(Height / 2);

            VolumeHeight = (int)(1.5 * ImageVolumeHeight);
            VolumeWidth = (int)(1.5 * ImageVolumeWidth);

            VolumeX = VolumeCenterX - (VolumeWidth / 2);
            VolumeY = VolumeCenterY - (VolumeHeight / 2);
            VolumeWidthX = VolumeX + VolumeWidth;
            VolumeHeightY = VolumeY + VolumeHeight;

            TextY = (VolumeY / 2) - (TextHeight / 2);
            TextX = VolumeCenterX - (TextWidth / 2);
            TextWidthX = TextX + TextWidth;
            TextHeightY = TextY + TextHeight;
        }

        bool PressTop(float x, float y)
        {
            return x > (VolumeX)
              && x < (VolumeWidthX)
              && y > (VolumeY)
              && y < (VolumeCenterY);
        }

        bool PressBottom(float x, float y)
        {
            return x > (VolumeX)
                && x < (VolumeWidthX)
                && y > (VolumeCenterY)
                && y < (VolumeHeightY);
        }

        public bool IsPressTop { get; set; } = false;

        public bool IsPressBottom { get; set; } = false;

        protected override void Draw(Canvas canvas, Rect bounds)
        {
            var begin = canvas.Save();

            mPaint.Color = Color.White;
            var text = FetchText?.Invoke();
            if (text != null) canvas.DrawText(text, VolumeCenterX, TextHeightY, mPaint);
            mPaint.SetStyle(Paint.Style.Fill);

            if (IsPressTop)
            {
                var topCircleRadius = VolumeWidth / 2;
                var topCircleX = VolumeCenterX;
                var topCircleY = VolumeY + topCircleRadius;
                mPaint.Color = Color.Green;
                canvas.DrawCircle(topCircleX, topCircleY, topCircleRadius, mPaint);

                mPaint.Color = Color.Green;
                canvas.DrawRect(VolumeX, topCircleY, VolumeWidthX, VolumeCenterY, mPaint);
            }

            if (IsPressBottom)
            {
                var bottomCircleRadius = VolumeWidth / 2;
                var bottomCircleX = VolumeCenterX;
                var bottomCircleY = VolumeHeightY - bottomCircleRadius;
                mPaint.Color = Color.Green;
                canvas.DrawCircle(bottomCircleX, bottomCircleY, bottomCircleRadius, mPaint);

                mPaint.Color = Color.Green;
                canvas.DrawRect(VolumeX, VolumeCenterY, VolumeWidthX, bottomCircleY, mPaint);
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