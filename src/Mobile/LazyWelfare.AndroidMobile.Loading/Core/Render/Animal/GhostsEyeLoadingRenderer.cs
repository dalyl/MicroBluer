namespace LazyWelfare.AndroidMobile.Loading.Render.Animal
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


	public class GhostsEyeLoadingRenderer : LoadingRenderer
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			EYE_BALL_INTERPOLATOR = new EyeBallInterpolator(this);
			EYE_CIRCLE_INTERPOLATOR = new EyeCircleInterpolator(this);
		}

		private Interpolator EYE_BALL_INTERPOLATOR;
		private Interpolator EYE_CIRCLE_INTERPOLATOR;

		private const float DEFAULT_WIDTH = 200.0f;
		private const float DEFAULT_HEIGHT = 176.0f;
		private const float DEFAULT_EYE_EDGE_WIDTH = 5.0f;

		private const float DEFAULT_EYE_BALL_HEIGHT = 9.0f;
		private const float DEFAULT_EYE_BALL_WIDTH = 11.0f;

		private const float DEFAULT_EYE_CIRCLE_INTERVAL = 8.0f;
		private const float DEFAULT_EYE_BALL_OFFSET_Y = 2.0f;
		private const float DEFAULT_ABOVE_RADIAN_EYE_CIRCLE_OFFSET = 6.0f;
		private const float DEFAULT_EYE_CIRCLE_RADIUS = 21.0f;
		private const float DEFAULT_MAX_EYE_JUMP_DISTANCE = 11.0f;

		private const float LEFT_EYE_CIRCLE_BALL_START_JUMP_UP_OFFSET = 0.0f;
		private const float RIGHT_EYE_CIRCLE_BALL_START_JUMP_UP_OFFSET = 0.067f;

		private const float LEFT_EYE_BALL_END_JUMP_OFFSET = 0.4f;
		private const float LEFT_EYE_CIRCLE_END_JUMP_OFFSET = 0.533f;
		private const float RIGHT_EYE_BALL_END_JUMP_OFFSET = 0.467f;
		private const float RIGHT_EYE_CIRCLE_END_JUMP_OFFSET = 0.60f;

		private const int DEGREE_180 = 180;

		private const long ANIMATION_DURATION = 2333;

		private static readonly int DEFAULT_COLOR = Color.ParseColor("#ff484852");

		private readonly Paint mPaint = new Paint();
		private readonly RectF mTempBounds = new RectF();

		private float mEyeInterval;
		private float mEyeCircleRadius;
		private float mMaxEyeJumptDistance;
		private float mAboveRadianEyeOffsetX;
		private float mEyeBallOffsetY;

		private float mEyeEdgeWidth;
		private float mEyeBallWidth;
		private float mEyeBallHeight;

		private float mLeftEyeCircleOffsetY;
		private float mRightEyeCircleOffsetY;
		private float mLeftEyeBallOffsetY;
		private float mRightEyeBallOffsetY;

		private int mColor;

        internal GhostsEyeLoadingRenderer(Context context) : base(context)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			init(context);
			setupPaint();
		}

		private void init(Context context)
		{
			mWidth = DensityUtil.dip2px(context, DEFAULT_WIDTH);
			mHeight = DensityUtil.dip2px(context, DEFAULT_HEIGHT);
			mEyeEdgeWidth = DensityUtil.dip2px(context, DEFAULT_EYE_EDGE_WIDTH);

			mEyeInterval = DensityUtil.dip2px(context, DEFAULT_EYE_CIRCLE_INTERVAL);
			mEyeBallOffsetY = DensityUtil.dip2px(context, DEFAULT_EYE_BALL_OFFSET_Y);
			mEyeCircleRadius = DensityUtil.dip2px(context, DEFAULT_EYE_CIRCLE_RADIUS);
			mMaxEyeJumptDistance = DensityUtil.dip2px(context, DEFAULT_MAX_EYE_JUMP_DISTANCE);
			mAboveRadianEyeOffsetX = DensityUtil.dip2px(context, DEFAULT_ABOVE_RADIAN_EYE_CIRCLE_OFFSET);

			mEyeBallWidth = DensityUtil.dip2px(context, DEFAULT_EYE_BALL_WIDTH);
			mEyeBallHeight = DensityUtil.dip2px(context, DEFAULT_EYE_BALL_HEIGHT);

			mColor = DEFAULT_COLOR;

			mDuration = ANIMATION_DURATION;
		}

		private void setupPaint()
		{
			mPaint.AntiAlias = true;
			mPaint.StrokeWidth = mEyeEdgeWidth;
			mPaint.StrokeJoin = Paint.Join.Round;
			mPaint.SetStyle(Paint.Style.Stroke);
			mPaint.StrokeCap = Paint.Cap.Round;
		}

		protected internal override void Draw(Canvas canvas, Rect bounds)
		{
			int saveCount = canvas.Save();
			RectF arcBounds = mTempBounds;
			arcBounds.Set(bounds);

			mPaint.Color = new Color (mColor);

			mPaint.SetStyle (Paint.Style.Stroke);
			canvas.DrawPath(createLeftEyeCircle(arcBounds, mLeftEyeCircleOffsetY), mPaint);
			canvas.DrawPath(createRightEyeCircle(arcBounds, mRightEyeCircleOffsetY), mPaint);

			mPaint.SetStyle(Paint.Style.Fill);
			//create left eye ball
			canvas.DrawOval(createLeftEyeBall(arcBounds, mLeftEyeBallOffsetY), mPaint);
			//create right eye ball
			canvas.DrawOval(createRightEyeBall(arcBounds, mRightEyeBallOffsetY), mPaint);

			canvas.RestoreToCount(saveCount);
		}

		protected internal override void ComputeRender(float renderProgress)
		{
			if (renderProgress <= LEFT_EYE_BALL_END_JUMP_OFFSET && renderProgress >= LEFT_EYE_CIRCLE_BALL_START_JUMP_UP_OFFSET)
			{
				float eyeCircle_BallJumpUpProgress = (renderProgress - LEFT_EYE_CIRCLE_BALL_START_JUMP_UP_OFFSET) / (LEFT_EYE_BALL_END_JUMP_OFFSET - LEFT_EYE_CIRCLE_BALL_START_JUMP_UP_OFFSET);
				mLeftEyeBallOffsetY = -mMaxEyeJumptDistance * EYE_BALL_INTERPOLATOR.GetInterpolation(eyeCircle_BallJumpUpProgress);
			}

			if (renderProgress <= LEFT_EYE_CIRCLE_END_JUMP_OFFSET && renderProgress >= LEFT_EYE_CIRCLE_BALL_START_JUMP_UP_OFFSET)
			{
				float eyeCircle_BallJumpUpProgress = (renderProgress - LEFT_EYE_CIRCLE_BALL_START_JUMP_UP_OFFSET) / (LEFT_EYE_CIRCLE_END_JUMP_OFFSET - LEFT_EYE_CIRCLE_BALL_START_JUMP_UP_OFFSET);
				mLeftEyeCircleOffsetY = -mMaxEyeJumptDistance * EYE_CIRCLE_INTERPOLATOR.GetInterpolation(eyeCircle_BallJumpUpProgress);
			}

			if (renderProgress <= RIGHT_EYE_BALL_END_JUMP_OFFSET && renderProgress >= RIGHT_EYE_CIRCLE_BALL_START_JUMP_UP_OFFSET)
			{
				float eyeCircle_BallJumpUpProgress = (renderProgress - RIGHT_EYE_CIRCLE_BALL_START_JUMP_UP_OFFSET) / (RIGHT_EYE_BALL_END_JUMP_OFFSET - RIGHT_EYE_CIRCLE_BALL_START_JUMP_UP_OFFSET);
				mRightEyeBallOffsetY = -mMaxEyeJumptDistance * EYE_BALL_INTERPOLATOR.GetInterpolation(eyeCircle_BallJumpUpProgress);
			}

			if (renderProgress <= RIGHT_EYE_CIRCLE_END_JUMP_OFFSET && renderProgress >= RIGHT_EYE_CIRCLE_BALL_START_JUMP_UP_OFFSET)
			{
				float eyeCircle_BallJumpUpProgress = (renderProgress - RIGHT_EYE_CIRCLE_BALL_START_JUMP_UP_OFFSET) / (RIGHT_EYE_CIRCLE_END_JUMP_OFFSET - RIGHT_EYE_CIRCLE_BALL_START_JUMP_UP_OFFSET);
				mRightEyeCircleOffsetY = -mMaxEyeJumptDistance * EYE_CIRCLE_INTERPOLATOR.GetInterpolation(eyeCircle_BallJumpUpProgress);
			}
		}

		protected internal override int Alpha
		{
			set
			{
    
			}
		}

		protected internal override ColorFilter ColorFilter
		{
			set
			{
    
			}
		}

		protected internal override void Reset()
		{
			mLeftEyeBallOffsetY = 0.0f;
			mRightEyeBallOffsetY = 0.0f;
			mLeftEyeCircleOffsetY = 0.0f;
			mRightEyeCircleOffsetY = 0.0f;
		}

		private RectF createLeftEyeBall(RectF arcBounds, float offsetY)
		{
			//the center of the left eye
			float leftEyeCenterX = arcBounds.CenterX() - mEyeInterval / 2.0f - mEyeCircleRadius;
			float leftEyeCenterY = arcBounds.CenterY() - mEyeBallOffsetY + offsetY;

			RectF rectF = new RectF(leftEyeCenterX - mEyeBallWidth / 2.0f, leftEyeCenterY - mEyeBallHeight / 2.0f, leftEyeCenterX + mEyeBallWidth / 2.0f, leftEyeCenterY + mEyeBallHeight / 2.0f);

			return rectF;
		}

		private RectF createRightEyeBall(RectF arcBounds, float offsetY)
		{
			//the center of the right eye
			float rightEyeCenterX = arcBounds.CenterX() + mEyeInterval / 2.0f + mEyeCircleRadius;
			float rightEyeCenterY = arcBounds.CenterY() - mEyeBallOffsetY + offsetY;

			RectF rectF = new RectF(rightEyeCenterX - mEyeBallWidth / 2.0f, rightEyeCenterY - mEyeBallHeight / 2.0f, rightEyeCenterX + mEyeBallWidth / 2.0f, rightEyeCenterY + mEyeBallHeight / 2.0f);

			return rectF;
		}


		private Path createLeftEyeCircle(RectF arcBounds, float offsetY)
		{
			Path path = new Path();

			//the center of the left eye
			float leftEyeCenterX = arcBounds.CenterX() - mEyeInterval / 2.0f - mEyeCircleRadius;
			float leftEyeCenterY = arcBounds.CenterY() + offsetY;
			//the bounds of left eye
			RectF leftEyeBounds = new RectF(leftEyeCenterX - mEyeCircleRadius, leftEyeCenterY - mEyeCircleRadius, leftEyeCenterX + mEyeCircleRadius, leftEyeCenterY + mEyeCircleRadius);
			path.AddArc(leftEyeBounds, 0, DEGREE_180 + 15);
			//the above radian of of the eye
			path.QuadTo(leftEyeBounds.Left + mAboveRadianEyeOffsetX, leftEyeBounds.Top + mEyeCircleRadius * 0.2f, leftEyeBounds.Left + mAboveRadianEyeOffsetX / 4.0f, leftEyeBounds.Top - mEyeCircleRadius * 0.15f);

			return path;
		}

		private Path createRightEyeCircle(RectF arcBounds, float offsetY)
		{
			Path path = new Path();

			//the center of the right eye
			float rightEyeCenterX = arcBounds.CenterX() + mEyeInterval / 2.0f + mEyeCircleRadius;
			float rightEyeCenterY = arcBounds.CenterY() + offsetY;
			//the bounds of left eye
			RectF leftEyeBounds = new RectF(rightEyeCenterX - mEyeCircleRadius, rightEyeCenterY - mEyeCircleRadius, rightEyeCenterX + mEyeCircleRadius, rightEyeCenterY + mEyeCircleRadius);
			path.AddArc(leftEyeBounds, 180, -(DEGREE_180 + 15));
			//the above radian of of the eye
			path.QuadTo(leftEyeBounds.Right - mAboveRadianEyeOffsetX, leftEyeBounds.Top + mEyeCircleRadius * 0.2f, leftEyeBounds.Right - mAboveRadianEyeOffsetX / 4.0f, leftEyeBounds.Top - mEyeCircleRadius * 0.15f);

			return path;
		}

		private class EyeCircleInterpolator : Java.Lang.Object,Interpolator
		{
			private readonly GhostsEyeLoadingRenderer outerInstance;

			public EyeCircleInterpolator(GhostsEyeLoadingRenderer outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public  float GetInterpolation(float input)
			{
				if (input < 0.25f)
				{
					return input * 4.0f;
				}
				else if (input < 0.5f)
				{
					return 1.0f - (input - 0.25f) * 4.0f;
				}
				else if (input < 0.75f)
				{
					return (input - 0.5f) * 2.0f;
				}
				else
				{
					return 0.5f - (input - 0.75f) * 2.0f;
				}

			}
		}

		private class EyeBallInterpolator : Java.Lang.Object, Interpolator
		{
			private readonly GhostsEyeLoadingRenderer outerInstance;

			public EyeBallInterpolator(GhostsEyeLoadingRenderer outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public  float GetInterpolation(float input)
			{
				if (input < 0.333333f)
				{
					return input * 3.0f;
				}
				else
				{
					return 1.0f - (input - 0.333333f) * 1.5f;
				}
			}
		}

		public class Builder
		{
			internal Context mContext;

			public Builder(Context mContext)
			{
				this.mContext = mContext;
			}

			public virtual GhostsEyeLoadingRenderer build()
			{
				GhostsEyeLoadingRenderer loadingRenderer = new GhostsEyeLoadingRenderer(mContext);
				return loadingRenderer;
			}
		}
	}

}