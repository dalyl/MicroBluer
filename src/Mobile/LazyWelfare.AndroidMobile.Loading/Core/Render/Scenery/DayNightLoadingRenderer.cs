namespace LazyWelfare.AndroidMobile.Loading.Render.Scenery
{

    using Context = Android.Content.Context;
    using Canvas = Android.Graphics.Canvas;
    using Color = Android.Graphics.Color;
    using ColorFilter = Android.Graphics.ColorFilter;
    using Paint = Android.Graphics.Paint;
	using PointF = Android.Graphics.PointF;
    using Path = Android.Graphics.Path;
    using Rect = Android.Graphics.Rect;
    using RectF = Android.Graphics.RectF;
    using Interpolator = Android.Views.Animations.IInterpolator;
    using AccelerateInterpolator = Android.Views.Animations.AccelerateInterpolator;
    using DecelerateInterpolator = Android.Views.Animations.DecelerateInterpolator;
	using FastOutLinearInInterpolator = Android.Support.V4.View.Animation.FastOutLinearInInterpolator;
    using FastOutSlowInInterpolator = Android.Support.V4.View.Animation.FastOutSlowInInterpolator;
    using Animator = Android.Animation.Animator;
    using AnimatorListenerAdapter = Android.Animation.AnimatorListenerAdapter;
    using LinearInterpolator = Android.Views.Animations.LinearInterpolator;
    using LazyWelfare.AndroidUtils.Common;
    using System.Collections.Generic;
    using System;
    using LazyWelfare.AndroidUtils.Animation;

    public class DayNightLoadingRenderer : LoadingRenderer
	{
		private static readonly Interpolator MATERIAL_INTERPOLATOR = new FastOutSlowInInterpolator();
		private static readonly Interpolator LINEAR_INTERPOLATOR = new LinearInterpolator();
		private static readonly Interpolator DECELERATE_INTERPOLATOR = new DecelerateInterpolator();
		private static readonly Interpolator ACCELERATE_INTERPOLATOR = new AccelerateInterpolator();
		private static readonly Interpolator FASTOUTLINEARIN_INTERPOLATOR = new FastOutLinearInInterpolator();

		private static readonly Interpolator[] INTERPOLATORS = new Interpolator[]{LINEAR_INTERPOLATOR, DECELERATE_INTERPOLATOR, ACCELERATE_INTERPOLATOR, FASTOUTLINEARIN_INTERPOLATOR, MATERIAL_INTERPOLATOR};

		private const int MAX_ALPHA = 255;
		private const int DEGREE_360 = 360;
		private const int MAX_SUN_RAY_COUNT = 12;

		private const float DEFAULT_Width = 200.0f;
		private const float DEFAULT_Height = 150.0f;
		private const float DEFAULT_STROKE_Width = 2.5f;
		private const float DEFAULT_SUN_MOON_RADIUS = 12.0f;
		private const float DEFAULT_STAR_RADIUS = 2.5f;
		private const float DEFAULT_SUN_RAY_LENGTH = 10.0f;
		private const float DEFAULT_SUN_RAY_OFFSET = 3.0f;

		public const float STAR_RISE_PROGRESS_OFFSET = 0.2f;
		public const float STAR_DECREASE_PROGRESS_OFFSET = 0.8f;
		public const float STAR_FLASH_PROGRESS_PERCENTAGE = 0.2f;

		private static readonly float MAX_SUN_ROTATE_DEGREE = DEGREE_360 / 3.0f;
		private static readonly float MAX_MOON_ROTATE_DEGREE = DEGREE_360 / 6.0f;
		private static readonly float SUN_RAY_INTERVAL_DEGREE = DEGREE_360 / 3.0f / 55;

		private const float SUN_RISE_DURATION_OFFSET = 0.143f;
		private const float SUN_ROTATE_DURATION_OFFSET = 0.492f;
		private const float SUN_DECREASE_DURATION_OFFSET = 0.570f;
		private const float MOON_RISE_DURATION_OFFSET = 0.713f;
		private const float MOON_DECREASE_START_DURATION_OFFSET = 0.935f;
		private const float MOON_DECREASE_END_DURATION_OFFSET = 1.0f;
		private const float STAR_RISE_START_DURATION_OFFSET = 0.684f;
		private const float STAR_DECREASE_START_DURATION_OFFSET = 1.0f;

		private static readonly int DEFAULT_COLOR = Color.ParseColor("#ff21fd8e");

		private const long ANIMATION_DURATION = 5111;

		private static readonly Random mRandom = new Random();
		private readonly IList<StarHolder> mStarHolders = new List<StarHolder>();

		private readonly Paint mPaint = new Paint();
		private readonly RectF mTempBounds = new RectF();

		private readonly Animator.IAnimatorListener mAnimatorListener = new AnonymousAnimatorListenerAdapter();

		private int mCurrentColor;

		private float mMaxStarOffsets;
		private float mStrokeWidth;
		private float mStarRadius;
		private float mSun_MoonRadius;
		private float mSunCoordinateY;
		private float mMoonCoordinateY;
		//the y-coordinate of the end point of the sun ray
		private float mSunRayEndCoordinateY;
		//the y-coordinate of the start point of the sun ray
		private float mSunRayStartCoordinateY;
		//the y-coordinate of the start point of the sun
		private float mInitSun_MoonCoordinateY;
		//the distance from the outside to the center of the drawable
		private float mMaxSun_MoonRiseDistance;

		private float mSunRayRotation;
		private float mMoonRotation;

		//the number of sun's rays is increasing
		private bool mIsExpandSunRay;
		private bool mShowStar;

		private int mSunRayCount;

		internal DayNightLoadingRenderer(Context context) : base(context)
		{
			Init(context);
			SetupPaint();
			AddRenderListener(mAnimatorListener);
		}

		private void Init(Context context)
		{
			mWidth = DensityUtil.Dip2Px(context, DEFAULT_Width);
			mHeight = DensityUtil.Dip2Px(context, DEFAULT_Height);
			mStrokeWidth = DensityUtil.Dip2Px(context, DEFAULT_STROKE_Width);

			mStarRadius = DensityUtil.Dip2Px(context, DEFAULT_STAR_RADIUS);
			mSun_MoonRadius = DensityUtil.Dip2Px(context, DEFAULT_SUN_MOON_RADIUS);
			mInitSun_MoonCoordinateY = mHeight + mSun_MoonRadius + mStrokeWidth * 2.0f;
			mMaxSun_MoonRiseDistance = mHeight / 2.0f + mSun_MoonRadius;

			mSunRayStartCoordinateY = mInitSun_MoonCoordinateY - mMaxSun_MoonRiseDistance - mSun_MoonRadius - mStrokeWidth - DensityUtil.Dip2Px(context, DEFAULT_SUN_RAY_OFFSET); //sub the interval between the sun and the sun ray -  sub the with the sun circle - sub the radius - the center

			//add strokeWidth * 2.0f because the stroke cap is Paint.Cap.ROUND
			mSunRayEndCoordinateY = mSunRayStartCoordinateY - DensityUtil.Dip2Px(context, DEFAULT_SUN_RAY_LENGTH) + mStrokeWidth;

			mSunCoordinateY = mInitSun_MoonCoordinateY;
			mMoonCoordinateY = mInitSun_MoonCoordinateY;

			mCurrentColor = DEFAULT_COLOR;

			mDuration = ANIMATION_DURATION;
		}

		private void SetupPaint()
		{
			mPaint.AntiAlias = true;
			mPaint.StrokeWidth = mStrokeWidth;
			mPaint.StrokeJoin = Paint.Join.Round;
			mPaint.SetStyle(Paint.Style.Stroke);
			mPaint.StrokeCap = Paint.Cap.Round;
		}

		protected internal override void Draw(Canvas canvas, Rect bounds)
		{
			int saveCount = canvas.Save();

			RectF arcBounds = mTempBounds;
			arcBounds.Set(bounds);

			mPaint.Alpha = MAX_ALPHA;
			mPaint.SetStyle(Paint.Style.Stroke);
            mPaint.Color = new Color (mCurrentColor);

			if (mSunCoordinateY < mInitSun_MoonCoordinateY)
			{
				canvas.DrawCircle(arcBounds.CenterX(), mSunCoordinateY, mSun_MoonRadius, mPaint);
			}

			if (mMoonCoordinateY < mInitSun_MoonCoordinateY)
			{
				int moonSaveCount = canvas.Save();
				canvas.Rotate(mMoonRotation, arcBounds.CenterX(), mMoonCoordinateY);
				canvas.DrawPath(CreateMoonPath(arcBounds.CenterX(), mMoonCoordinateY), mPaint);
				canvas.RestoreToCount(moonSaveCount);
			}

			for (int i = 0; i < mSunRayCount; i++)
			{
				int sunRaySaveCount = canvas.Save();
				//rotate 45 degrees can change the direction of 0 degrees to 1:30 clock
				//-mSunRayRotation means reverse rotation
				canvas.Rotate(45 - mSunRayRotation + (mIsExpandSunRay ? i : MAX_SUN_RAY_COUNT - i) * DEGREE_360 / MAX_SUN_RAY_COUNT, arcBounds.CenterX(), mSunCoordinateY);

				canvas.DrawLine(arcBounds.CenterX(), mSunRayStartCoordinateY, arcBounds.CenterX(), mSunRayEndCoordinateY, mPaint);
				canvas.RestoreToCount(sunRaySaveCount);
			}

			if (mShowStar)
			{
				if (mStarHolders.Count == 0)
				{
					InitStarHolders(arcBounds);
				}

				for (int i = 0; i < mStarHolders.Count; i++)
				{
					mPaint.SetStyle(Paint.Style.Fill);
					mPaint.Alpha = mStarHolders[i].mAlpha;
					canvas.DrawCircle(mStarHolders[i].mCurrentPoint.X, mStarHolders[i].mCurrentPoint.Y, mStarRadius, mPaint);
				}
			}

			canvas.RestoreToCount(saveCount);
		}

		protected internal override void ComputeRender(float renderProgress)
		{
			if (renderProgress <= SUN_RISE_DURATION_OFFSET)
			{
				float sunRiseProgress = renderProgress / SUN_RISE_DURATION_OFFSET;
				mSunCoordinateY = mInitSun_MoonCoordinateY - mMaxSun_MoonRiseDistance * MATERIAL_INTERPOLATOR.GetInterpolation(sunRiseProgress);
				mMoonCoordinateY = mInitSun_MoonCoordinateY;
				mShowStar = false;
			}

			if (renderProgress <= SUN_ROTATE_DURATION_OFFSET && renderProgress > SUN_RISE_DURATION_OFFSET)
			{
				float sunRotateProgress = (renderProgress - SUN_RISE_DURATION_OFFSET) / (SUN_ROTATE_DURATION_OFFSET - SUN_RISE_DURATION_OFFSET);
				mSunRayRotation = sunRotateProgress * MAX_SUN_ROTATE_DEGREE;

				if ((int)(mSunRayRotation / SUN_RAY_INTERVAL_DEGREE) <= MAX_SUN_RAY_COUNT)
				{
					mIsExpandSunRay = true;
					mSunRayCount = (int)(mSunRayRotation / SUN_RAY_INTERVAL_DEGREE);
				}

				if ((int)((MAX_SUN_ROTATE_DEGREE - mSunRayRotation) / SUN_RAY_INTERVAL_DEGREE) <= MAX_SUN_RAY_COUNT)
				{
					mIsExpandSunRay = false;
					mSunRayCount = (int)((MAX_SUN_ROTATE_DEGREE - mSunRayRotation) / SUN_RAY_INTERVAL_DEGREE);
				}
			}

			if (renderProgress <= SUN_DECREASE_DURATION_OFFSET && renderProgress > SUN_ROTATE_DURATION_OFFSET)
			{
				float sunDecreaseProgress = (renderProgress - SUN_ROTATE_DURATION_OFFSET) / (SUN_DECREASE_DURATION_OFFSET - SUN_ROTATE_DURATION_OFFSET);
				mSunCoordinateY = mInitSun_MoonCoordinateY - mMaxSun_MoonRiseDistance * (1.0f - ACCELERATE_INTERPOLATOR.GetInterpolation(sunDecreaseProgress));
			}

			if (renderProgress <= MOON_RISE_DURATION_OFFSET && renderProgress > SUN_DECREASE_DURATION_OFFSET)
			{
				float moonRiseProgress = (renderProgress - SUN_DECREASE_DURATION_OFFSET) / (MOON_RISE_DURATION_OFFSET - SUN_DECREASE_DURATION_OFFSET);
				mMoonRotation = MATERIAL_INTERPOLATOR.GetInterpolation(moonRiseProgress) * MAX_MOON_ROTATE_DEGREE;
				mSunCoordinateY = mInitSun_MoonCoordinateY;
				mMoonCoordinateY = mInitSun_MoonCoordinateY - mMaxSun_MoonRiseDistance * MATERIAL_INTERPOLATOR.GetInterpolation(moonRiseProgress);
			}

			if (renderProgress <= STAR_DECREASE_START_DURATION_OFFSET && renderProgress > STAR_RISE_START_DURATION_OFFSET)
			{
				float starProgress = (renderProgress - STAR_RISE_START_DURATION_OFFSET) / (STAR_DECREASE_START_DURATION_OFFSET - STAR_RISE_START_DURATION_OFFSET);
				if (starProgress <= STAR_RISE_PROGRESS_OFFSET)
				{
					for (int i = 0; i < mStarHolders.Count; i++)
					{
						StarHolder starHolder = mStarHolders[i];
						starHolder.mCurrentPoint.Y = starHolder.mPoint.Y - (1.0f - starHolder.mInterpolator.GetInterpolation(starProgress * 5.0f)) * (mMaxStarOffsets * 0.65f);
						starHolder.mCurrentPoint.X = starHolder.mPoint.X;
					}
				}

				if (starProgress > STAR_RISE_PROGRESS_OFFSET && starProgress < STAR_DECREASE_PROGRESS_OFFSET)
				{
					for (int i = 0; i < mStarHolders.Count; i++)
					{
						StarHolder starHolder = mStarHolders[i];
						if (starHolder.mFlashOffset < starProgress && starProgress < starHolder.mFlashOffset + STAR_FLASH_PROGRESS_PERCENTAGE)
						{
							starHolder.mAlpha = (int)(MAX_ALPHA * MATERIAL_INTERPOLATOR.GetInterpolation(Math.Abs(starProgress - (starHolder.mFlashOffset + STAR_FLASH_PROGRESS_PERCENTAGE / 2.0f)) / (STAR_FLASH_PROGRESS_PERCENTAGE / 2.0f)));
						}
					}
				}

				if (starProgress >= STAR_DECREASE_PROGRESS_OFFSET)
				{
					for (int i = 0; i < mStarHolders.Count; i++)
					{
						StarHolder starHolder = mStarHolders[i];
						starHolder.mCurrentPoint.Y = starHolder.mPoint.Y + starHolder.mInterpolator.GetInterpolation((starProgress - STAR_DECREASE_PROGRESS_OFFSET) * 5.0f) * mMaxStarOffsets;
						starHolder.mCurrentPoint.X = starHolder.mPoint.X;
					}
				}
				mShowStar = true;
			}

			if (renderProgress <= MOON_DECREASE_END_DURATION_OFFSET && renderProgress > MOON_DECREASE_START_DURATION_OFFSET)
			{
				float moonDecreaseProgress = (renderProgress - MOON_DECREASE_START_DURATION_OFFSET) / (MOON_DECREASE_END_DURATION_OFFSET - MOON_DECREASE_START_DURATION_OFFSET);
				mMoonCoordinateY = mInitSun_MoonCoordinateY - mMaxSun_MoonRiseDistance * (1.0f - ACCELERATE_INTERPOLATOR.GetInterpolation(moonDecreaseProgress));
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

        private void InitStarHolders(RectF currentBounds)
		{

            mStarHolders.Add(new StarHolder( 0.3f, new PointF(currentBounds.Left + currentBounds.Width() * 0.175f, currentBounds.Top + currentBounds.Height() * 0.0934f)));
			mStarHolders.Add(new StarHolder( 0.2f, new PointF(currentBounds.Left + currentBounds.Width() * 0.175f, currentBounds.Top + currentBounds.Height() * 0.62f)));
			mStarHolders.Add(new StarHolder( 0.2f, new PointF(currentBounds.Left + currentBounds.Width() * 0.2525f, currentBounds.Top + currentBounds.Height() * 0.43f)));
			mStarHolders.Add(new StarHolder( 0.5f, new PointF(currentBounds.Left + currentBounds.Width() * 0.4075f, currentBounds.Top + currentBounds.Height() * 0.0934f)));
			mStarHolders.Add(new StarHolder( new PointF(currentBounds.Left + currentBounds.Width() * 0.825f, currentBounds.Top + currentBounds.Height() * 0.04f)));
			mStarHolders.Add(new StarHolder( new PointF(currentBounds.Left + currentBounds.Width() * 0.7075f, currentBounds.Top + currentBounds.Height() * 0.147f)));
			mStarHolders.Add(new StarHolder( new PointF(currentBounds.Left + currentBounds.Width() * 0.3475f, currentBounds.Top + currentBounds.Height() * 0.2567f)));
			mStarHolders.Add(new StarHolder( 0.6f, new PointF(currentBounds.Left + currentBounds.Width() * 0.5825f, currentBounds.Top + currentBounds.Height() * 0.277f)));
			mStarHolders.Add(new StarHolder( new PointF(currentBounds.Left + currentBounds.Width() * 0.84f, currentBounds.Top + currentBounds.Height() * 0.32f)));
			mStarHolders.Add(new StarHolder( new PointF(currentBounds.Left + currentBounds.Width() * 0.8f, currentBounds.Top + currentBounds.Height() / 0.502f)));
			mStarHolders.Add(new StarHolder( 0.6f, new PointF(currentBounds.Left + currentBounds.Width() * 0.7f, currentBounds.Top + currentBounds.Height() * 0.473f)));

			mMaxStarOffsets = currentBounds.Height();
		}

		private Path CreateMoonPath(float moonCenterX, float moonCenterY)
		{
			RectF moonRectF = new RectF(moonCenterX - mSun_MoonRadius, moonCenterY - mSun_MoonRadius, moonCenterX + mSun_MoonRadius, moonCenterY + mSun_MoonRadius);
			Path path = new Path();
			path.AddArc(moonRectF, -90, 180);
			path.QuadTo(moonCenterX + mSun_MoonRadius / 2.0f, moonCenterY, moonCenterX, moonCenterY - mSun_MoonRadius);
			return path;
		}

		private class StarHolder
		{
			public int mAlpha;
			public PointF mCurrentPoint;

			public readonly PointF mPoint;
			public readonly float mFlashOffset;
			public readonly Interpolator mInterpolator;

			public StarHolder(PointF point) : this( 1.0f, point)
			{
			}

			public StarHolder( float flashOffset, PointF mPoint)
			{
				this.mAlpha = MAX_ALPHA;
				this.mCurrentPoint = new PointF();
				this.mPoint = mPoint;
				this.mFlashOffset = flashOffset;
                this.mInterpolator = INTERPOLATORS[mRandom.Next(INTERPOLATORS.Length)];
            }
		}

	}

}