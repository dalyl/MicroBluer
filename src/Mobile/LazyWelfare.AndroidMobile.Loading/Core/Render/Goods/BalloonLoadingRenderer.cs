namespace LazyWelfare.AndroidMobile.Loading.Render.Goods
{

    using Context = Android.Content.Context;
    using Canvas = Android.Graphics.Canvas;
    using Color = Android.Graphics.Color;
    using ColorFilter = Android.Graphics.ColorFilter;
    using Paint = Android.Graphics.Paint;
    using Path = Android.Graphics.Path;
    using RectF = Android.Graphics.RectF;
    using Interpolator = Android.Views.Animations.IInterpolator;
    using AccelerateInterpolator = Android.Views.Animations.AccelerateInterpolator;
    using LazyWelfare.AndroidUtils.Common;
    using Android.Graphics;

    public class BalloonLoadingRenderer : LoadingRenderer
	{
		private const string PERCENT_SIGN = "%";

		private static readonly Interpolator ACCELERATE_INTERPOLATOR = new AccelerateInterpolator();

		private const float START_INHALE_DURATION_OFFSET = 0.4f;

		private const float DEFAULT_WIDTH = 200.0f;
		private const float DEFAULT_HEIGHT = 150.0f;
		private const float DEFAULT_STROKE_WIDTH = 2.0f;
		private const float DEFAULT_GAS_TUBE_WIDTH = 48;
		private const float DEFAULT_GAS_TUBE_HEIGHT = 20;
		private const float DEFAULT_CANNULA_WIDTH = 13;
		private const float DEFAULT_CANNULA_HEIGHT = 37;
		private const float DEFAULT_CANNULA_OFFSET_Y = 3;
		private const float DEFAULT_CANNULA_MAX_OFFSET_Y = 15;
		private const float DEFAULT_PIPE_BODY_WIDTH = 16;
		private const float DEFAULT_PIPE_BODY_HEIGHT = 36;
		private const float DEFAULT_BALLOON_WIDTH = 38;
		private const float DEFAULT_BALLOON_HEIGHT = 48;
		private const float DEFAULT_RECT_CORNER_RADIUS = 2;

		private static readonly int DEFAULT_BALLOON_COLOR = Color.ParseColor("#ffF3C211");
		private static readonly int DEFAULT_GAS_TUBE_COLOR = Color.ParseColor("#ff174469");
		private static readonly int DEFAULT_PIPE_BODY_COLOR = Color.ParseColor("#aa2369B1");
		private static readonly int DEFAULT_CANNULA_COLOR = Color.ParseColor("#ff174469");

		private const float DEFAULT_TEXT_SIZE = 7.0f;

		private const long ANIMATION_DURATION = 3333;

		private readonly Paint mPaint = new Paint();
		private readonly RectF mCurrentBounds = new RectF();
		private readonly RectF mGasTubeBounds = new RectF();
		private readonly RectF mPipeBodyBounds = new RectF();
		private readonly RectF mCannulaBounds = new RectF();
		private readonly RectF mBalloonBounds = new RectF();

		private readonly Rect mProgressBounds = new Rect();

		private float mTextSize;
		private float mProgress;

		private string mProgressText;

		private float mGasTubeWidth;
		private float mGasTubeHeight;
		private float mCannulaWidth;
		private float mCannulaHeight;
		private float mCannulaMaxOffsetY;
		private float mCannulaOffsetY;
		private float mPipeBodyWidth;
		private float mPipeBodyHeight;
		private float mBalloonWidth;
		private float mBalloonHeight;
		private float mRectCornerRadius;
		private float mStrokeWidth;

		private int mBalloonColor;
		private int mGasTubeColor;
		private int mCannulaColor;
		private int mPipeBodyColor;

        internal BalloonLoadingRenderer(Context context) : base(context)
		{
			Init(context);
			SetupPaint();
		}

		private void Init(Context context)
		{
			mTextSize = DensityUtil.Dip2Px(context, DEFAULT_TEXT_SIZE);

			mWidth = DensityUtil.Dip2Px(context, DEFAULT_WIDTH);
			mHeight = DensityUtil.Dip2Px(context, DEFAULT_HEIGHT);
			mStrokeWidth = DensityUtil.Dip2Px(context, DEFAULT_STROKE_WIDTH);

			mGasTubeWidth = DensityUtil.Dip2Px(context, DEFAULT_GAS_TUBE_WIDTH);
			mGasTubeHeight = DensityUtil.Dip2Px(context, DEFAULT_GAS_TUBE_HEIGHT);
			mCannulaWidth = DensityUtil.Dip2Px(context, DEFAULT_CANNULA_WIDTH);
			mCannulaHeight = DensityUtil.Dip2Px(context, DEFAULT_CANNULA_HEIGHT);
			mCannulaOffsetY = DensityUtil.Dip2Px(context, DEFAULT_CANNULA_OFFSET_Y);
			mCannulaMaxOffsetY = DensityUtil.Dip2Px(context, DEFAULT_CANNULA_MAX_OFFSET_Y);
			mPipeBodyWidth = DensityUtil.Dip2Px(context, DEFAULT_PIPE_BODY_WIDTH);
			mPipeBodyHeight = DensityUtil.Dip2Px(context, DEFAULT_PIPE_BODY_HEIGHT);
			mBalloonWidth = DensityUtil.Dip2Px(context, DEFAULT_BALLOON_WIDTH);
			mBalloonHeight = DensityUtil.Dip2Px(context, DEFAULT_BALLOON_HEIGHT);
			mRectCornerRadius = DensityUtil.Dip2Px(context, DEFAULT_RECT_CORNER_RADIUS);

			mBalloonColor = DEFAULT_BALLOON_COLOR;
			mGasTubeColor = DEFAULT_GAS_TUBE_COLOR;
			mCannulaColor = DEFAULT_CANNULA_COLOR;
			mPipeBodyColor = DEFAULT_PIPE_BODY_COLOR;

			mProgressText = 10 + PERCENT_SIGN;

			mDuration = ANIMATION_DURATION;
		}

		private void SetupPaint()
		{
			mPaint.AntiAlias = true;
			mPaint.StrokeWidth = mStrokeWidth;
		}

		protected internal override void Draw(Canvas canvas, Rect bounds)
		{
			int saveCount = canvas.Save();

			RectF arcBounds = mCurrentBounds;
			arcBounds.Set(bounds);

			//draw draw gas tube
			mPaint.Color = new Color (mGasTubeColor);
			mPaint.SetStyle(Paint.Style.Stroke);
			mPaint.StrokeWidth = mStrokeWidth;
			canvas.DrawPath(CreateGasTubePath(mGasTubeBounds), mPaint);

			//draw balloon
			mPaint.Color = new Color (mBalloonColor);
			mPaint.SetStyle(Paint.Style.FillAndStroke);
			canvas.DrawPath(CreateBalloonPath(mBalloonBounds, mProgress), mPaint);

			//draw progress
			mPaint.Color = new Color (mGasTubeColor);
			mPaint.TextSize = mTextSize;
			mPaint.StrokeWidth = mStrokeWidth / 5.0f;
			canvas.DrawText(mProgressText, arcBounds.CenterX() - mProgressBounds.Width() / 2.0f, mGasTubeBounds.CenterY() + mProgressBounds.Height() / 2.0f, mPaint);

			//draw cannula
			mPaint.Color = new Color (mCannulaColor);
			mPaint.SetStyle(Paint.Style.Stroke);
			mPaint.StrokeWidth = mStrokeWidth;
			canvas.DrawPath(CreateCannulaHeadPath(mCannulaBounds), mPaint);
			mPaint.SetStyle (Paint.Style.Fill);
			canvas.DrawPath(CreateCannulaBottomPath(mCannulaBounds), mPaint);

			//draw pipe body
			mPaint.Color = new Color (mPipeBodyColor);
			mPaint.SetStyle (Paint.Style.Fill);
            canvas.DrawRoundRect(mPipeBodyBounds, mRectCornerRadius, mRectCornerRadius, mPaint);

			canvas.RestoreToCount(saveCount);
		}

		protected internal override void ComputeRender(float renderProgress)
		{
			RectF arcBounds = mCurrentBounds;
			//compute gas tube bounds
			mGasTubeBounds.Set(arcBounds.CenterX() - mGasTubeWidth / 2.0f, arcBounds.CenterY(), arcBounds.CenterX() + mGasTubeWidth / 2.0f, arcBounds.CenterY() + mGasTubeHeight);
			//compute pipe body bounds
			mPipeBodyBounds.Set(arcBounds.CenterX() + mGasTubeWidth / 2.0f - mPipeBodyWidth / 2.0f, arcBounds.CenterY() - mPipeBodyHeight, arcBounds.CenterX() + mGasTubeWidth / 2.0f + mPipeBodyWidth / 2.0f, arcBounds.CenterY());
			//compute cannula bounds
			mCannulaBounds.Set(arcBounds.CenterX() + mGasTubeWidth / 2.0f - mCannulaWidth / 2.0f, arcBounds.CenterY() - mCannulaHeight - mCannulaOffsetY, arcBounds.CenterX() + mGasTubeWidth / 2.0f + mCannulaWidth / 2.0f, arcBounds.CenterY() - mCannulaOffsetY);
			//compute balloon bounds
			float insetX = mBalloonWidth * 0.333f * (1 - mProgress);
			float insetY = mBalloonHeight * 0.667f * (1 - mProgress);
			mBalloonBounds.Set(arcBounds.CenterX() - mGasTubeWidth / 2.0f - mBalloonWidth / 2.0f + insetX, arcBounds.CenterY() - mBalloonHeight + insetY, arcBounds.CenterX() - mGasTubeWidth / 2.0f + mBalloonWidth / 2.0f - insetX, arcBounds.CenterY());

			if (renderProgress <= START_INHALE_DURATION_OFFSET)
			{
				mCannulaBounds.Offset(0, -mCannulaMaxOffsetY * renderProgress / START_INHALE_DURATION_OFFSET);

				mProgress = 0.0f;
				mProgressText = 10 + PERCENT_SIGN;

				mPaint.TextSize = mTextSize;
				mPaint.GetTextBounds(mProgressText, 0, mProgressText.Length, mProgressBounds);
			}
			else
			{
				float exhaleProgress = ACCELERATE_INTERPOLATOR.GetInterpolation(1.0f - (renderProgress - START_INHALE_DURATION_OFFSET) / (1.0f - START_INHALE_DURATION_OFFSET));
				mCannulaBounds.Offset(0, -mCannulaMaxOffsetY * exhaleProgress);

				mProgress = 1.0f - exhaleProgress;
				mProgressText = AdjustProgress((int)(exhaleProgress * 100.0f)) + PERCENT_SIGN;

				mPaint.TextSize = mTextSize;
				mPaint.GetTextBounds(mProgressText, 0, mProgressText.Length, mProgressBounds);
			}
		}

		private int AdjustProgress(int progress)
		{
			progress = progress / 10 * 10;
			progress = 100 - progress + 10;
			if (progress > 100)
			{
				progress = 100;
			}

			return progress;
		}

		private Path CreateGasTubePath(RectF gasTubeRect)
		{
			Path path = new Path();
			path.MoveTo(gasTubeRect.Left, gasTubeRect.Top);
			path.LineTo(gasTubeRect.Left, gasTubeRect.Bottom);
			path.LineTo(gasTubeRect.Right, gasTubeRect.Bottom);
			path.LineTo(gasTubeRect.Right, gasTubeRect.Top);
			return path;
		}

		private Path CreateCannulaHeadPath(RectF cannulaRect)
		{
			Path path = new Path();
			path.MoveTo(cannulaRect.Left, cannulaRect.Top);
			path.LineTo(cannulaRect.Right, cannulaRect.Top);
			path.MoveTo(cannulaRect.CenterX(), cannulaRect.Top);
			path.LineTo(cannulaRect.CenterX(), cannulaRect.Bottom - 0.833f * cannulaRect.Width());
			return path;
		}

		private Path CreateCannulaBottomPath(RectF cannulaRect)
		{
			RectF cannulaHeadRect = new RectF(cannulaRect.Left, cannulaRect.Bottom - 0.833f * cannulaRect.Width(), cannulaRect.Right, cannulaRect.Bottom);

			Path path = new Path();
			path.AddRoundRect(cannulaHeadRect, mRectCornerRadius, mRectCornerRadius, Path.Direction.Ccw);
			return path;
		}

		/// <summary>
		/// Coordinates are approximate, you have better cooperate with the designer's design draft
		/// </summary>
		private Path CreateBalloonPath(RectF balloonRect, float progress)
		{

			Path path = new Path();
			path.MoveTo(balloonRect.CenterX(), balloonRect.Bottom);

			float progressWidth = balloonRect.Width() * progress;
			float progressHeight = balloonRect.Height() * progress;
			//draw left half
			float leftIncrementX1 = progressWidth * -0.48f;
			float leftIncrementY1 = progressHeight * 0.75f;
			float leftIncrementX2 = progressWidth * -0.03f;
			float leftIncrementY2 = progressHeight * -1.6f;
			float leftIncrementX3 = progressWidth * 0.9f;
			float leftIncrementY3 = progressHeight * -1.0f;

			path.CubicTo(balloonRect.Left + balloonRect.Width() * 0.25f + leftIncrementX1, balloonRect.CenterY() - balloonRect.Height() * 0.4f + leftIncrementY1, balloonRect.Left - balloonRect.Width() * 0.20f + leftIncrementX2, balloonRect.CenterY() + balloonRect.Height() * 1.15f + leftIncrementY2, balloonRect.Left - balloonRect.Width() * 0.4f + leftIncrementX3, balloonRect.Bottom + leftIncrementY3);

	//        the results of the left final transformation
	//        path.cubicTo(balloonRect.left - balloonRect.width() * 0.13f, balloonRect.CenterY() + balloonRect.height() * 0.35f,
	//                balloonRect.left - balloonRect.width() * 0.23f, balloonRect.CenterY() - balloonRect.height() * 0.45f,
	//                balloonRect.left + balloonRect.width() * 0.5f, balloonRect.bottom － balloonRect.height());

			//draw right half
			float rightIncrementX1 = progressWidth * 1.51f;
			float rightIncrementY1 = progressHeight * -0.05f;
			float rightIncrementX2 = progressWidth * 0.03f;
			float rightIncrementY2 = progressHeight * 0.5f;
			float rightIncrementX3 = 0.0f;
			float rightIncrementY3 = 0.0f;

			path.CubicTo(balloonRect.Left - balloonRect.Width() * 0.38f + rightIncrementX1, balloonRect.CenterY() - balloonRect.Height() * 0.4f + rightIncrementY1, balloonRect.Left + balloonRect.Width() * 1.1f + rightIncrementX2, balloonRect.CenterY() - balloonRect.Height() * 0.15f + rightIncrementY2, balloonRect.Left + balloonRect.Width() * 0.5f + rightIncrementX3, balloonRect.Bottom + rightIncrementY3);

	//        the results of the right final transformation
	//        path.cubicTo(balloonRect.left + balloonRect.width() * 1.23f, balloonRect.CenterY() - balloonRect.height() * 0.45f,
	//                balloonRect.left + balloonRect.width() * 1.13f, balloonRect.CenterY() + balloonRect.height() * 0.35f,
	//                balloonRect.left + balloonRect.width() * 0.5f, balloonRect.bottom);

			return path;
		}

		protected internal override int Alpha
        {
            set => mPaint.Alpha = value;
        }

        protected internal override ColorFilter ColorFilter
        {
            set => mPaint.SetColorFilter(value);
        }

    }
}