namespace LazyWelfare.AndroidCtrls.LoadingAnimation.Render.Circle.Jump
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
    using Android.Graphics;
    using System;

	using FastOutSlowInInterpolator = Android.Support.V4.View.Animation.FastOutSlowInInterpolator;
    using LazyWelfare.AndroidUtils.Common;

    public class DanceLoadingRenderer : LoadingRenderer
	{
		private static readonly Interpolator MATERIAL_INTERPOLATOR = new FastOutSlowInInterpolator();
		private static readonly Interpolator ACCELERATE_INTERPOLATOR = new AccelerateInterpolator();
		private static readonly Interpolator DECELERATE_INTERPOLATOR = new DecelerateInterpolator();

		private const long ANIMATION_DURATION = 1888;

		private const float DEFAULT_CENTER_RADIUS = 12.5f;
		private const float DEFAULT_STROKE_WIDTH = 1.5f;
		private const float DEFAULT_DANCE_BALL_RADIUS = 2.0f;

		private const int NUM_POINTS = 3;
		private const int DEGREE_360 = 360;
		private const int RING_START_ANGLE = -90;
		private const int DANCE_START_ANGLE = 0;
		private const int DANCE_INTERVAL_ANGLE = 60;

		private static readonly int DEFAULT_COLOR = Android.Graphics.Color.White;

		//the center coordinate of the oval
		private static readonly float[] POINT_X = new float[NUM_POINTS];
		private static readonly float[] POINT_Y = new float[NUM_POINTS];
		//1: the coordinate x from small to large; -1: the coordinate x from large to small
		private static readonly int[] DIRECTION = new int[]{1, 1, -1};

		private const float BALL_FORWARD_START_ENTER_DURATION_OFFSET = 0f;
		private const float BALL_FORWARD_END_ENTER_DURATION_OFFSET = 0.125f;

		private const float RING_FORWARD_START_ROTATE_DURATION_OFFSET = 0.125f;
		private const float RING_FORWARD_END_ROTATE_DURATION_OFFSET = 0.375f;

		private const float CENTER_CIRCLE_FORWARD_START_SCALE_DURATION_OFFSET = 0.225f;
		private const float CENTER_CIRCLE_FORWARD_END_SCALE_DURATION_OFFSET = 0.475f;

		private const float BALL_FORWARD_START_EXIT_DURATION_OFFSET = 0.375f;
		private const float BALL_FORWARD_END_EXIT_DURATION_OFFSET = 0.54f;

		private const float RING_REVERSAL_START_ROTATE_DURATION_OFFSET = 0.5f;
		private const float RING_REVERSAL_END_ROTATE_DURATION_OFFSET = 0.75f;

		private const float BALL_REVERSAL_START_ENTER_DURATION_OFFSET = 0.6f;
		private const float BALL_REVERSAL_END_ENTER_DURATION_OFFSET = 0.725f;

		private const float CENTER_CIRCLE_REVERSAL_START_SCALE_DURATION_OFFSET = 0.675f;
		private const float CENTER_CIRCLE_REVERSAL_END_SCALE_DURATION_OFFSET = 0.875f;

		private const float BALL_REVERSAL_START_EXIT_DURATION_OFFSET = 0.875f;
		private const float BALL_REVERSAL_END_EXIT_DURATION_OFFSET = 1.0f;

		private readonly Paint mPaint = new Paint();
		private readonly RectF mTempBounds = new RectF();
		private readonly RectF mCurrentBounds = new RectF();

		private float mScale;
		private float mRotation;
		private float mStrokeInset;

		private float mCenterRadius;
		private float mStrokeWidth;
		private float mDanceBallRadius;
		private float mShapeChangeWidth;
		private float mShapeChangeHeight;

		private int mColor;
		private int mArcColor;

		internal DanceLoadingRenderer(Context context) : base(context)
		{
			Init(context);
			SetupPaint();
		}

		private void Init(Context context)
		{
			mStrokeWidth = DensityUtil.Dip2Px(context, DEFAULT_STROKE_WIDTH);
			mCenterRadius = DensityUtil.Dip2Px(context, DEFAULT_CENTER_RADIUS);
			mDanceBallRadius = DensityUtil.Dip2Px(context, DEFAULT_DANCE_BALL_RADIUS);

			Color = DEFAULT_COLOR;
			SetInsets((int) Width, (int) Height);
			Duration = ANIMATION_DURATION;
		}

		private void SetupPaint()
		{
			mPaint.AntiAlias = true;
			mPaint.StrokeWidth = mStrokeWidth;
			mPaint.SetStyle (Paint.Style.Stroke);
		}

		protected  override void Draw(Canvas canvas, Rect bounds)
		{
			int saveCount = canvas.Save();

			mTempBounds.Set(bounds);
			mTempBounds.Inset(mStrokeInset, mStrokeInset);
			mCurrentBounds.Set(mTempBounds);

			float outerCircleRadius = Math.Min(mTempBounds.Height(), mTempBounds.Width()) / 2.0f;
			float interCircleRadius = outerCircleRadius / 2.0f;
			float centerRingWidth = interCircleRadius - mStrokeWidth / 2;

			mPaint.SetStyle(Paint.Style.Stroke);
			mPaint.Color = new Color (mColor);
			mPaint.StrokeWidth = mStrokeWidth;
			canvas.DrawCircle(mTempBounds.CenterX(), mTempBounds.CenterY(), outerCircleRadius, mPaint);
			mPaint.SetStyle(Paint.Style.Fill);
			canvas.DrawCircle(mTempBounds.CenterX(), mTempBounds.CenterY(), interCircleRadius * mScale, mPaint);

			if (mRotation != 0)
			{
				mPaint.Color = new Color (mArcColor);
			    mPaint.SetStyle(Paint.Style.Stroke);
                //strokeWidth / 2.0f + mStrokeWidth / 2.0f is the center of the inter circle width
                mTempBounds.Inset(centerRingWidth / 2.0f + mStrokeWidth / 2.0f, centerRingWidth / 2.0f + mStrokeWidth / 2.0f);
				mPaint.StrokeWidth = centerRingWidth;
				canvas.DrawArc(mTempBounds, RING_START_ANGLE, mRotation, false, mPaint);
			}

			mPaint.Color = new Color (mColor);
			mPaint.SetStyle(Paint.Style.Fill);
            for (int i = 0; i < NUM_POINTS; i++)
			{
				canvas.Rotate(i * DANCE_INTERVAL_ANGLE, POINT_X[i], POINT_Y[i]);
				RectF rectF = new RectF(POINT_X[i] - mDanceBallRadius - mShapeChangeWidth / 2.0f, POINT_Y[i] - mDanceBallRadius - mShapeChangeHeight / 2.0f, POINT_X[i] + mDanceBallRadius + mShapeChangeWidth / 2.0f, POINT_Y[i] + mDanceBallRadius + mShapeChangeHeight / 2.0f);
				canvas.DrawOval(rectF, mPaint);
				canvas.Rotate(-i * DANCE_INTERVAL_ANGLE, POINT_X[i], POINT_Y[i]);
			}

			canvas.RestoreToCount(saveCount);
		}

		protected internal override void ComputeRender(float renderProgress)
		{
			float radius = Math.Min(mCurrentBounds.Height(), mCurrentBounds.Width()) / 2.0f;
			//the origin coordinate is the centerLeft of the field mCurrentBounds
			float originCoordinateX = mCurrentBounds.Left;
			float originCoordinateY = mCurrentBounds.Top + radius;

			if (renderProgress <= BALL_FORWARD_END_ENTER_DURATION_OFFSET && renderProgress > BALL_FORWARD_START_ENTER_DURATION_OFFSET)
			{
				float ballForwardEnterProgress = (renderProgress - BALL_FORWARD_START_ENTER_DURATION_OFFSET) / (BALL_FORWARD_END_ENTER_DURATION_OFFSET - BALL_FORWARD_START_ENTER_DURATION_OFFSET);

				mShapeChangeHeight = (0.5f - ballForwardEnterProgress) * mDanceBallRadius / 2.0f;
				mShapeChangeWidth = -mShapeChangeHeight;
				//y = k(x - r)--> k = tan(angle)
				//(x - r)^2 + y^2 = r^2
				// compute crossover point --> (k(x -r)) ^ 2 + (x - )^2 = r^2
				// so x --> [r + r / sqrt(k ^ 2 + 1), r - r / sqrt(k ^ 2 + 1)]
				for (int i = 0; i < NUM_POINTS; i++)
				{
					float k = (float) Math.Tan((DANCE_START_ANGLE + DANCE_INTERVAL_ANGLE * i) / 360.0f * (2.0f * Math.PI));
					// progress[-1, 1]
					float progress = (ACCELERATE_INTERPOLATOR.GetInterpolation(ballForwardEnterProgress) / 2.0f - 0.5f) * 2.0f * DIRECTION[i];
					POINT_X[i] = (float)(radius + progress * (radius / Math.Sqrt(Math.Pow(k, 2.0f) + 1.0f)));
					POINT_Y[i] = k * (POINT_X[i] - radius);

					POINT_X[i] += originCoordinateX;
					POINT_Y[i] += originCoordinateY;
				}
			}

			if (renderProgress <= RING_FORWARD_END_ROTATE_DURATION_OFFSET && renderProgress > RING_FORWARD_START_ROTATE_DURATION_OFFSET)
			{
				float forwardRotateProgress = (renderProgress - RING_FORWARD_START_ROTATE_DURATION_OFFSET) / (RING_FORWARD_END_ROTATE_DURATION_OFFSET - RING_FORWARD_START_ROTATE_DURATION_OFFSET);
				mRotation = DEGREE_360 * MATERIAL_INTERPOLATOR.GetInterpolation(forwardRotateProgress);
			}

			if (renderProgress <= CENTER_CIRCLE_FORWARD_END_SCALE_DURATION_OFFSET && renderProgress > CENTER_CIRCLE_FORWARD_START_SCALE_DURATION_OFFSET)
			{
				float centerCircleScaleProgress = (renderProgress - CENTER_CIRCLE_FORWARD_START_SCALE_DURATION_OFFSET) / (CENTER_CIRCLE_FORWARD_END_SCALE_DURATION_OFFSET - CENTER_CIRCLE_FORWARD_START_SCALE_DURATION_OFFSET);

				if (centerCircleScaleProgress <= 0.5f)
				{
					mScale = 1.0f + DECELERATE_INTERPOLATOR.GetInterpolation(centerCircleScaleProgress * 2.0f) * 0.2f;
				}
				else
				{
					mScale = 1.2f - ACCELERATE_INTERPOLATOR.GetInterpolation((centerCircleScaleProgress - 0.5f) * 2.0f) * 0.2f;
				}

			}

			if (renderProgress <= BALL_FORWARD_END_EXIT_DURATION_OFFSET && renderProgress > BALL_FORWARD_START_EXIT_DURATION_OFFSET)
			{
				float ballForwardExitProgress = (renderProgress - BALL_FORWARD_START_EXIT_DURATION_OFFSET) / (BALL_FORWARD_END_EXIT_DURATION_OFFSET - BALL_FORWARD_START_EXIT_DURATION_OFFSET);
				mShapeChangeHeight = (ballForwardExitProgress - 0.5f) * mDanceBallRadius / 2.0f;
				mShapeChangeWidth = -mShapeChangeHeight;
				for (int i = 0; i < NUM_POINTS; i++)
				{
					float k = (float) Math.Tan((DANCE_START_ANGLE + DANCE_INTERVAL_ANGLE * i) / 360.0f * (2.0f * Math.PI));
					float progress = (DECELERATE_INTERPOLATOR.GetInterpolation(ballForwardExitProgress) / 2.0f) * 2.0f * DIRECTION[i];
					POINT_X[i] = (float)(radius + progress * (radius / Math.Sqrt(Math.Pow(k, 2.0f) + 1.0f)));
					POINT_Y[i] = k * (POINT_X[i] - radius);

					POINT_X[i] += originCoordinateX;
					POINT_Y[i] += originCoordinateY;
				}
			}

			if (renderProgress <= RING_REVERSAL_END_ROTATE_DURATION_OFFSET && renderProgress > RING_REVERSAL_START_ROTATE_DURATION_OFFSET)
			{
				float scaledTime = (renderProgress - RING_REVERSAL_START_ROTATE_DURATION_OFFSET) / (RING_REVERSAL_END_ROTATE_DURATION_OFFSET - RING_REVERSAL_START_ROTATE_DURATION_OFFSET);
				mRotation = DEGREE_360 * MATERIAL_INTERPOLATOR.GetInterpolation(scaledTime) - 360;
			}
			else if (renderProgress > RING_REVERSAL_END_ROTATE_DURATION_OFFSET)
			{
				mRotation = 0.0f;
			}

			if (renderProgress <= BALL_REVERSAL_END_ENTER_DURATION_OFFSET && renderProgress > BALL_REVERSAL_START_ENTER_DURATION_OFFSET)
			{
				float ballReversalEnterProgress = (renderProgress - BALL_REVERSAL_START_ENTER_DURATION_OFFSET) / (BALL_REVERSAL_END_ENTER_DURATION_OFFSET - BALL_REVERSAL_START_ENTER_DURATION_OFFSET);
				mShapeChangeHeight = (0.5f - ballReversalEnterProgress) * mDanceBallRadius / 2.0f;
				mShapeChangeWidth = -mShapeChangeHeight;

				for (int i = 0; i < NUM_POINTS; i++)
				{
					float k = (float) Math.Tan((DANCE_START_ANGLE + DANCE_INTERVAL_ANGLE * i) / 360.0f * (2.0f * Math.PI));
					float progress = (0.5f - ACCELERATE_INTERPOLATOR.GetInterpolation(ballReversalEnterProgress) / 2.0f) * 2.0f * DIRECTION[i];
					POINT_X[i] = (float)(radius + progress * (radius / Math.Sqrt(Math.Pow(k, 2.0f) + 1.0f)));
					POINT_Y[i] = k * (POINT_X[i] - radius);

					POINT_X[i] += originCoordinateX;
					POINT_Y[i] += originCoordinateY;
				}
			}

			if (renderProgress <= CENTER_CIRCLE_REVERSAL_END_SCALE_DURATION_OFFSET && renderProgress > CENTER_CIRCLE_REVERSAL_START_SCALE_DURATION_OFFSET)
			{
				float centerCircleScaleProgress = (renderProgress - CENTER_CIRCLE_REVERSAL_START_SCALE_DURATION_OFFSET) / (CENTER_CIRCLE_REVERSAL_END_SCALE_DURATION_OFFSET - CENTER_CIRCLE_REVERSAL_START_SCALE_DURATION_OFFSET);

				if (centerCircleScaleProgress <= 0.5f)
				{
					mScale = 1.0f + DECELERATE_INTERPOLATOR.GetInterpolation(centerCircleScaleProgress * 2.0f) * 0.2f;
				}
				else
				{
					mScale = 1.2f - ACCELERATE_INTERPOLATOR.GetInterpolation((centerCircleScaleProgress - 0.5f) * 2.0f) * 0.2f;
				}

			}

			if (renderProgress <= BALL_REVERSAL_END_EXIT_DURATION_OFFSET && renderProgress > BALL_REVERSAL_START_EXIT_DURATION_OFFSET)
			{
				float ballReversalExitProgress = (renderProgress - BALL_REVERSAL_START_EXIT_DURATION_OFFSET) / (BALL_REVERSAL_END_EXIT_DURATION_OFFSET - BALL_REVERSAL_START_EXIT_DURATION_OFFSET);
				mShapeChangeHeight = (ballReversalExitProgress - 0.5f) * mDanceBallRadius / 2.0f;
				mShapeChangeWidth = -mShapeChangeHeight;

				for (int i = 0; i < NUM_POINTS; i++)
				{
					float k = (float) Math.Tan((DANCE_START_ANGLE + DANCE_INTERVAL_ANGLE * i) / 360.0f * (2.0f * Math.PI));
					float progress = (0.0f - DECELERATE_INTERPOLATOR.GetInterpolation(ballReversalExitProgress) / 2.0f) * 2.0f * DIRECTION[i];
					POINT_X[i] = (float)(radius + progress * (radius / Math.Sqrt(Math.Pow(k, 2.0f) + 1.0f)));
					POINT_Y[i] = k * (POINT_X[i] - radius);

					POINT_X[i] += originCoordinateX;
					POINT_Y[i] += originCoordinateY;
				}
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

        protected internal override void Reset()
		{
			mScale = 1.0f;
			mRotation = 0;
		}

		private int Color
		{
			set
			{
				mColor = value;
				mArcColor = HalfAlphaColor(mColor);
			}
		}

		private float Rotation { get; set; }

		private float DanceBallRadius { get; set; }
		

		private void SetInsets(int width, int height)
		{
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

		private int HalfAlphaColor(int colorValue)
		{
			int startA = (colorValue >> 24) & 0xff;
			int startR = (colorValue >> 16) & 0xff;
			int startG = (colorValue >> 8) & 0xff;
			int startB = colorValue & 0xff;

			return ((startA / 2) << 24) | (startR << 16) | (startG << 8) | startB;
		}

	}

}