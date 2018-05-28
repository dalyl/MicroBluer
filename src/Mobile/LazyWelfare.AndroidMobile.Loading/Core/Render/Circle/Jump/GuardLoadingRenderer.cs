namespace LazyWelfare.AndroidMobile.Loading.Render.Circle.Jump
{
    using Context = Android.Content.Context;
    using Canvas = Android.Graphics.Canvas;
    using Color = Android.Graphics.Color;
    using ColorFilter = Android.Graphics.ColorFilter;
    using Paint = Android.Graphics.Paint;
    using LinearGradient = Android.Graphics.LinearGradient;
    using Path = Android.Graphics.Path;
    using RectF = Android.Graphics.RectF;
    using Shader = Android.Graphics.Shader;
    using Interpolator = Android.Views.Animations.IInterpolator;
    using AccelerateInterpolator = Android.Views.Animations.AccelerateInterpolator;
    using DecelerateInterpolator = Android.Views.Animations.DecelerateInterpolator;
    using Android.Graphics;
    using System;

	using PathMeasure = Android.Graphics.PathMeasure;
	using FastOutSlowInInterpolator = Android.Support.V4.View.Animation.FastOutSlowInInterpolator;


	public class GuardLoadingRenderer : LoadingRenderer
	{
		private static readonly Interpolator MATERIAL_INTERPOLATOR = new FastOutSlowInInterpolator();
		private static readonly Interpolator ACCELERATE_INTERPOLATOR = new AccelerateInterpolator();
		private static readonly Interpolator DECELERATE_INTERPOLATOR = new DecelerateInterpolator();

		private const long ANIMATION_DURATION = 5000;

		private const float DEFAULT_STROKE_WIDTH = 1.0f;
		private const float DEFAULT_CENTER_RADIUS = 12.5f;
		private const float DEFAULT_SKIP_BALL_RADIUS = 1.0f;

		private const float START_TRIM_INIT_ROTATION = -0.5f;
		private const float START_TRIM_MAX_ROTATION = -0.25f;
		private const float END_TRIM_INIT_ROTATION = 0.25f;
		private const float END_TRIM_MAX_ROTATION = 0.75f;

		private const float START_TRIM_DURATION_OFFSET = 0.23f;
		private const float WAVE_DURATION_OFFSET = 0.36f;
		private const float BALL_SKIP_DURATION_OFFSET = 0.74f;
		private const float BALL_SCALE_DURATION_OFFSET = 0.82f;
		private const float END_TRIM_DURATION_OFFSET = 1.0f;

		private static readonly int DEFAULT_COLOR = Color.White;
		private static readonly int DEFAULT_BALL_COLOR = Color.Red;

		private readonly Paint mPaint = new Paint();
		private readonly RectF mTempBounds = new RectF();
		private readonly RectF mCurrentBounds = new RectF();
		private readonly float[] mCurrentPosition = new float[2];

		private float mStrokeInset;
		private float mSkipBallSize;

		private float mScale;
		private float mEndTrim;
		private float mRotation;
		private float mStartTrim;
		private float mWaveProgress;

		private float mStrokeWidth;
		private float mCenterRadius;

		private int mColor;
		private int mBallColor;

		private PathMeasure mPathMeasure;

		private GuardLoadingRenderer(Context context) : base(context)
		{

			mDuration = ANIMATION_DURATION;
			Init(context);
			SetupPaint();
		}

		private void Init(Context context)
		{
			mStrokeWidth = DensityUtil.dip2px(context, DEFAULT_STROKE_WIDTH);
			mCenterRadius = DensityUtil.dip2px(context, DEFAULT_CENTER_RADIUS);
			mSkipBallSize = DensityUtil.dip2px(context, DEFAULT_SKIP_BALL_RADIUS);

			mColor = DEFAULT_COLOR;
			mBallColor = DEFAULT_BALL_COLOR;
		}

		private void SetupPaint()
		{
			mPaint.AntiAlias = true;
			mPaint.StrokeWidth = mStrokeWidth;
			mPaint.SetStyle(Paint.Style.Stroke);
			mPaint.StrokeCap = Paint.Cap.Round;

			SetInsets((int) mWidth, (int) mHeight);
		}

		protected internal override void Draw(Canvas canvas, Rect bounds)
		{
			RectF arcBounds = mTempBounds;
			arcBounds.Set(bounds);
			arcBounds.Inset(mStrokeInset, mStrokeInset);
			mCurrentBounds.Set(arcBounds);

			int saveCount = canvas.Save();

			//draw circle trim
			float startAngle = (mStartTrim + mRotation) * 360;
			float endAngle = (mEndTrim + mRotation) * 360;
			float sweepAngle = endAngle - startAngle;
			if (sweepAngle != 0)
			{
				mPaint.Color = new Color (mColor);
				mPaint.SetStyle (Paint.Style.Stroke);
				canvas.DrawArc(arcBounds, startAngle, sweepAngle, false, mPaint);
			}

			//draw water wave
			if (mWaveProgress < 1.0f)
			{
                var nColor = new Color(mColor); 
               

                mPaint.Color = Color.Argb((int)(Color.GetAlphaComponent(mColor) * (1.0f - mWaveProgress)), Color.GetRedComponent(mColor), Color.GetGreenComponent(mColor), Color.GetBlueComponent(mColor));

                mPaint.SetStyle (Paint.Style.Stroke);
				float radius = Math.Min(arcBounds.Width(), arcBounds.Height()) / 2.0f;
				canvas.DrawCircle(arcBounds.CenterX(), arcBounds.CenterY(), radius * (1.0f + mWaveProgress), mPaint);
			}
			//draw ball bounce
			if (mPathMeasure != null)
			{
				mPaint.Color = new Color (mBallColor);
				mPaint.SetStyle (Paint.Style.Fill);
				canvas.DrawCircle(mCurrentPosition[0], mCurrentPosition[1], mSkipBallSize * mScale, mPaint);
			}

			canvas.RestoreToCount(saveCount);
		}

		protected internal override void ComputeRender(float renderProgress)
		{
			if (renderProgress <= START_TRIM_DURATION_OFFSET)
			{
				float startTrimProgress = (renderProgress) / START_TRIM_DURATION_OFFSET;
				mEndTrim = -MATERIAL_INTERPOLATOR.GetInterpolation(startTrimProgress);
				mRotation = START_TRIM_INIT_ROTATION + START_TRIM_MAX_ROTATION * MATERIAL_INTERPOLATOR.GetInterpolation(startTrimProgress);
			}

			if (renderProgress <= WAVE_DURATION_OFFSET && renderProgress > START_TRIM_DURATION_OFFSET)
			{
				float waveProgress = (renderProgress - START_TRIM_DURATION_OFFSET) / (WAVE_DURATION_OFFSET - START_TRIM_DURATION_OFFSET);
				mWaveProgress = ACCELERATE_INTERPOLATOR.GetInterpolation(waveProgress);
			}

			if (renderProgress <= BALL_SKIP_DURATION_OFFSET && renderProgress > WAVE_DURATION_OFFSET)
			{
				if (mPathMeasure == null)
				{
					mPathMeasure = new PathMeasure(CreateSkipBallPath(), false);
				}

				float ballSkipProgress = (renderProgress - WAVE_DURATION_OFFSET) / (BALL_SKIP_DURATION_OFFSET - WAVE_DURATION_OFFSET);
				mPathMeasure.GetPosTan(ballSkipProgress * mPathMeasure.Length, mCurrentPosition, null);

				mWaveProgress = 1.0f;
			}

			if (renderProgress <= BALL_SCALE_DURATION_OFFSET && renderProgress > BALL_SKIP_DURATION_OFFSET)
			{
				float ballScaleProgress = (renderProgress - BALL_SKIP_DURATION_OFFSET) / (BALL_SCALE_DURATION_OFFSET - BALL_SKIP_DURATION_OFFSET);
				if (ballScaleProgress < 0.5f)
				{
					mScale = 1.0f + DECELERATE_INTERPOLATOR.GetInterpolation(ballScaleProgress * 2.0f);
				}
				else
				{
					mScale = 2.0f - ACCELERATE_INTERPOLATOR.GetInterpolation((ballScaleProgress - 0.5f) * 2.0f) * 2.0f;
				}
			}

			if (renderProgress >= BALL_SCALE_DURATION_OFFSET)
			{
				float endTrimProgress = (renderProgress - BALL_SKIP_DURATION_OFFSET) / (END_TRIM_DURATION_OFFSET - BALL_SKIP_DURATION_OFFSET);
				mEndTrim = -1 + MATERIAL_INTERPOLATOR.GetInterpolation(endTrimProgress);
				mRotation = END_TRIM_INIT_ROTATION + END_TRIM_MAX_ROTATION * MATERIAL_INTERPOLATOR.GetInterpolation(endTrimProgress);

				mScale = 1.0f;
				mPathMeasure = null;
			}
		}

		protected internal override int Alpha
		{
			set
			{
				mPaint.Alpha = value;
    
			}
		}

		protected internal override ColorFilter ColorFilter
		{
			set
			{
				mPaint.SetColorFilter (value);
    
			}
		}

		protected internal override void Reset()
		{
			mScale = 1.0f;
			mEndTrim = 0.0f;
			mRotation = 0.0f;
			mStartTrim = 0.0f;
			mWaveProgress = 1.0f;
		}

		private Path CreateSkipBallPath()
		{
			float radius = Math.Min(mCurrentBounds.Width(), mCurrentBounds.Height()) / 2.0f;
			float radiusPow2 = (float) Math.Pow(radius, 2.0f);
			float originCoordinateX = mCurrentBounds.CenterX();
			float originCoordinateY = mCurrentBounds.CenterY();

			float[] coordinateX = new float[]{0.0f, 0.0f, -0.8f * radius, 0.75f * radius, -0.45f * radius, 0.9f * radius, -0.5f * radius};
			float[] sign = new float[]{1.0f, -1.0f, 1.0f, 1.0f, -1.0f, -1.0f, 1.0f};

			Path path = new Path();
			for (int i = 0; i < coordinateX.Length; i++)
			{
				// x^2 + y^2 = radius^2 --> y = sqrt(radius^2 - x^2)
				if (i == 0)
				{
					path.MoveTo(originCoordinateX + coordinateX[i], originCoordinateY + sign[i] * (float) Math.Sqrt(radiusPow2 - Math.Pow(coordinateX[i], 2.0f)));
					continue;
				}

				path.LineTo(originCoordinateX + coordinateX[i], originCoordinateY + sign[i] * (float) Math.Sqrt(radiusPow2 - Math.Pow(coordinateX[i], 2.0f)));

				if (i == coordinateX.Length - 1)
				{
					path.LineTo(originCoordinateX, originCoordinateY);
				}
			}
			return path;
		}

		private void SetInsets(int width, int height)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float minEdge = (float) Math.min(width, height);
			float minEdge = (float) Math.Min(width, height);
			float insets;
			if (mCenterRadius <= 0 || minEdge < 0)
			{
				insets = (float) Math.Ceiling(mStrokeWidth / 2.0f);
			}
			else
			{
				insets = minEdge / 2.0f - mCenterRadius;
			}
			mStrokeInset = insets;
		}

		public class Builder
		{
			internal Context mContext;

			public Builder(Context mContext)
			{
				this.mContext = mContext;
			}

			public virtual GuardLoadingRenderer Build()
			{
				GuardLoadingRenderer loadingRenderer = new GuardLoadingRenderer(mContext);
				return loadingRenderer;
			}
		}
	}

}