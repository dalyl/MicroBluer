namespace LazyWelfare.AndroidMobile.Loading.Render.Circle.Rotate
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

    using FastOutSlowInInterpolator = Android.Support.V4.View.Animation.FastOutSlowInInterpolator;
    using DisplayMetrics = Android.Util.DisplayMetrics;
    using TypedValue = Android.Util.TypedValue;
    using Animator = Android.Animation.Animator;
	using AnimatorListenerAdapter = Android.Animation.AnimatorListenerAdapter;


	public class GearLoadingRenderer : LoadingRenderer
	{
		private static readonly Interpolator ACCELERATE_INTERPOLATOR = new AccelerateInterpolator();
		private static readonly Interpolator DECELERATE_INTERPOLATOR = new DecelerateInterpolator();

		private const int GEAR_COUNT = 4;
		private const int NUM_POINTS = 3;
		private const int MAX_ALPHA = 255;
		private const int DEGREE_360 = 360;

		private const int DEFAULT_GEAR_SWIPE_DEGREES = 60;

		private static readonly float FULL_GROUP_ROTATION = 3.0f * DEGREE_360;

		private const float START_SCALE_DURATION_OFFSET = 0.3f;
		private const float START_TRIM_DURATION_OFFSET = 0.5f;
		private const float END_TRIM_DURATION_OFFSET = 0.7f;
		private const float END_SCALE_DURATION_OFFSET = 1.0f;

		private const float DEFAULT_CENTER_RADIUS = 12.5f;
		private const float DEFAULT_STROKE_WIDTH = 2.5f;

		private static readonly int DEFAULT_COLOR = Color.White;

		private readonly Paint mPaint = new Paint();
		private readonly RectF mTempBounds = new RectF();

		private Animator.IAnimatorListener AnimatorListener { get; }

		private class AnimatorListenerAdapterAnonymousInnerClass : AnimatorListenerAdapter
		{

            GearLoadingRenderer OuterInstance { get; }
            public AnimatorListenerAdapterAnonymousInnerClass(GearLoadingRenderer master)
			{
                OuterInstance = master;

            }

			public override void OnAnimationRepeat(Animator animator)
			{
				base.OnAnimationRepeat(animator);
				OuterInstance.storeOriginals();

				OuterInstance.mStartDegrees = OuterInstance.mEndDegrees;
				OuterInstance.mRotationCount = (OuterInstance.mRotationCount + 1) % NUM_POINTS;
			}

			public override void OnAnimationStart(Animator animation)
			{
				base.OnAnimationStart(animation);
				OuterInstance.mRotationCount = 0;
			}
		}

		private int mColor;

		private int mGearCount;
		private int mGearSwipeDegrees;

		private float mStrokeInset;

		private float mRotationCount;
		private float mGroupRotation;

		private float mScale;
		private float mEndDegrees;
		private float mStartDegrees;
		private float mSwipeDegrees;
		private float mOriginEndDegrees;
		private float mOriginStartDegrees;

		private float mStrokeWidth;
		private float mCenterRadius;

		internal GearLoadingRenderer(Context context) : base(context)
		{

            AnimatorListener = new AnimatorListenerAdapterAnonymousInnerClass(this);
			Init(context);
            SetupPaint();
			AddRenderListener(AnimatorListener);

        }

		private void Init(Context context)
		{
			mStrokeWidth = DensityUtil.dip2px(context, DEFAULT_STROKE_WIDTH);
			mCenterRadius = DensityUtil.dip2px(context, DEFAULT_CENTER_RADIUS);

			mColor = DEFAULT_COLOR;

			mGearCount = GEAR_COUNT;
			mGearSwipeDegrees = DEFAULT_GEAR_SWIPE_DEGREES;
		}

		private void SetupPaint()
		{
			mPaint.AntiAlias = true;
			mPaint.StrokeWidth = mStrokeWidth;
			mPaint.SetStyle(Paint.Style.Stroke);
			mPaint.StrokeCap = Paint.Cap.Round;

			initStrokeInset(mWidth, mHeight);
		}

		protected internal override void Draw(Canvas canvas)
		{
			int saveCount = canvas.Save();

			mTempBounds.Set(mBounds);
			mTempBounds.Inset(mStrokeInset, mStrokeInset);
			mTempBounds.Inset(mTempBounds.Width() * (1.0f - mScale) / 2.0f, mTempBounds.Width() * (1.0f - mScale) / 2.0f);

			canvas.Rotate(mGroupRotation, mTempBounds.CenterX(), mTempBounds.CenterY());

			mPaint.Color = new Color (mColor);
			mPaint.Alpha = (int)(MAX_ALPHA * mScale);
			mPaint.StrokeWidth = mStrokeWidth * mScale;

			if (mSwipeDegrees != 0)
			{
				for (int i = 0; i < mGearCount; i++)
				{
					canvas.DrawArc(mTempBounds, mStartDegrees + DEGREE_360 / mGearCount * i, mSwipeDegrees, false, mPaint);
				}
			}

			canvas.RestoreToCount(saveCount);
		}

		protected internal override void ComputeRender(float renderProgress)
		{
			// Scaling up the start size only occurs in the first 20% of a single ring animation
			if (renderProgress <= START_SCALE_DURATION_OFFSET)
			{
				float startScaleProgress = (renderProgress) / START_SCALE_DURATION_OFFSET;
				mScale = DECELERATE_INTERPOLATOR.GetInterpolation(startScaleProgress);
			}

			// Moving the start trim only occurs between 20% to 50% of a single ring animation
			if (renderProgress <= START_TRIM_DURATION_OFFSET && renderProgress > START_SCALE_DURATION_OFFSET)
			{
				float startTrimProgress = (renderProgress - START_SCALE_DURATION_OFFSET) / (START_TRIM_DURATION_OFFSET - START_SCALE_DURATION_OFFSET);
				mStartDegrees = mOriginStartDegrees + mGearSwipeDegrees * startTrimProgress;
			}

			// Moving the end trim starts between 50% to 80% of a single ring animation
			if (renderProgress <= END_TRIM_DURATION_OFFSET && renderProgress > START_TRIM_DURATION_OFFSET)
			{
				float endTrimProgress = (renderProgress - START_TRIM_DURATION_OFFSET) / (END_TRIM_DURATION_OFFSET - START_TRIM_DURATION_OFFSET);
				mEndDegrees = mOriginEndDegrees + mGearSwipeDegrees * endTrimProgress;
			}

			// Scaling down the end size starts after 80% of a single ring animation
			if (renderProgress > END_TRIM_DURATION_OFFSET)
			{
				float endScaleProgress = (renderProgress - END_TRIM_DURATION_OFFSET) / (END_SCALE_DURATION_OFFSET - END_TRIM_DURATION_OFFSET);
				mScale = 1.0f - ACCELERATE_INTERPOLATOR.GetInterpolation(endScaleProgress);
			}

			if (renderProgress <= END_TRIM_DURATION_OFFSET && renderProgress > START_SCALE_DURATION_OFFSET)
			{
				float rotateProgress = (renderProgress - START_SCALE_DURATION_OFFSET) / (END_TRIM_DURATION_OFFSET - START_SCALE_DURATION_OFFSET);
				mGroupRotation = ((FULL_GROUP_ROTATION / NUM_POINTS) * rotateProgress) + (FULL_GROUP_ROTATION * (mRotationCount / NUM_POINTS));
			}

			if (Math.Abs(mEndDegrees - mStartDegrees) > 0)
			{
				mSwipeDegrees = mEndDegrees - mStartDegrees;
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
			resetOriginals();
		}

		private void initStrokeInset(float width, float height)
		{
			float minSize = Math.Min(width, height);
			float strokeInset = minSize / 2.0f - mCenterRadius;
			float minStrokeInset = (float) Math.Ceiling(mStrokeWidth / 2.0f);
			mStrokeInset = strokeInset < minStrokeInset ? minStrokeInset : strokeInset;
		}

		private void storeOriginals()
		{
			mOriginEndDegrees = mEndDegrees;
			mOriginStartDegrees = mEndDegrees;
		}

		private void resetOriginals()
		{
			mOriginEndDegrees = 0;
			mOriginStartDegrees = 0;

			mEndDegrees = 0;
			mStartDegrees = 0;

			mSwipeDegrees = 1;
		}

		private void apply(Builder builder)
		{
			this.mWidth = builder.mWidth > 0 ? builder.mWidth : this.mWidth;
			this.mHeight = builder.mHeight > 0 ? builder.mHeight : this.mHeight;
			this.mStrokeWidth = builder.mStrokeWidth > 0 ? builder.mStrokeWidth : this.mStrokeWidth;
			this.mCenterRadius = builder.mCenterRadius > 0 ? builder.mCenterRadius : this.mCenterRadius;

			this.mDuration = builder.mDuration > 0 ? builder.mDuration : this.mDuration;

			this.mColor = builder.mColor != 0 ? builder.mColor : this.mColor;

			this.mGearCount = builder.mGearCount > 0 ? builder.mGearCount : this.mGearCount;
			this.mGearSwipeDegrees = builder.mGearSwipeDegrees > 0 ? builder.mGearSwipeDegrees : this.mGearSwipeDegrees;

			SetupPaint();
			initStrokeInset(this.mWidth, this.mHeight);
		}

		public class Builder
		{
			internal Context mContext;

			internal int mWidth;
			internal int mHeight;
			internal int mStrokeWidth;
			internal int mCenterRadius;

			internal int mDuration;

			internal int mColor;

			internal int mGearCount;
			internal int mGearSwipeDegrees;

			public Builder(Context mContext)
			{
				this.mContext = mContext;
			}

			public virtual Builder setWidth(int width)
			{
				this.mWidth = width;
				return this;
			}

			public virtual Builder setHeight(int height)
			{
				this.mHeight = height;
				return this;
			}

			public virtual Builder setStrokeWidth(int strokeWidth)
			{
				this.mStrokeWidth = strokeWidth;
				return this;
			}

			public virtual Builder setCenterRadius(int centerRadius)
			{
				this.mCenterRadius = centerRadius;
				return this;
			}

			public virtual Builder setDuration(int duration)
			{
				this.mDuration = duration;
				return this;
			}

			public virtual Builder setColor(int color)
			{
				this.mColor = color;
				return this;
			}

			public virtual Builder setGearCount(int gearCount)
			{
				this.mGearCount = gearCount;
				return this;
			}

            //(from = 0, to = 360)
            public virtual Builder SetGearSwipeDegrees( int gearSwipeDegrees)
			{
                this.mGearSwipeDegrees = gearSwipeDegrees;
				return this;
			}

			public virtual GearLoadingRenderer Build()
			{
				GearLoadingRenderer loadingRenderer = new GearLoadingRenderer(mContext);
				loadingRenderer.apply(this);
				return loadingRenderer;
			}
		}
	}

}