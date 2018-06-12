namespace LazyWelfare.AndroidCtrls.LoadingAnimation.Render.Circle.Rotate
{
    using Context = Android.Content.Context;
    using Canvas = Android.Graphics.Canvas;
    using Color = Android.Graphics.Color;
    using ColorFilter = Android.Graphics.ColorFilter;
    using Paint = Android.Graphics.Paint;
    using RectF = Android.Graphics.RectF;
    using Interpolator = Android.Views.Animations.IInterpolator;
    using FastOutSlowInInterpolator = Android.Support.V4.View.Animation.FastOutSlowInInterpolator;
    using Animator = Android.Animation.Animator;
    using AnimatorListenerAdapter = Android.Animation.AnimatorListenerAdapter;
    using LazyWelfare.AndroidUtils.Common;
    using System;
    using LazyWelfare.AndroidUtils.Animation;
    using Android.Graphics;

    public class MaterialLoadingRenderer : LoadingRenderer
	{
		private static readonly Interpolator MATERIAL_INTERPOLATOR = new FastOutSlowInInterpolator();

		private const int DEGREE_360 = 360;
		private const int NUM_POINTS = 5;

		private static readonly float MAX_SWIPE_DEGREES = 0.8f * DEGREE_360;
		private static readonly float FULL_GROUP_ROTATION = 3.0f * DEGREE_360;

		private const float COLOR_START_DELAY_OFFSET = 0.8f;
		private const float END_TRIM_DURATION_OFFSET = 1.0f;
		private const float START_TRIM_DURATION_OFFSET = 0.5f;

		private const float DEFAULT_CENTER_RADIUS = 12.5f;
		private const float DEFAULT_STROKE_WIDTH = 2.5f;

		private static readonly int[] DEFAULT_COLORS = new int[]{Color.Red, Color.Green, Color.Blue};

		private readonly Paint mPaint = new Paint();
		private readonly RectF mTempBounds = new RectF();

		private  Animator.IAnimatorListener AnimatorListener { get; }

		private int[] mColors;
		private int mColorIndex;
		private int mCurrentColor;

		private float mStrokeInset;

		private float mRotationCount;
		private float mGroupRotation;

		private float mEndDegrees;
		private float mStartDegrees;
		private float mSwipeDegrees;
		private float mOriginEndDegrees;
		private float mOriginStartDegrees;

		private float mStrokeWidth;
		private float mCenterRadius;

		internal MaterialLoadingRenderer(Context context) : base(context)
		{
            AnimatorListener = new AnonymousAnimatorListenerAdapter {
                Start = amor => this.mRotationCount = 0,
                Repeat = amor => {
                    this.StoreOriginals();
                    this.GoToNextColor();
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

			mColors = DEFAULT_COLORS;

			ColorIndex = 0;
			InitStrokeInset(Width, Height);
		}

		private void SetupPaint()
		{
			mPaint.AntiAlias = true;
			mPaint.StrokeWidth = mStrokeWidth;
			mPaint.SetStyle(Paint.Style.Stroke);
			mPaint.StrokeCap = Paint.Cap.Round;
		}

		protected  override void Draw(Canvas canvas, Rect bounds)
        {
			int saveCount = canvas.Save();

			mTempBounds.Set(Bounds);
			mTempBounds.Inset(mStrokeInset, mStrokeInset);

			canvas.Rotate(mGroupRotation, mTempBounds.CenterX(), mTempBounds.CenterY());

			if (mSwipeDegrees != 0)
			{
				mPaint.Color = new Color (mCurrentColor);
				canvas.DrawArc(mTempBounds, mStartDegrees, mSwipeDegrees, false, mPaint);
			}

			canvas.RestoreToCount(saveCount);
		}

		protected internal override void ComputeRender(float renderProgress)
		{
			UpdateRingColor(renderProgress);

			// Moving the start trim only occurs in the first 50% of a single ring animation
			if (renderProgress <= START_TRIM_DURATION_OFFSET)
			{
				float startTrimProgress = renderProgress / START_TRIM_DURATION_OFFSET;
				mStartDegrees = mOriginStartDegrees + MAX_SWIPE_DEGREES * MATERIAL_INTERPOLATOR.GetInterpolation(startTrimProgress);
			}

			// Moving the end trim starts after 50% of a single ring animation completes
			if (renderProgress > START_TRIM_DURATION_OFFSET)
			{
				float endTrimProgress = (renderProgress - START_TRIM_DURATION_OFFSET) / (END_TRIM_DURATION_OFFSET - START_TRIM_DURATION_OFFSET);
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
			ResetOriginals();
		}

		private int ColorIndex
		{
			set
			{
				mColorIndex = value;
				mCurrentColor = mColors[mColorIndex];
			}
		}

		private int NextColor
		{
			get
			{
				return mColors[NextColorIndex];
			}
		}

		private int NextColorIndex
		{
			get
			{
				return (mColorIndex + 1) % (mColors.Length);
			}
		}

		private void GoToNextColor()
		{
			ColorIndex = NextColorIndex;
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
		}

		private int StartingColor
		{
			get
			{
				return mColors[mColorIndex];
			}
		}

		private void UpdateRingColor(float interpolatedTime)
		{
			if (interpolatedTime > COLOR_START_DELAY_OFFSET)
			{
				mCurrentColor = EvaluateColorChange((interpolatedTime - COLOR_START_DELAY_OFFSET) / (1.0f - COLOR_START_DELAY_OFFSET), StartingColor, NextColor);
			}
		}

		private int EvaluateColorChange(float fraction, int startValue, int endValue)
		{
			int startA = (startValue >> 24) & 0xff;
			int startR = (startValue >> 16) & 0xff;
			int startG = (startValue >> 8) & 0xff;
			int startB = startValue & 0xff;

			int endA = (endValue >> 24) & 0xff;
			int endR = (endValue >> 16) & 0xff;
			int endG = (endValue >> 8) & 0xff;
			int endB = endValue & 0xff;

			return ((startA + (int)(fraction * (endA - startA))) << 24) | ((startR + (int)(fraction * (endR - startR))) << 16) | ((startG + (int)(fraction * (endG - startG))) << 8) | ((startB + (int)(fraction * (endB - startB))));
		}

	}

}