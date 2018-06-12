namespace LazyWelfare.AndroidCtrls.LoadingAnimation.Render.Circle.Rotate
{

    using Context = Android.Content.Context;
    using Canvas = Android.Graphics.Canvas;
    using Color = Android.Graphics.Color;
    using ColorFilter = Android.Graphics.ColorFilter;
    using Paint = Android.Graphics.Paint;
    using RectF = Android.Graphics.RectF;
    using Interpolator = Android.Views.Animations.IInterpolator;
    using AccelerateInterpolator = Android.Views.Animations.AccelerateInterpolator;
    using DecelerateInterpolator = Android.Views.Animations.DecelerateInterpolator;
    using System;

    using FastOutSlowInInterpolator = Android.Support.V4.View.Animation.FastOutSlowInInterpolator;
    using Animator = Android.Animation.Animator;
    using AnimatorListenerAdapter = Android.Animation.AnimatorListenerAdapter;
	using LinearInterpolator = Android.Views.Animations.LinearInterpolator;
    using LazyWelfare.AndroidUtils.Common;
    using LazyWelfare.AndroidUtils.Animation;
    using Android.Graphics;

    public class LevelLoadingRenderer : LoadingRenderer
	{
		private static readonly Interpolator LINEAR_INTERPOLATOR = new LinearInterpolator();
		private static readonly Interpolator MATERIAL_INTERPOLATOR = new FastOutSlowInInterpolator();
		private static readonly Interpolator ACCELERATE_INTERPOLATOR = new AccelerateInterpolator();
		private static readonly Interpolator DECELERATE_INTERPOLATOR = new DecelerateInterpolator();

		private const int NUM_POINTS = 5;
		private const int DEGREE_360 = 360;

		private static readonly float MAX_SWIPE_DEGREES = 0.8f * DEGREE_360;
		private static readonly float FULL_GROUP_ROTATION = 3.0f * DEGREE_360;

		private static readonly float[] LEVEL_SWEEP_ANGLE_OFFSETS = new float[]{1.0f, 7.0f / 8.0f, 5.0f / 8.0f};

		private const float START_TRIM_DURATION_OFFSET = 0.5f;
		private const float END_TRIM_DURATION_OFFSET = 1.0f;

		private const float DEFAULT_CENTER_RADIUS = 12.5f;
		private const float DEFAULT_STROKE_WIDTH = 2.5f;

		private static readonly int[] DEFAULT_LEVEL_COLORS = new int[]{Color.ParseColor("#55ffffff"), Color.ParseColor("#b1ffffff"), Color.ParseColor("#ffffffff")};

		private readonly Paint mPaint = new Paint();
		private readonly RectF mTempBounds = new RectF();

		private Animator.IAnimatorListener AnimatorListener { get; }

		private int[] mLevelColors;
		private float[] mLevelSwipeDegrees;

		private float mStrokeInset;

		private float mRotationCount;
		private float mGroupRotation;

		private float mEndDegrees;
		private float mStartDegrees;
		private float mOriginEndDegrees;
		private float mOriginStartDegrees;

		private float mStrokeWidth;
		private float mCenterRadius;

		internal LevelLoadingRenderer(Context context) : base(context)
		{
            AnimatorListener = new AnonymousAnimatorListenerAdapter
            {
                Start = amor =>
                {
                    this.mRotationCount = 0;
                },
                Repeat = amor =>
                {
                    this.StoreOriginals();
                    this.mStartDegrees = this.mEndDegrees;
                    this.mRotationCount = (this.mRotationCount + 1) % (NUM_POINTS);
                },
            };
            Init(context);
			SetupPaint();
			AddRenderListener(AnimatorListener);
		}

		private void Init(Context context)
		{
			mStrokeWidth = DensityUtil.Dip2Px(context, DEFAULT_STROKE_WIDTH);
			mCenterRadius = DensityUtil.Dip2Px(context, DEFAULT_CENTER_RADIUS);

			mLevelSwipeDegrees = new float[3];
			mLevelColors = DEFAULT_LEVEL_COLORS;
		}

		private void SetupPaint()
		{

			mPaint.AntiAlias = true;
			mPaint.StrokeWidth = mStrokeWidth;
			mPaint.SetStyle(Paint.Style.Stroke);
			mPaint.StrokeCap = Paint.Cap.Round;

			InitStrokeInset((int) Width, (int) Height);
		}

		protected  override void Draw(Canvas canvas, Rect bounds)
        {
			int saveCount = canvas.Save();

			mTempBounds.Set(Bounds);
			mTempBounds.Inset(mStrokeInset, mStrokeInset);
			canvas.Rotate(mGroupRotation, mTempBounds.CenterX(), mTempBounds.CenterY());

			for (int i = 0; i < 3; i++)
			{
				if (mLevelSwipeDegrees[i] != 0)
				{
					mPaint.Color = new Color (mLevelColors[i]);
					canvas.DrawArc(mTempBounds, mEndDegrees, mLevelSwipeDegrees[i], false, mPaint);
				}
			}

			canvas.RestoreToCount(saveCount);
		}

		protected internal override void ComputeRender(float renderProgress)
		{
			// Moving the start trim only occurs in the first 50% of a single ring animation
			if (renderProgress <= START_TRIM_DURATION_OFFSET)
			{
				float startTrimProgress = (renderProgress) / START_TRIM_DURATION_OFFSET;
				mStartDegrees = mOriginStartDegrees + MAX_SWIPE_DEGREES * MATERIAL_INTERPOLATOR.GetInterpolation(startTrimProgress);

				float mSwipeDegrees = mEndDegrees - mStartDegrees;
				float levelSwipeDegreesProgress = Math.Abs(mSwipeDegrees) / MAX_SWIPE_DEGREES;

				float level1Increment = DECELERATE_INTERPOLATOR.GetInterpolation(levelSwipeDegreesProgress) - LINEAR_INTERPOLATOR.GetInterpolation(levelSwipeDegreesProgress);
				float level3Increment = ACCELERATE_INTERPOLATOR.GetInterpolation(levelSwipeDegreesProgress) - LINEAR_INTERPOLATOR.GetInterpolation(levelSwipeDegreesProgress);

				mLevelSwipeDegrees[0] = -mSwipeDegrees * LEVEL_SWEEP_ANGLE_OFFSETS[0] * (1.0f + level1Increment);
				mLevelSwipeDegrees[1] = -mSwipeDegrees * LEVEL_SWEEP_ANGLE_OFFSETS[1] * 1.0f;
				mLevelSwipeDegrees[2] = -mSwipeDegrees * LEVEL_SWEEP_ANGLE_OFFSETS[2] * (1.0f + level3Increment);
			}

			// Moving the end trim starts after 50% of a single ring animation
			if (renderProgress > START_TRIM_DURATION_OFFSET)
			{
				float endTrimProgress = (renderProgress - START_TRIM_DURATION_OFFSET) / (END_TRIM_DURATION_OFFSET - START_TRIM_DURATION_OFFSET);
				mEndDegrees = mOriginEndDegrees + MAX_SWIPE_DEGREES * MATERIAL_INTERPOLATOR.GetInterpolation(endTrimProgress);

				float mSwipeDegrees = mEndDegrees - mStartDegrees;
				float levelSwipeDegreesProgress = Math.Abs(mSwipeDegrees) / MAX_SWIPE_DEGREES;

				if (levelSwipeDegreesProgress > LEVEL_SWEEP_ANGLE_OFFSETS[1])
				{
					mLevelSwipeDegrees[0] = -mSwipeDegrees;
					mLevelSwipeDegrees[1] = MAX_SWIPE_DEGREES * LEVEL_SWEEP_ANGLE_OFFSETS[1];
					mLevelSwipeDegrees[2] = MAX_SWIPE_DEGREES * LEVEL_SWEEP_ANGLE_OFFSETS[2];
				}
				else if (levelSwipeDegreesProgress > LEVEL_SWEEP_ANGLE_OFFSETS[2])
				{
					mLevelSwipeDegrees[0] = 0;
					mLevelSwipeDegrees[1] = -mSwipeDegrees;
					mLevelSwipeDegrees[2] = MAX_SWIPE_DEGREES * LEVEL_SWEEP_ANGLE_OFFSETS[2];
				}
				else
				{
					mLevelSwipeDegrees[0] = 0;
					mLevelSwipeDegrees[1] = 0;
					mLevelSwipeDegrees[2] = -mSwipeDegrees;
				}
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
				mPaint.SetColorFilter (value);
			}
		}

		protected internal override void Reset()
		{
			ResetOriginals();
		}

		private void InitStrokeInset(float width, float height)
		{
			float minSize = Math.Min(width, height);
			float strokeInset = minSize / 2.0f - mCenterRadius;
			float minStrokeInset = (float) Math.Ceiling(mStrokeWidth / 2.0f);
			mStrokeInset = strokeInset < minStrokeInset ? minStrokeInset : strokeInset;
		}

		private void StoreOriginals()
		{
			mOriginEndDegrees = mEndDegrees;
			mOriginStartDegrees = mEndDegrees;
		}

		private void ResetOriginals()
		{
			mOriginEndDegrees = 0;
			mOriginStartDegrees = 0;

			mEndDegrees = 0;
			mStartDegrees = 0;

			mLevelSwipeDegrees[0] = 0;
			mLevelSwipeDegrees[1] = 0;
			mLevelSwipeDegrees[2] = 0;
		}

	}

}