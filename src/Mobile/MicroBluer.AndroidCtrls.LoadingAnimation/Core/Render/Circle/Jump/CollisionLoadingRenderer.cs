namespace MicroBluer.AndroidCtrls.LoadingAnimation.Render.Circle.Jump
{
    using Context = Android.Content.Context;
    using Canvas = Android.Graphics.Canvas;
    using Color = Android.Graphics.Color;
    using ColorFilter = Android.Graphics.ColorFilter;
    using Paint = Android.Graphics.Paint;
    using LinearGradient = Android.Graphics.LinearGradient;
    using RectF = Android.Graphics.RectF;
    using Shader = Android.Graphics.Shader;
    using Interpolator = Android.Views.Animations.IInterpolator;
	using AccelerateInterpolator = Android.Views.Animations.AccelerateInterpolator;
	using DecelerateInterpolator = Android.Views.Animations.DecelerateInterpolator;
    using Android.Graphics;
    using System;
    using MicroBluer.AndroidUtils.Common;

    public class CollisionLoadingRenderer : LoadingRenderer
	{
		private static readonly Interpolator ACCELERATE_INTERPOLATOR = new AccelerateInterpolator();
		private static readonly Interpolator DECELERATE_INTERPOLATOR = new DecelerateInterpolator();

		private const int MAX_ALPHA = 255;
		private const int OVAL_ALPHA = 64;

		private const int DEFAULT_BALL_COUNT = 7;

		private const float DEFAULT_OVAL_HEIGHT = 1.5f;
		private const float DEFAULT_BALL_RADIUS = 7.5f;
		private static readonly float DEFAULT_WIDTH = 15.0f * 11;
		private static readonly float DEFAULT_HEIGHT = 15.0f * 4;

		private const float START_LEFT_DURATION_OFFSET = 0.25f;
		private const float START_RIGHT_DURATION_OFFSET = 0.5f;
		private const float END_RIGHT_DURATION_OFFSET = 0.75f;
		private const float END_LEFT_DURATION_OFFSET = 1.0f;

		private static readonly int[] DEFAULT_COLORS = new int[]{Color.ParseColor("#FF28435D"), Color.ParseColor("#FFC32720")};

		private static readonly float[] DEFAULT_POSITIONS = new float[]{0.0f, 1.0f};

        private readonly Paint mPaint = new Paint(PaintFlags.AntiAlias);
        private readonly RectF mOvalRect = new RectF();

		private int[] mColors;
		private float[] mPositions;

		private float mOvalVerticalRadius;

		private float mBallRadius;
		private float mBallCenterY;
		private float mBallSideOffsets;
		private float mBallMoveXOffsets;
		private float mBallQuadCoefficient;

		private float mLeftBallMoveXOffsets;
		private float mLeftBallMoveYOffsets;
		private float mRightBallMoveXOffsets;
		private float mRightBallMoveYOffsets;

		private float mLeftOvalShapeRate;
		private float mRightOvalShapeRate;

		private int mBallCount;

		internal CollisionLoadingRenderer(Context context) : base(context)
		{
			Init(context);
			AdjustParams();
			SetupPaint();
		}

		private void Init(Context context)
		{
			mBallRadius = DensityUtil.Dip2Px(context, DEFAULT_BALL_RADIUS);
			Width = DensityUtil.Dip2Px(context, DEFAULT_WIDTH);
			Height = DensityUtil.Dip2Px(context, DEFAULT_HEIGHT);
			mOvalVerticalRadius = DensityUtil.Dip2Px(context, DEFAULT_OVAL_HEIGHT);

			mColors = DEFAULT_COLORS;
			mPositions = DEFAULT_POSITIONS;
			mBallCount = DEFAULT_BALL_COUNT;

			//mBallMoveYOffsets = mBallQuadCoefficient * mBallMoveXOffsets ^ 2
			// ==> if mBallMoveYOffsets == mBallMoveXOffsets
			// ==> mBallQuadCoefficient = 1.0f / mBallMoveXOffsets;
			mBallMoveXOffsets = 1.5f * (2 * mBallRadius);
			mBallQuadCoefficient = 1.0f / mBallMoveXOffsets;
		}

		private void AdjustParams()
		{
			mBallCenterY = Height / 2.0f;
			mBallSideOffsets = (Width - mBallRadius * 2.0f * (mBallCount - 2)) / 2;
		}

		private void SetupPaint()
		{
			mPaint.SetStyle(Paint.Style.Fill);
            mPaint.SetShader(new LinearGradient(mBallSideOffsets, 0, Width - mBallSideOffsets, 0, mColors, mPositions, Shader.TileMode.Clamp));
		}

		protected  override void Draw(Canvas canvas, Rect bounds)
        {
			int saveCount = canvas.Save();

			for (int i = 1; i < mBallCount - 1; i++)
			{
				mPaint.Alpha = MAX_ALPHA;
				canvas.DrawCircle(mBallRadius * (i * 2 - 1) + mBallSideOffsets, mBallCenterY, mBallRadius, mPaint);

				mOvalRect.Set(mBallRadius * (i * 2 - 2) + mBallSideOffsets, Height - mOvalVerticalRadius * 2, mBallRadius * (i * 2) + mBallSideOffsets, Height);
				mPaint.Alpha = OVAL_ALPHA;
				canvas.DrawOval(mOvalRect, mPaint);
			}

			//draw the first ball
			mPaint.Alpha = MAX_ALPHA;
			canvas.DrawCircle(mBallSideOffsets - mBallRadius - mLeftBallMoveXOffsets, mBallCenterY - mLeftBallMoveYOffsets, mBallRadius, mPaint);

			mOvalRect.Set(mBallSideOffsets - mBallRadius - mBallRadius * mLeftOvalShapeRate - mLeftBallMoveXOffsets, Height - mOvalVerticalRadius - mOvalVerticalRadius * mLeftOvalShapeRate, mBallSideOffsets - mBallRadius + mBallRadius * mLeftOvalShapeRate - mLeftBallMoveXOffsets, Height - mOvalVerticalRadius + mOvalVerticalRadius * mLeftOvalShapeRate);
			mPaint.Alpha = OVAL_ALPHA;
			canvas.DrawOval(mOvalRect, mPaint);

			//draw the last ball
			mPaint.Alpha = MAX_ALPHA;
			canvas.DrawCircle(mBallRadius * (mBallCount * 2 - 3) + mBallSideOffsets + mRightBallMoveXOffsets, mBallCenterY - mRightBallMoveYOffsets, mBallRadius, mPaint);

			mOvalRect.Set(mBallRadius * (mBallCount * 2 - 3) - mBallRadius * mRightOvalShapeRate + mBallSideOffsets + mRightBallMoveXOffsets, Height - mOvalVerticalRadius - mOvalVerticalRadius * mRightOvalShapeRate, mBallRadius * (mBallCount * 2 - 3) + mBallRadius * mRightOvalShapeRate + mBallSideOffsets + mRightBallMoveXOffsets, Height - mOvalVerticalRadius + mOvalVerticalRadius * mRightOvalShapeRate);
			mPaint.Alpha = OVAL_ALPHA;
			canvas.DrawOval(mOvalRect, mPaint);

			canvas.RestoreToCount(saveCount);
		}

		protected internal override void ComputeRender(float renderProgress)
		{
			// Moving the left ball to the left sides only occurs in the first 25% of a jump animation
			if (renderProgress <= START_LEFT_DURATION_OFFSET)
			{
				float startLeftOffsetProgress = renderProgress / START_LEFT_DURATION_OFFSET;
				ComputeLeftBallMoveOffsets(DECELERATE_INTERPOLATOR.GetInterpolation(startLeftOffsetProgress));
				return;
			}

			// Moving the left ball to the origin location only occurs between 25% and 50% of a jump ring animation
			if (renderProgress <= START_RIGHT_DURATION_OFFSET)
			{
				float startRightOffsetProgress = (renderProgress - START_LEFT_DURATION_OFFSET) / (START_RIGHT_DURATION_OFFSET - START_LEFT_DURATION_OFFSET);
				ComputeLeftBallMoveOffsets(ACCELERATE_INTERPOLATOR.GetInterpolation(1.0f - startRightOffsetProgress));
				return;
			}

			// Moving the right ball to the right sides only occurs between 50% and 75% of a jump animation
			if (renderProgress <= END_RIGHT_DURATION_OFFSET)
			{
				float endRightOffsetProgress = (renderProgress - START_RIGHT_DURATION_OFFSET) / (END_RIGHT_DURATION_OFFSET - START_RIGHT_DURATION_OFFSET);
				ComputeRightBallMoveOffsets(DECELERATE_INTERPOLATOR.GetInterpolation(endRightOffsetProgress));
				return;
			}

			// Moving the right ball to the origin location only occurs after 75% of a jump animation
			if (renderProgress <= END_LEFT_DURATION_OFFSET)
			{
				float endRightOffsetProgress = (renderProgress - END_RIGHT_DURATION_OFFSET) / (END_LEFT_DURATION_OFFSET - END_RIGHT_DURATION_OFFSET);
				ComputeRightBallMoveOffsets(ACCELERATE_INTERPOLATOR.GetInterpolation(1 - endRightOffsetProgress));
				return;
			}

		}

		private void ComputeLeftBallMoveOffsets(float progress)
		{
			mRightBallMoveXOffsets = 0.0f;
			mRightBallMoveYOffsets = 0.0f;

			mLeftOvalShapeRate = 1.0f - progress;
			mLeftBallMoveXOffsets = mBallMoveXOffsets * progress;
			mLeftBallMoveYOffsets = (float)(Math.Pow(mLeftBallMoveXOffsets, 2) * mBallQuadCoefficient);
		}

		private void ComputeRightBallMoveOffsets(float progress)
		{
			mLeftBallMoveXOffsets = 0.0f;
			mLeftBallMoveYOffsets = 0.0f;

			mRightOvalShapeRate = 1.0f - progress;
			mRightBallMoveXOffsets = mBallMoveXOffsets * progress;
			mRightBallMoveYOffsets = (float)(Math.Pow(mRightBallMoveXOffsets, 2) * mBallQuadCoefficient);
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