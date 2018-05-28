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
    using LinearInterpolator = Android.Views.Animations.LinearInterpolator;


	public class WhorlLoadingRenderer : LoadingRenderer
	{
		private static readonly Interpolator MATERIAL_INTERPOLATOR = new FastOutSlowInInterpolator();

		private const int DEGREE_180 = 180;
		private const int DEGREE_360 = 360;
		private const int NUM_POINTS = 5;

		private static readonly float MAX_SWIPE_DEGREES = 0.6f * DEGREE_360;
		private static readonly float FULL_GROUP_ROTATION = 3.0f * DEGREE_360;

		private const float START_TRIM_DURATION_OFFSet = 0.5f;
		private const float END_TRIM_DURATION_OFFSet = 1.0f;

		private const float DEFAULT_CENTER_RADIUS = 12.5f;
		private const float DEFAULT_STROKE_WIDTH = 2.5f;

		private static readonly int[] DEFAULT_COLORS = new int[]{Color.Red, Color.Green, Color.Blue};

		private readonly Paint mPaint = new Paint();
		private readonly RectF mTempBounds = new RectF();
		private readonly RectF mTempArcBounds = new RectF();

		private Animator.IAnimatorListener AnimatorListener { get; }

		private class AnimatorListenerAdapterAnonymousInnerClass : AnimatorListenerAdapter
		{
            WhorlLoadingRenderer OuterInstance { get; }
            public AnimatorListenerAdapterAnonymousInnerClass(WhorlLoadingRenderer master)
			{
                OuterInstance = master;
            }

			public override void OnAnimationRepeat(Animator animator)
			{
				base.OnAnimationRepeat(animator);
				OuterInstance.storeOriginals();

				OuterInstance.mStartDegrees = OuterInstance.mEndDegrees;
				OuterInstance.mRotationCount = (OuterInstance.mRotationCount + 1) % (NUM_POINTS);
			}

			public override void OnAnimationStart(Animator animation)
			{
				base.OnAnimationStart(animation);
				OuterInstance.mRotationCount = 0;
			}
		}

		private int[] mColors;

		private float mStrokeInSet;

		private float mRotationCount;
		private float mGroupRotation;

		private float mEndDegrees;
		private float mStartDegrees;
		private float mSwipeDegrees;
		private float mOriginEndDegrees;
		private float mOriginStartDegrees;

		private float mStrokeWidth;
		private float mCenterRadius;

		internal WhorlLoadingRenderer(Context context) : base(context)
		{
            AnimatorListener = new AnimatorListenerAdapterAnonymousInnerClass(this);
            Init(context);
			SetupPaint();
			AddRenderListener(AnimatorListener);
		}

		private void Init(Context context)
		{
			mColors = DEFAULT_COLORS;
			mStrokeWidth = DensityUtil.dip2px(context, DEFAULT_STROKE_WIDTH);
			mCenterRadius = DensityUtil.dip2px(context, DEFAULT_CENTER_RADIUS);

			initStrokeInSet(mWidth, mHeight);
		}

		private void SetupPaint()
		{
			mPaint.AntiAlias = true;
			mPaint.StrokeWidth = mStrokeWidth;
			mPaint.SetStyle(Paint.Style.Stroke);
			mPaint.StrokeCap = Paint.Cap.Round;
		}

		protected internal override void Draw(Canvas canvas)
		{
			int saveCount = canvas.Save();

			mTempBounds.Set(mBounds);
			mTempBounds.Inset(mStrokeInSet, mStrokeInSet);

			canvas.Rotate(mGroupRotation, mTempBounds.CenterX(), mTempBounds.CenterY());

			if (mSwipeDegrees != 0)
			{
				for (int i = 0; i < mColors.Length; i++)
				{
					mPaint.StrokeWidth = mStrokeWidth / (i + 1);
					mPaint.Color =new Color(mColors[i]);
					canvas.DrawArc(createArcBounds(mTempBounds, i), mStartDegrees + DEGREE_180 * (i % 2), mSwipeDegrees, false, mPaint);
				}
			}

			canvas.RestoreToCount(saveCount);
		}

		private RectF createArcBounds(RectF sourceArcBounds, int index)
		{
			int intervalWidth = 0;

			for (int i = 0; i < index; i++)
			{
				intervalWidth += (int)(mStrokeWidth / (i + 1.0f) * 1.5f);
			}

			int arcBoundsLeft = (int)(sourceArcBounds.Left + intervalWidth);
			int arcBoundsTop = (int)(sourceArcBounds.Top + intervalWidth);
			int arcBoundsRight = (int)(sourceArcBounds.Right - intervalWidth);
			int arcBoundsBottom = (int)(sourceArcBounds.Bottom - intervalWidth);
			mTempArcBounds.Set(arcBoundsLeft, arcBoundsTop, arcBoundsRight, arcBoundsBottom);

			return mTempArcBounds;
		}

		protected internal override void ComputeRender(float renderProgress)
		{
			// Moving the start trim only occurs in the first 50% of a single ring animation
			if (renderProgress <= START_TRIM_DURATION_OFFSet)
			{
				float startTrimProgress = (renderProgress) / (1.0f - START_TRIM_DURATION_OFFSet);
				mStartDegrees = mOriginStartDegrees + MAX_SWIPE_DEGREES * MATERIAL_INTERPOLATOR.GetInterpolation(startTrimProgress);
			}

			// Moving the end trim starts after 50% of a single ring animation
			if (renderProgress > START_TRIM_DURATION_OFFSet)
			{
				float endTrimProgress = (renderProgress - START_TRIM_DURATION_OFFSet) / (END_TRIM_DURATION_OFFSet - START_TRIM_DURATION_OFFSet);
				mEndDegrees = mOriginEndDegrees + MAX_SWIPE_DEGREES * MATERIAL_INTERPOLATOR.GetInterpolation(endTrimProgress);
			}

			if (Math.Abs(mEndDegrees - mStartDegrees) > 0)
			{
				mSwipeDegrees = mEndDegrees - mStartDegrees;
			}

			mGroupRotation = ((FULL_GROUP_ROTATION / NUM_POINTS) * renderProgress) + (FULL_GROUP_ROTATION * (mRotationCount / NUM_POINTS));
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
				mPaint.SetColorFilter(value);
    
			}
		}

		protected internal override void Reset()
		{
			reSetOriginals();
		}

		private void initStrokeInSet(float width, float height)
		{
			float minSize = Math.Min(width, height);
			float strokeInSet = minSize / 2.0f - mCenterRadius;
			float minStrokeInSet = (float) Math.Ceiling(mStrokeWidth / 2.0f);
			mStrokeInSet = strokeInSet < minStrokeInSet ? minStrokeInSet : strokeInSet;
		}

		private void storeOriginals()
		{
			mOriginEndDegrees = mEndDegrees;
			mOriginStartDegrees = mEndDegrees;
		}

		private void reSetOriginals()
		{
			mOriginEndDegrees = 0;
			mOriginStartDegrees = 0;

			mEndDegrees = 0;
			mStartDegrees = 0;

			mSwipeDegrees = 0;
		}

		private void apply(Builder builder)
		{
			this.mWidth = builder.mWidth > 0 ? builder.mWidth : this.mWidth;
			this.mHeight = builder.mHeight > 0 ? builder.mHeight : this.mHeight;
			this.mStrokeWidth = builder.mStrokeWidth > 0 ? builder.mStrokeWidth : this.mStrokeWidth;
			this.mCenterRadius = builder.mCenterRadius > 0 ? builder.mCenterRadius : this.mCenterRadius;

			this.mDuration = builder.mDuration > 0 ? builder.mDuration : this.mDuration;

			this.mColors = builder.mColors != null && builder.mColors.Length > 0 ? builder.mColors : this.mColors;

			SetupPaint();
			initStrokeInSet(this.mWidth, this.mHeight);
		}

		public class Builder
		{
			internal Context mContext;

			internal int mWidth;
			internal int mHeight;
			internal int mStrokeWidth;
			internal int mCenterRadius;

			internal int mDuration;

			internal int[] mColors;

			public Builder(Context mContext)
			{
				this.mContext = mContext;
			}

			public virtual Builder SetWidth(int width)
			{
				this.mWidth = width;
				return this;
			}

			public virtual Builder SetHeight(int height)
			{
				this.mHeight = height;
				return this;
			}

			public virtual Builder SetStrokeWidth(int strokeWidth)
			{
				this.mStrokeWidth = strokeWidth;
				return this;
			}

			public virtual Builder SetCenterRadius(int centerRadius)
			{
				this.mCenterRadius = centerRadius;
				return this;
			}

			public virtual Builder SetDuration(int duration)
			{
				this.mDuration = duration;
				return this;
			}

			public virtual Builder SetColors(int[] colors)
			{
				this.mColors = colors;
				return this;
			}

			public virtual WhorlLoadingRenderer build()
			{
				WhorlLoadingRenderer loadingRenderer = new WhorlLoadingRenderer(mContext);
				loadingRenderer.apply(this);
				return loadingRenderer;
			}
		}
	}

}