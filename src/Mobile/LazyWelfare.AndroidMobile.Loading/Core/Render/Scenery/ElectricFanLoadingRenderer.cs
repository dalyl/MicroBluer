namespace LazyWelfare.AndroidMobile.Loading.Render.Scenery
{
    using Context = Android.Content.Context;
    using Canvas = Android.Graphics.Canvas;
    using Color = Android.Graphics.Color;
    using ColorFilter = Android.Graphics.ColorFilter;
    using Paint = Android.Graphics.Paint;
    using PointF = Android.Graphics.PointF;
    using LinearGradient = Android.Graphics.LinearGradient;
    using Path = Android.Graphics.Path;
    using Rect = Android.Graphics.Rect;
    using RectF = Android.Graphics.RectF;
    using Shader = Android.Graphics.Shader;
	using Drawable = Android.Graphics.Drawables.Drawable;
    using Interpolator = Android.Views.Animations.IInterpolator;
    using AccelerateInterpolator = Android.Views.Animations.AccelerateInterpolator;
    using DecelerateInterpolator = Android.Views.Animations.DecelerateInterpolator;
    using Android.Graphics;
    using System.Collections.Generic;
    using System;

    using FastOutLinearInInterpolator = Android.Support.V4.View.Animation.FastOutLinearInInterpolator;
    using FastOutSlowInInterpolator = Android.Support.V4.View.Animation.FastOutSlowInInterpolator;
    using DisplayMetrics = Android.Util.DisplayMetrics;
    using TypedValue = Android.Util.TypedValue;
    using Animator = Android.Animation.Animator;
    using AnimatorListenerAdapter = Android.Animation.AnimatorListenerAdapter;
    using LinearInterpolator = Android.Views.Animations.LinearInterpolator;
    using AnimatorSet = Android.Animation.AnimatorSet;
    using ITypeEvaluator = Android.Animation.ITypeEvaluator;
	using ValueAnimator = Android.Animation.ValueAnimator;


	public class ElectricFanLoadingRenderer : LoadingRenderer
	{
		private static readonly Interpolator LINEAR_INTERPOLATOR = new LinearInterpolator();
		private static readonly Interpolator MATERIAL_INTERPOLATOR = new FastOutSlowInInterpolator();
		private static readonly Interpolator DECELERATE_INTERPOLATOR = new DecelerateInterpolator();
		private static readonly Interpolator ACCELERATE_INTERPOLATOR = new AccelerateInterpolator();
		private static readonly Interpolator FASTOUTLINEARIN_INTERPOLATOR = new FastOutLinearInInterpolator();

		private static readonly Interpolator[] INTERPOLATORS = new Interpolator[]{LINEAR_INTERPOLATOR, DECELERATE_INTERPOLATOR, ACCELERATE_INTERPOLATOR, FASTOUTLINEARIN_INTERPOLATOR, MATERIAL_INTERPOLATOR};
		private static readonly IList<LeafHolder> mLeafHolders = new List<LeafHolder>();
		private static readonly Random mRandom = new Random();
		private  Animator.IAnimatorListener AnimatorListener { get; }

		private class AnimatorListenerAdapterAnonymousInnerClass : AnimatorListenerAdapter
		{
            ElectricFanLoadingRenderer outerInstance { get; }
            public AnimatorListenerAdapterAnonymousInnerClass(ElectricFanLoadingRenderer master)
			{
                outerInstance = master;
            }

            public override void OnAnimationRepeat(Animator animator)
			{
				base.OnAnimationRepeat(animator);
				outerInstance.Reset();
			}
		}

		public const int MODE_NORMAL = 0;
		public const int MODE_LEAF_COUNT = 1;

 

		private const string PERCENTAGE_100 = "100%";

		private const long ANIMATION_DURATION = 7333;

		private const int LEAF_COUNT = 28;
		private const int DEGREE_180 = 180;
		private const int DEGREE_360 = 360;
		private static readonly int FULL_GROUP_ROTATION = (int)(5.25f * DEGREE_360);

		private const int DEFAULT_PROGRESS_COLOR = unchecked((int)0xfffca72e);
		private const int DEFAULT_PROGRESS_BGCOLOR = unchecked((int)0xfffcd49f);
		private const int DEFAULT_ELECTRIC_FAN_BGCOLOR = unchecked((int)0xfffccc59);
		private static readonly int DEFAULT_ELECTRIC_FAN_OUTLINE_COLOR = Color.White;

		private const float DEFAULT_Width = 182.0f;
		private const float DEFAULT_Height = 65.0f;
		private const float DEFAULT_TEXT_SIZE = 11.0f;
		private const float DEFAULT_STROKE_Width = 2.0f;
		private const float DEFAULT_STROKE_INTERVAL = .2f;
		private const float DEFAULT_CENTER_RADIUS = 16.0f;
		private const float DEFAULT_PROGRESS_CENTER_RADIUS = 11.0f;

		private const float DEFAULT_LEAF_FLY_DURATION_FACTOR = 0.1f;

		private static readonly float LEAF_CREATE_DURATION_INTERVAL = 1.0f / LEAF_COUNT;
		private const float DECELERATE_DURATION_PERCENTAGE = 0.4f;
		private const float ACCELERATE_DURATION_PERCENTAGE = 0.6f;

		private readonly Paint mPaint = new Paint();
		private readonly RectF mTempBounds = new RectF();
		private readonly RectF mCurrentProgressBounds = new RectF();

		private float mTextSize;
		private float mStrokeXInset;
		private float mStrokeYInset;
		private float mProgressCenterRadius;

		private float mScale;
		private float mRotation;
		private float mProgress;

		private float mNextLeafCreateThreshold;

		private int mProgressColor;
		private int mProgressBgColor;
		private int mElectricFanBgColor;
		private int mElectricFanOutlineColor;

		private float mStrokeWidth;
		private float mCenterRadius;

		private int mMode;
		private int mCurrentLeafCount;

		private Drawable mLeafDrawable;
		private Drawable mLoadingDrawable;
		private Drawable mElectricFanDrawable;

        internal ElectricFanLoadingRenderer(Context context) : base(context)
		{
            AnimatorListener = new AnimatorListenerAdapterAnonymousInnerClass(this);
            Init(context);
			SetupPaint();
			AddRenderListener(AnimatorListener);
		}

		private void Init(Context context)
		{
			mMode = MODE_NORMAL;

			mWidth = DensityUtil.dip2px(context, DEFAULT_Width);
			mHeight = DensityUtil.dip2px(context, DEFAULT_Height);
			mTextSize = DensityUtil.dip2px(context, DEFAULT_TEXT_SIZE);
			mStrokeWidth = DensityUtil.dip2px(context, DEFAULT_STROKE_Width);
			mCenterRadius = DensityUtil.dip2px(context, DEFAULT_CENTER_RADIUS);
			mProgressCenterRadius = DensityUtil.dip2px(context, DEFAULT_PROGRESS_CENTER_RADIUS);

			mProgressColor = DEFAULT_PROGRESS_COLOR;
			mProgressBgColor = DEFAULT_PROGRESS_BGCOLOR;
			mElectricFanBgColor = DEFAULT_ELECTRIC_FAN_BGCOLOR;
			mElectricFanOutlineColor = DEFAULT_ELECTRIC_FAN_OUTLINE_COLOR;

			mLeafDrawable = context.Resources.GetDrawable(Resource.Drawable.ic_leaf);
            mLoadingDrawable = context.Resources.GetDrawable(Resource.Drawable.ic_loading);
			mElectricFanDrawable = context.Resources.GetDrawable(Resource.Drawable.ic_eletric_fan);

			mDuration = ANIMATION_DURATION;
			SetInsets((int) mWidth, (int) mHeight);
		}

		private void SetupPaint()
		{
			mPaint.AntiAlias = true;
			mPaint.StrokeWidth = mStrokeWidth;
			mPaint.SetStyle (Paint.Style.Stroke);
			mPaint.StrokeCap = Paint.Cap.Round;
		}

		protected internal override void Draw(Canvas canvas, Rect bounds)
		{
			int saveCount = canvas.Save();

			RectF arcBounds = mTempBounds;
			arcBounds.Set(bounds);
			arcBounds.Inset(mStrokeXInset, mStrokeYInset);

			mCurrentProgressBounds.Set(arcBounds.Left, arcBounds.Bottom - 2 * mCenterRadius, arcBounds.Right, arcBounds.Bottom);

			//Draw loading Drawable
			mLoadingDrawable.SetBounds((int) arcBounds.CenterX() - mLoadingDrawable.IntrinsicWidth / 2, 0, (int) arcBounds.CenterX() + mLoadingDrawable.IntrinsicWidth / 2, mLoadingDrawable.IntrinsicHeight);
			mLoadingDrawable.Draw(canvas);

			//Draw progress background
			float progressInset = mCenterRadius - mProgressCenterRadius;
			RectF progressRect = new RectF(mCurrentProgressBounds);
			//sub DEFAULT_STROKE_INTERVAL, otherwise will have a interval between progress background and progress outline
			progressRect.Inset(progressInset - DEFAULT_STROKE_INTERVAL, progressInset - DEFAULT_STROKE_INTERVAL);
			mPaint.Color = new Color (mProgressBgColor);
			mPaint.SetStyle(Paint.Style.Fill) ;
			canvas.DrawRoundRect(progressRect, mProgressCenterRadius, mProgressCenterRadius, mPaint);

			//Draw progress
			mPaint.Color = new Color (mProgressColor);
			mPaint.SetStyle(Paint.Style.Fill) ;
            canvas.DrawPath(CreateProgressPath(mProgress, mProgressCenterRadius, progressRect), mPaint);

			//Draw leaves
			for (int i = 0; i < mLeafHolders.Count; i++)
			{
				int leafSaveCount = canvas.Save();
				LeafHolder leafHolder = mLeafHolders[i];
				Rect leafBounds = leafHolder.mLeafRect;

				canvas.Rotate(leafHolder.mLeafRotation, leafBounds.CenterX(), leafBounds.CenterY());
				mLeafDrawable.Bounds = leafBounds;
				mLeafDrawable.Draw(canvas);

				canvas.RestoreToCount(leafSaveCount);
			}

			//Draw progress background outline,
			//after Drawing the leaves and then Draw the outline of the progress background can
			//prevent the leaves from flying to the outside
			RectF progressOutlineRect = new RectF(mCurrentProgressBounds);
			float progressOutlineStrokeInset = (mCenterRadius - mProgressCenterRadius) / 2.0f;
			progressOutlineRect.Inset(progressOutlineStrokeInset, progressOutlineStrokeInset);
			mPaint.SetStyle (Paint.Style.Stroke);
            mPaint.Color = new Color (mProgressBgColor);
			mPaint.StrokeWidth = mCenterRadius - mProgressCenterRadius;
			canvas.DrawRoundRect(progressOutlineRect, mCenterRadius, mCenterRadius, mPaint);

			//Draw electric fan outline
			float electricFanCenterX = arcBounds.Right - mCenterRadius;
			float electricFanCenterY = arcBounds.Bottom - mCenterRadius;

			mPaint.Color = new Color (mElectricFanOutlineColor);
			mPaint.SetStyle (Paint.Style.Stroke);
            mPaint.StrokeWidth = mStrokeWidth;
			canvas.DrawCircle(arcBounds.Right - mCenterRadius, arcBounds.Bottom - mCenterRadius, mCenterRadius - mStrokeWidth / 2.0f, mPaint);

			//Draw electric background
			mPaint.Color = new Color (mElectricFanBgColor);
			mPaint.SetStyle(Paint.Style.Fill) ;
            canvas.DrawCircle(arcBounds.Right - mCenterRadius, arcBounds.Bottom - mCenterRadius, mCenterRadius - mStrokeWidth + DEFAULT_STROKE_INTERVAL, mPaint);

			//Draw electric fan
			int rotateSaveCount = canvas.Save();
			canvas.Rotate(mRotation, electricFanCenterX, electricFanCenterY);
			mElectricFanDrawable.SetBounds((int)(electricFanCenterX - mElectricFanDrawable.IntrinsicWidth / 2 * mScale), (int)(electricFanCenterY - mElectricFanDrawable.IntrinsicHeight / 2 * mScale), (int)(electricFanCenterX + mElectricFanDrawable.IntrinsicWidth / 2 * mScale), (int)(electricFanCenterY + mElectricFanDrawable.IntrinsicHeight / 2 * mScale));
			mElectricFanDrawable.Draw(canvas);
			canvas.RestoreToCount(rotateSaveCount);

			//Draw 100% text
			if (mScale < 1.0f)
			{
				mPaint.TextSize = mTextSize * (1 - mScale);
				mPaint.Color = new Color (mElectricFanOutlineColor);
				Rect textRect = new Rect();
				mPaint.GetTextBounds(PERCENTAGE_100, 0, PERCENTAGE_100.Length, textRect);
				canvas.DrawText(PERCENTAGE_100, electricFanCenterX - textRect.Width() / 2.0f, electricFanCenterY + textRect.Height() / 2.0f, mPaint);
			}

			canvas.RestoreToCount(saveCount);
		}

		private Path CreateProgressPath(float progress, float circleRadius, RectF progressRect)
		{
			RectF arcProgressRect = new RectF(progressRect.Left, progressRect.Top, progressRect.Left + circleRadius * 2, progressRect.Bottom);
			RectF rectProgressRect = null;

			float progressWidth = progress * progressRect.Width();
			float progressModeWidth = mMode == MODE_LEAF_COUNT ? (float) mCurrentLeafCount / (float) LEAF_COUNT * progressRect.Width() : progress * progressRect.Width();

			float swipeAngle = DEGREE_180;
			//the Left half circle of the progressbar
			if (progressModeWidth < circleRadius)
			{
				swipeAngle = progressModeWidth / circleRadius * DEGREE_180;
			}

			//the center rect of the progressbar
			if (progressModeWidth < progressRect.Width() - circleRadius && progressModeWidth >= circleRadius)
			{
				rectProgressRect = new RectF(progressRect.Left + circleRadius, progressRect.Top, progressRect.Left + progressModeWidth, progressRect.Bottom);
			}

			//the Right half circle of the progressbar
			if (progressWidth >= progressRect.Width() - circleRadius)
			{
				rectProgressRect = new RectF(progressRect.Left + circleRadius, progressRect.Top, progressRect.Right - circleRadius, progressRect.Bottom);
				mScale = (progressRect.Width() - progressWidth) / circleRadius;
			}

			//the Left of the Right half circle
			if (progressWidth < progressRect.Width() - circleRadius)
			{
				mRotation = (progressWidth / (progressRect.Width() - circleRadius)) * FULL_GROUP_ROTATION % DEGREE_360;

				RectF leafRect = new RectF(progressRect.Left + progressWidth, progressRect.Top, progressRect.Right - circleRadius, progressRect.Bottom);
				AddLeaf(progress, leafRect);
			}

			Path path = new Path();
			path.AddArc(arcProgressRect, DEGREE_180 - swipeAngle / 2, swipeAngle);

			if (rectProgressRect != null)
			{
				path.AddRect(rectProgressRect, Path.Direction.Cw);
			}

			return path;
		}

		protected internal override void ComputeRender(float renderProgress)
		{
			if (renderProgress < DECELERATE_DURATION_PERCENTAGE)
			{
				mProgress = DECELERATE_INTERPOLATOR.GetInterpolation(renderProgress / DECELERATE_DURATION_PERCENTAGE) * DECELERATE_DURATION_PERCENTAGE;
			}
			else
			{
				mProgress = ACCELERATE_INTERPOLATOR.GetInterpolation((renderProgress - DECELERATE_DURATION_PERCENTAGE) / ACCELERATE_DURATION_PERCENTAGE) * ACCELERATE_DURATION_PERCENTAGE + DECELERATE_DURATION_PERCENTAGE;
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
				mPaint.SetColorFilter(value);
    
			}
		}

		protected internal override void Reset()
		{
			mScale = 1.0f;
			mCurrentLeafCount = 0;
			mNextLeafCreateThreshold = 0.0f;
			mLeafHolders.Clear();
		}

		protected internal virtual void SetInsets(int Width, int Height)
		{
			float minEdge = (float) Math.Min(Width, Height);
			float insetXs;
			if (mCenterRadius <= 0 || minEdge < 0)
			{
				insetXs = (float) Math.Ceiling(mCenterRadius / 2.0f);
			}
			else
			{
				insetXs = mCenterRadius;
			}
			mStrokeYInset = (float) Math.Ceiling(mCenterRadius / 2.0f);
			mStrokeXInset = insetXs;
		}

		private void AddLeaf(float progress, RectF leafFlyRect)
		{
			if (progress < mNextLeafCreateThreshold)
			{
				return;
			}
			mNextLeafCreateThreshold += LEAF_CREATE_DURATION_INTERVAL;

			LeafHolder leafHolder = new LeafHolder(this);
			mLeafHolders.Add(leafHolder);
			Animator leafAnimator = GetAnimator(leafHolder, leafFlyRect, progress);
			leafAnimator.AddListener(new AnimEndListener(this, leafHolder));
			leafAnimator.Start();
		}

		private Animator GetAnimator(LeafHolder target, RectF leafFlyRect, float progress)
		{
			ValueAnimator bezierValueAnimator = GetBezierValueAnimator(target, leafFlyRect, progress);

			AnimatorSet finalSet = new AnimatorSet();
			finalSet.PlaySequentially(bezierValueAnimator);
			finalSet.SetInterpolator(INTERPOLATORS[mRandom.Next(INTERPOLATORS.Length)]);
			finalSet.SetTarget (target);
			return finalSet;
		}

		private ValueAnimator GetBezierValueAnimator(LeafHolder target, RectF leafFlyRect, float progress)
		{
			BezierEvaluator evaluator = new BezierEvaluator(this, GetPoint1(leafFlyRect), GetPoint2(leafFlyRect));

			int leafFlyStartY = (int)(mCurrentProgressBounds.Bottom - mLeafDrawable.IntrinsicHeight);
			int leafFlyRange = (int)(mCurrentProgressBounds.Height() - mLeafDrawable.IntrinsicHeight);

			int startPointY = leafFlyStartY - mRandom.Next(leafFlyRange);
			int endPointY = leafFlyStartY - mRandom.Next(leafFlyRange);

			ValueAnimator animator = ValueAnimator.OfObject(evaluator, new PointF((int)(leafFlyRect.Right - mLeafDrawable.IntrinsicWidth), startPointY), new PointF(leafFlyRect.Left, endPointY));
			animator.AddUpdateListener(new BezierListener(this, target));
			animator.SetTarget (target);

			animator.SetDuration ((long)((mRandom.Next(300) + mDuration * DEFAULT_LEAF_FLY_DURATION_FACTOR) * (1.0f - progress)));

			return animator;
		}

		//get the pointF which belong to the Right half side
		private PointF GetPoint1(RectF leafFlyRect)
		{
			PointF point = new PointF();
			point.X = leafFlyRect.Right - mRandom.Next((int)(leafFlyRect.Width() / 2));
			point.Y = (int)(leafFlyRect.Bottom - mRandom.Next((int) leafFlyRect.Height()));
			return point;
		}

		//get the pointF which belong to the Left half side
		private PointF GetPoint2(RectF leafFlyRect)
		{
			PointF point = new PointF();
			point.X = leafFlyRect.Left + mRandom.Next((int)(leafFlyRect.Width() / 2));
			point.Y = (int)(leafFlyRect.Bottom - mRandom.Next((int) leafFlyRect.Height()));
			return point;
		}

		private class BezierEvaluator : Java.Lang.Object,ITypeEvaluator
		{
			private readonly ElectricFanLoadingRenderer outerInstance;

			internal PointF point1;
			internal PointF point2;

			public BezierEvaluator(ElectricFanLoadingRenderer outerInstance, PointF point1, PointF point2)
			{
				this.outerInstance = outerInstance;
				this.point1 = point1;
				this.point2 = point2;
			}

            public Java.Lang.Object Evaluate(float fraction, Java.Lang.Object startValue, Java.Lang.Object endValue)
            {
                PointF point0 = startValue as PointF;
                PointF point3 = endValue as PointF;
                float t = fraction;
                float tLeft = 1.0f - t;

                float x = (float)(point0.X * Math.Pow(tLeft, 3) + 3 * point1.X * t * Math.Pow(tLeft, 2) + 3 * point2.X * Math.Pow(t, 2) * tLeft + point3.X * Math.Pow(t, 3));
                float y = (float)(point0.Y * Math.Pow(tLeft, 3) + 3 * point1.Y * t * Math.Pow(tLeft, 2) + 3 * point2.Y * Math.Pow(t, 2) * tLeft + point3.Y * Math.Pow(t, 3));

                return new PointF(x, y);
            }
		}

		private class BezierListener : Java.Lang.Object,ValueAnimator.IAnimatorUpdateListener
		{
			private readonly ElectricFanLoadingRenderer outerInstance;


			internal LeafHolder target;

			public BezierListener(ElectricFanLoadingRenderer outerInstance, LeafHolder target)
			{
				this.outerInstance = outerInstance;
				this.target = target;
			}

			public  void OnAnimationUpdate(ValueAnimator animation)
			{
				PointF point = (PointF) animation.AnimatedValue;
				target.mLeafRect.Set((int) point.X, (int) point.Y, (int)(point.X + outerInstance.mLeafDrawable.IntrinsicWidth), (int)(point.Y + outerInstance.mLeafDrawable.IntrinsicHeight));
				target.mLeafRotation = target.mMaxRotation * animation.AnimatedFraction;
			}
		}

		private class AnimEndListener : AnimatorListenerAdapter
		{
			private readonly ElectricFanLoadingRenderer outerInstance;

			internal LeafHolder target;

			public AnimEndListener(ElectricFanLoadingRenderer outerInstance, LeafHolder target)
			{
				this.outerInstance = outerInstance;
				this.target = target;
			}

			public override void OnAnimationEnd(Animator animation)
			{
				base.OnAnimationEnd(animation);
				mLeafHolders.Remove(target);
				outerInstance.mCurrentLeafCount++;
			}
		}

		private class LeafHolder:Java.Lang.Object
		{
			private readonly ElectricFanLoadingRenderer outerInstance;

			public LeafHolder(ElectricFanLoadingRenderer outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public Rect mLeafRect = new Rect();
			public float mLeafRotation = 0.0f;

			public float mMaxRotation = mRandom.Next(120);
		}

		public class Builder
		{
			internal Context mContext;

			public Builder(Context mContext)
			{
				this.mContext = mContext;
			}

			public virtual ElectricFanLoadingRenderer Build()
			{
				ElectricFanLoadingRenderer loadingRenderer = new ElectricFanLoadingRenderer(mContext);
				return loadingRenderer;
			}
		}
	}

}