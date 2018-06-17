namespace MicroBluer.AndroidCtrls.LoadingAnimation.Render.Shapechange
{

    using Context = Android.Content.Context;
    using Canvas = Android.Graphics.Canvas;
    using Color = Android.Graphics.Color;
    using ColorFilter = Android.Graphics.ColorFilter;
    using Paint = Android.Graphics.Paint;
    using Path = Android.Graphics.Path;
    using Rect = Android.Graphics.Rect;
    using RectF = Android.Graphics.RectF;
	using PathMeasure = Android.Graphics.PathMeasure;
    using Interpolator = Android.Views.Animations.IInterpolator;
    using AccelerateInterpolator = Android.Views.Animations.AccelerateInterpolator;
    using DecelerateInterpolator = Android.Views.Animations.DecelerateInterpolator;
	using AccelerateDecelerateInterpolator = Android.Views.Animations.AccelerateDecelerateInterpolator;
    using MicroBluer.AndroidUtils.Common;

    public class CoolWaitLoadingRenderer : LoadingRenderer
	{
		private readonly Interpolator ACCELERATE_INTERPOLATOR08 = new AccelerateInterpolator(0.8f);
		private readonly Interpolator ACCELERATE_INTERPOLATOR10 = new AccelerateInterpolator(1.0f);
		private readonly Interpolator ACCELERATE_INTERPOLATOR15 = new AccelerateInterpolator(1.5f);

		private readonly Interpolator DECELERATE_INTERPOLATOR03 = new DecelerateInterpolator(0.3f);
		private readonly Interpolator DECELERATE_INTERPOLATOR05 = new DecelerateInterpolator(0.5f);
		private readonly Interpolator DECELERATE_INTERPOLATOR08 = new DecelerateInterpolator(0.8f);
		private readonly Interpolator DECELERATE_INTERPOLATOR10 = new DecelerateInterpolator(1.0f);

		private static readonly Interpolator ACCELERATE_DECELERATE_INTERPOLATOR = new AccelerateDecelerateInterpolator();

		private readonly float DEFAULT_WIDTH = 200.0f;
		private readonly float DEFAULT_HEIGHT = 150.0f;
		private readonly float DEFAULT_STROKE_WIDTH = 8.0f;
		private readonly float WAIT_CIRCLE_RADIUS = 50.0f;

		private const float WAIT_TRIM_DURATION_OFFSET = 0.5f;
		private const float END_TRIM_DURATION_OFFSET = 1.0f;

		private readonly long ANIMATION_DURATION = 2222;

		private readonly Paint mPaint = new Paint();

		private readonly Path mWaitPath = new Path();
		private readonly Path mCurrentTopWaitPath = new Path();
		private readonly Path mCurrentMiddleWaitPath = new Path();
		private readonly Path mCurrentBottomWaitPath = new Path();
		private readonly PathMeasure mWaitPathMeasure = new PathMeasure();

		private readonly RectF mCurrentBounds = new RectF();

		private float mStrokeWidth;
		private float mWaitCircleRadius;
		private float mOriginEndDistance;
		private float mOriginStartDistance;
		private float mWaitPathLength;

		private int mTopColor;
		private int mMiddleColor;
		private int mBottomColor;

        internal CoolWaitLoadingRenderer(Context context) : base(context)
		{
			Init(context);
			SetupPaint();
		}

		private void Init(Context context)
		{
			Width = DensityUtil.Dip2Px(context, DEFAULT_WIDTH);
			Height = DensityUtil.Dip2Px(context, DEFAULT_HEIGHT);
			mStrokeWidth = DensityUtil.Dip2Px(context, DEFAULT_STROKE_WIDTH);
			mWaitCircleRadius = DensityUtil.Dip2Px(context, WAIT_CIRCLE_RADIUS);

			mTopColor = Color.White;
			mMiddleColor = Color.ParseColor("#FFF3C742");
			mBottomColor = Color.ParseColor("#FF89CC59");

			Duration = ANIMATION_DURATION;
		}

		private void SetupPaint()
		{
			mPaint.AntiAlias = true;
			mPaint.StrokeWidth = mStrokeWidth;
			mPaint.SetStyle(Paint.Style.Stroke);
			mPaint.StrokeJoin = Paint.Join.Round;
			mPaint.StrokeCap = Paint.Cap.Round;
		}

		protected  override void Draw(Canvas canvas, Rect bounds)
		{
			int SaveCount = canvas.Save();
			RectF arcBounds = mCurrentBounds;
			arcBounds.Set(bounds);

			mPaint.Color = new Color (mBottomColor);
			canvas.DrawPath(mCurrentBottomWaitPath, mPaint);

			mPaint.Color = new Color (mMiddleColor);
			canvas.DrawPath(mCurrentMiddleWaitPath, mPaint);

			mPaint.Color = new Color (mTopColor);
			canvas.DrawPath(mCurrentTopWaitPath, mPaint);

			canvas.RestoreToCount(SaveCount);
		}

		private Path CreateWaitPath(RectF bounds)
		{
			Path path = new Path();
			//create circle
			path.MoveTo(bounds.CenterX() + mWaitCircleRadius, bounds.CenterY());

			//create w
			path.CubicTo(bounds.CenterX() + mWaitCircleRadius, bounds.CenterY() - mWaitCircleRadius * 0.5f, bounds.CenterX() + mWaitCircleRadius * 0.3f, bounds.CenterY() - mWaitCircleRadius, bounds.CenterX() - mWaitCircleRadius * 0.35f, bounds.CenterY() + mWaitCircleRadius * 0.5f);
			path.QuadTo(bounds.CenterX() + mWaitCircleRadius, bounds.CenterY() - mWaitCircleRadius, bounds.CenterX() + mWaitCircleRadius * 0.05f, bounds.CenterY() + mWaitCircleRadius * 0.5f);
			path.LineTo(bounds.CenterX() + mWaitCircleRadius * 0.75f, bounds.CenterY() - mWaitCircleRadius * 0.2f);

			path.CubicTo(bounds.CenterX(), bounds.CenterY() + mWaitCircleRadius * 1f, bounds.CenterX() + mWaitCircleRadius, bounds.CenterY() + mWaitCircleRadius * 0.4f, bounds.CenterX() + mWaitCircleRadius, bounds.CenterY());

			//create arc
			path.ArcTo(new RectF(bounds.CenterX() - mWaitCircleRadius, bounds.CenterY() - mWaitCircleRadius, bounds.CenterX() + mWaitCircleRadius, bounds.CenterY() + mWaitCircleRadius), 0, -359);
			path.ArcTo(new RectF(bounds.CenterX() - mWaitCircleRadius, bounds.CenterY() - mWaitCircleRadius, bounds.CenterX() + mWaitCircleRadius, bounds.CenterY() + mWaitCircleRadius), 1, -359);
			path.ArcTo(new RectF(bounds.CenterX() - mWaitCircleRadius, bounds.CenterY() - mWaitCircleRadius, bounds.CenterX() + mWaitCircleRadius, bounds.CenterY() + mWaitCircleRadius), 2, -2);
			//create w
			path.CubicTo(bounds.CenterX() + mWaitCircleRadius, bounds.CenterY() - mWaitCircleRadius * 0.5f, bounds.CenterX() + mWaitCircleRadius * 0.3f, bounds.CenterY() - mWaitCircleRadius, bounds.CenterX() - mWaitCircleRadius * 0.35f, bounds.CenterY() + mWaitCircleRadius * 0.5f);
			path.QuadTo(bounds.CenterX() + mWaitCircleRadius, bounds.CenterY() - mWaitCircleRadius, bounds.CenterX() + mWaitCircleRadius * 0.05f, bounds.CenterY() + mWaitCircleRadius * 0.5f);
			path.LineTo(bounds.CenterX() + mWaitCircleRadius * 0.75f, bounds.CenterY() - mWaitCircleRadius * 0.2f);

			path.CubicTo(bounds.CenterX(), bounds.CenterY() + mWaitCircleRadius * 1f, bounds.CenterX() + mWaitCircleRadius, bounds.CenterY() + mWaitCircleRadius * 0.4f, bounds.CenterX() + mWaitCircleRadius, bounds.CenterY());

			return path;
		}

		protected internal override void ComputeRender(float renderProgress)
		{
			if (mCurrentBounds.IsEmpty)
			{
				return;
			}

			if (mWaitPath.IsEmpty)
			{
				mWaitPath.Set(CreateWaitPath(mCurrentBounds));
				mWaitPathMeasure.SetPath(mWaitPath, false);
				mWaitPathLength = mWaitPathMeasure.Length;

				mOriginEndDistance = mWaitPathLength * 0.255f;
				mOriginStartDistance = mWaitPathLength * 0.045f;
			}

			mCurrentTopWaitPath.Reset();
			mCurrentMiddleWaitPath.Reset();
			mCurrentBottomWaitPath.Reset();

			//Draw the first half : top
			if (renderProgress <= WAIT_TRIM_DURATION_OFFSET)
			{
				float topTrimProgress = ACCELERATE_DECELERATE_INTERPOLATOR.GetInterpolation(renderProgress / WAIT_TRIM_DURATION_OFFSET);
				float topEndDistance = mOriginEndDistance + mWaitPathLength * 0.3f * topTrimProgress;
				float topStartDistance = mOriginStartDistance + mWaitPathLength * 0.48f * topTrimProgress;
				mWaitPathMeasure.GetSegment(topStartDistance, topEndDistance, mCurrentTopWaitPath, true);
			}

			//Draw the first half : middle
			if (renderProgress > 0.02f * WAIT_TRIM_DURATION_OFFSET && renderProgress <= WAIT_TRIM_DURATION_OFFSET * 0.75f)
			{
				float middleStartTrimProgress = ACCELERATE_INTERPOLATOR10.GetInterpolation((renderProgress - 0.02f * WAIT_TRIM_DURATION_OFFSET) / (WAIT_TRIM_DURATION_OFFSET * 0.73f));
				float middleEndTrimProgress = DECELERATE_INTERPOLATOR08.GetInterpolation((renderProgress - 0.02f * WAIT_TRIM_DURATION_OFFSET) / (WAIT_TRIM_DURATION_OFFSET * 0.73f));

				float middleEndDistance = mOriginStartDistance + mWaitPathLength * 0.42f * middleEndTrimProgress;
				float middleStartDistance = mOriginStartDistance + mWaitPathLength * 0.42f * middleStartTrimProgress;
				mWaitPathMeasure.GetSegment(middleStartDistance, middleEndDistance, mCurrentMiddleWaitPath, true);
			}

			//Draw the first half : bottom
			if (renderProgress > 0.04f * WAIT_TRIM_DURATION_OFFSET && renderProgress <= WAIT_TRIM_DURATION_OFFSET * 0.75f)
			{
				float bottomStartTrimProgress = ACCELERATE_INTERPOLATOR15.GetInterpolation((renderProgress - 0.04f * WAIT_TRIM_DURATION_OFFSET) / (WAIT_TRIM_DURATION_OFFSET * 0.71f));
				float bottomEndTrimProgress = DECELERATE_INTERPOLATOR05.GetInterpolation((renderProgress - 0.04f * WAIT_TRIM_DURATION_OFFSET) / (WAIT_TRIM_DURATION_OFFSET * 0.71f));

				float bottomEndDistance = mOriginStartDistance + mWaitPathLength * 0.42f * bottomEndTrimProgress;
				float bottomStartDistance = mOriginStartDistance + mWaitPathLength * 0.42f * bottomStartTrimProgress;
				mWaitPathMeasure.GetSegment(bottomStartDistance, bottomEndDistance, mCurrentBottomWaitPath, true);
			}

			//Draw the last half : top
			if (renderProgress <= END_TRIM_DURATION_OFFSET && renderProgress > WAIT_TRIM_DURATION_OFFSET)
			{
				float trimProgress = ACCELERATE_DECELERATE_INTERPOLATOR.GetInterpolation((renderProgress - WAIT_TRIM_DURATION_OFFSET) / (END_TRIM_DURATION_OFFSET - WAIT_TRIM_DURATION_OFFSET));
				float topEndDistance = mOriginEndDistance + mWaitPathLength * 0.3f + mWaitPathLength * 0.45f * trimProgress;
				float topStartDistance = mOriginStartDistance + mWaitPathLength * 0.48f + mWaitPathLength * 0.27f * trimProgress;
				mWaitPathMeasure.GetSegment(topStartDistance, topEndDistance, mCurrentTopWaitPath, true);
			}

			//Draw the last half : middle
			if (renderProgress > WAIT_TRIM_DURATION_OFFSET + 0.02f * WAIT_TRIM_DURATION_OFFSET && renderProgress <= WAIT_TRIM_DURATION_OFFSET + WAIT_TRIM_DURATION_OFFSET * 0.62f)
			{
				float middleStartTrimProgress = ACCELERATE_INTERPOLATOR08.GetInterpolation((renderProgress - WAIT_TRIM_DURATION_OFFSET - 0.02f * WAIT_TRIM_DURATION_OFFSET) / (WAIT_TRIM_DURATION_OFFSET * 0.60f));
				float middleEndTrimProgress = DECELERATE_INTERPOLATOR03.GetInterpolation((renderProgress - WAIT_TRIM_DURATION_OFFSET - 0.02f * WAIT_TRIM_DURATION_OFFSET) / (WAIT_TRIM_DURATION_OFFSET * 0.60f));

				float middleEndDistance = mOriginStartDistance + mWaitPathLength * 0.48f + mWaitPathLength * 0.20f * middleEndTrimProgress;
				float middleStartDistance = mOriginStartDistance + mWaitPathLength * 0.48f + mWaitPathLength * 0.10f * middleStartTrimProgress;
				mWaitPathMeasure.GetSegment(middleStartDistance, middleEndDistance, mCurrentMiddleWaitPath, true);
			}

			if (renderProgress > WAIT_TRIM_DURATION_OFFSET + 0.62f * WAIT_TRIM_DURATION_OFFSET && renderProgress <= END_TRIM_DURATION_OFFSET)
			{
				float middleStartTrimProgress = DECELERATE_INTERPOLATOR10.GetInterpolation((renderProgress - WAIT_TRIM_DURATION_OFFSET - 0.62f * WAIT_TRIM_DURATION_OFFSET) / (WAIT_TRIM_DURATION_OFFSET * 0.38f));
				float middleEndTrimProgress = DECELERATE_INTERPOLATOR03.GetInterpolation((renderProgress - WAIT_TRIM_DURATION_OFFSET - 0.62f * WAIT_TRIM_DURATION_OFFSET) / (WAIT_TRIM_DURATION_OFFSET * 0.38f));

				float middleEndDistance = mOriginStartDistance + mWaitPathLength * 0.68f + mWaitPathLength * 0.325f * middleEndTrimProgress;
				float middleStartDistance = mOriginStartDistance + mWaitPathLength * 0.58f + mWaitPathLength * 0.17f * middleStartTrimProgress;
				mWaitPathMeasure.GetSegment(middleStartDistance, middleEndDistance, mCurrentMiddleWaitPath, true);
			}

			//Draw the last half : bottom
			if (renderProgress > WAIT_TRIM_DURATION_OFFSET + 0.10f * WAIT_TRIM_DURATION_OFFSET && renderProgress <= WAIT_TRIM_DURATION_OFFSET + WAIT_TRIM_DURATION_OFFSET * 0.70f)
			{
				float bottomStartTrimProgress = ACCELERATE_INTERPOLATOR15.GetInterpolation((renderProgress - WAIT_TRIM_DURATION_OFFSET - 0.10f * WAIT_TRIM_DURATION_OFFSET) / (WAIT_TRIM_DURATION_OFFSET * 0.60f));
				float bottomEndTrimProgress = DECELERATE_INTERPOLATOR03.GetInterpolation((renderProgress - WAIT_TRIM_DURATION_OFFSET - 0.10f * WAIT_TRIM_DURATION_OFFSET) / (WAIT_TRIM_DURATION_OFFSET * 0.60f));

				float bottomEndDistance = mOriginStartDistance + mWaitPathLength * 0.48f + mWaitPathLength * 0.20f * bottomEndTrimProgress;
				float bottomStartDistance = mOriginStartDistance + mWaitPathLength * 0.48f + mWaitPathLength * 0.10f * bottomStartTrimProgress;
				mWaitPathMeasure.GetSegment(bottomStartDistance, bottomEndDistance, mCurrentBottomWaitPath, true);
			}

			if (renderProgress > WAIT_TRIM_DURATION_OFFSET + 0.70f * WAIT_TRIM_DURATION_OFFSET && renderProgress <= END_TRIM_DURATION_OFFSET)
			{
				float bottomStartTrimProgress = DECELERATE_INTERPOLATOR05.GetInterpolation((renderProgress - WAIT_TRIM_DURATION_OFFSET - 0.70f * WAIT_TRIM_DURATION_OFFSET) / (WAIT_TRIM_DURATION_OFFSET * 0.30f));
				float bottomEndTrimProgress = DECELERATE_INTERPOLATOR03.GetInterpolation((renderProgress - WAIT_TRIM_DURATION_OFFSET - 0.70f * WAIT_TRIM_DURATION_OFFSET) / (WAIT_TRIM_DURATION_OFFSET * 0.30f));

				float bottomEndDistance = mOriginStartDistance + mWaitPathLength * 0.68f + mWaitPathLength * 0.325f * bottomEndTrimProgress;
				float bottomStartDistance = mOriginStartDistance + mWaitPathLength * 0.58f + mWaitPathLength * 0.17f * bottomStartTrimProgress;
				mWaitPathMeasure.GetSegment(bottomStartDistance, bottomEndDistance, mCurrentBottomWaitPath, true);
			}

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