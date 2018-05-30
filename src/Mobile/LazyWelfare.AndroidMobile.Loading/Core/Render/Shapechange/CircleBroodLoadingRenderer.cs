namespace LazyWelfare.AndroidMobile.Loading.Render.Shapechange
{
    using Context = Android.Content.Context;
    using Canvas = Android.Graphics.Canvas;
    using Color = Android.Graphics.Color;
    using ColorFilter = Android.Graphics.ColorFilter;
    using Paint = Android.Graphics.Paint;
    using Path = Android.Graphics.Path;
    using Rect = Android.Graphics.Rect;
    using RectF = Android.Graphics.RectF;
    using Interpolator = Android.Views.Animations.IInterpolator;
    using AccelerateInterpolator = Android.Views.Animations.AccelerateInterpolator;
    using DecelerateInterpolator = Android.Views.Animations.DecelerateInterpolator;
    using LazyWelfare.AndroidUtils.Common;
    using Android.Graphics;
    using System;


    public class CircleBroodLoadingRenderer : LoadingRenderer
	{
		private bool InstanceFieldsInitialized { get; } = false;

		private void InitializeInstanceFields()
		{
			MOTHER_MOVE_INTERPOLATOR = new MotherMoveInterpolator(this);
			CHILD_MOVE_INTERPOLATOR = new ChildMoveInterpolator(this);
		}


		private Interpolator MOTHER_MOVE_INTERPOLATOR;
		private Interpolator CHILD_MOVE_INTERPOLATOR;

		private readonly Interpolator ACCELERATE_INTERPOLATOR03 = new AccelerateInterpolator(0.3f);
		private readonly Interpolator ACCELERATE_INTERPOLATOR05 = new AccelerateInterpolator(0.5f);
		private readonly Interpolator ACCELERATE_INTERPOLATOR08 = new AccelerateInterpolator(0.8f);
		private readonly Interpolator ACCELERATE_INTERPOLATOR10 = new AccelerateInterpolator(1.0f);

		private readonly Interpolator DECELERATE_INTERPOLATOR03 = new DecelerateInterpolator(0.3f);
		private readonly Interpolator DECELERATE_INTERPOLATOR05 = new DecelerateInterpolator(0.5f);
		private readonly Interpolator DECELERATE_INTERPOLATOR08 = new DecelerateInterpolator(0.8f);
		private readonly Interpolator DECELERATE_INTERPOLATOR10 = new DecelerateInterpolator(1.0f);

		private float STAGE_MOTHER_FORWARD_TOP_LEFT = 0.34f;
		private float STAGE_MOTHER_BACKWARD_TOP_LEFT = 0.5f;
		private float STAGE_MOTHER_FORWARD_BOTTOM_LEFT = 0.65f;
		private float STAGE_MOTHER_BACKWARD_BOTTOM_LEFT = 0.833f;
		private float STAGE_CHILD_DELAY = 0.1f;
		private float STAGE_CHILD_PRE_FORWARD_TOP_LEFT = 0.26f;
		private float STAGE_CHILD_FORWARD_TOP_LEFT = 0.34f;
		private float STAGE_CHILD_PRE_BACKWARD_TOP_LEFT = 0.42f;
		private float STAGE_CHILD_BACKWARD_TOP_LEFT = 0.5f;
		private float STAGE_CHILD_FORWARD_BOTTOM_LEFT = 0.7f;
		private float STAGE_CHILD_BACKWARD_BOTTOM_LEFT = 0.9f;

		private readonly float OVAL_BEZIER_FACTOR = 0.55152f;

		private readonly float DEFAULT_WIDTH = 200.0f;
		private readonly float DEFAULT_HEIGHT = 150.0f;
		private readonly float MAX_MATHER_OVAL_SIZE = 19;
		private readonly float MIN_CHILD_OVAL_RADIUS = 5;
		private readonly float MAX_MATHER_SHAPE_CHANGE_FACTOR = 0.8452f;

		private readonly int DEFAULT_OVAL_COLOR = Color.ParseColor("#FFBE1C23");
		private readonly int DEFAULT_OVAL_DEEP_COLOR = Color.ParseColor("#FFB21721");
		private readonly int DEFAULT_BACKGROUND_COLOR = Color.ParseColor("#FFE3C172");
		private readonly int DEFAULT_BACKGROUND_DEEP_COLOR = Color.ParseColor("#FFE2B552");

		private readonly long ANIMATION_DURATION = 4111;

		private readonly Paint mPaint = new Paint();
		private readonly RectF mCurrentBounds = new RectF();
		private readonly Path mMotherOvalPath = new Path();
		private readonly Path mMotherMovePath = new Path();
		private readonly Path mChildMovePath = new Path();

		private readonly float[] mMotherPosition = new float[2];
		private readonly float[] mChildPosition = new float[2];
		private readonly PathMeasure mMotherMovePathMeasure = new PathMeasure();
		private readonly PathMeasure mChildMovePathMeasure = new PathMeasure();

		private float mChildOvalRadius;
		private float mBasicChildOvalRadius;
		private float mMaxMotherOvalSize;
		private float mMotherOvalHalfWidth;
		private float mMotherOvalHalfHeight;

		private float mChildLeftXOffset;
		private float mChildLeftYOffset;
		private float mChildRightXOffset;
		private float mChildRightYOffset;

		private int mOvalColor;
		private int mOvalDeepColor;
		private int mBackgroundColor;
		private int mBackgroundDeepColor;
		private int mCurrentOvalColor;
		private int mCurrentBackgroundColor;

		private int mRevealCircleRadius;
		private int mMaxRevealCircleRadius;

		private int mRotateDegrees;

		private float mStageMotherForwardTopLeftLength;
		private float mStageMotherBackwardTopLeftLength;
		private float mStageMotherForwardBottomLeftLength;
		private float mStageMotherBackwardBottomLeftLength;

		private float mStageChildPreForwardTopLeftLength;
		private float mStageChildForwardTopLeftLength;
		private float mStageChildPreBackwardTopLeftLength;
		private float mStageChildBackwardTopLeftLength;
		private float mStageChildForwardBottomLeftLength;
		private float mStageChildBackwardBottomLeftLength;

        internal CircleBroodLoadingRenderer(Context context) : base(context)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			Init(context);
			SetupPaint();
		}

		private void Init(Context context)
		{
			mWidth = DensityUtil.Dip2Px(context, DEFAULT_WIDTH);
			mHeight = DensityUtil.Dip2Px(context, DEFAULT_HEIGHT);

			mMaxMotherOvalSize = DensityUtil.Dip2Px(context, MAX_MATHER_OVAL_SIZE);
			mBasicChildOvalRadius = DensityUtil.Dip2Px(context, MIN_CHILD_OVAL_RADIUS);

			mOvalColor = DEFAULT_OVAL_COLOR;
			mOvalDeepColor = DEFAULT_OVAL_DEEP_COLOR;
			mBackgroundColor = DEFAULT_BACKGROUND_COLOR;
			mBackgroundDeepColor = DEFAULT_BACKGROUND_DEEP_COLOR;

			mMotherOvalHalfWidth = mMaxMotherOvalSize;
			mMotherOvalHalfHeight = mMaxMotherOvalSize;

			mMaxRevealCircleRadius = (int)(Math.Sqrt(mWidth * mWidth + mHeight * mHeight) / 2 + 1);

			mDuration = ANIMATION_DURATION;
		}

		private void SetupPaint()
		{
			mPaint.AntiAlias = true;
			mPaint.SetStyle(Paint.Style.Fill);
			mPaint.StrokeWidth = 1.0f;
		}

		protected internal override void Draw(Canvas canvas, Rect bounds)
		{
			int SaveCount = canvas.Save();

			RectF arcBounds = mCurrentBounds;
			arcBounds.Set(bounds);

			//Draw background
			canvas.DrawColor(new Color (mCurrentBackgroundColor));
			//Draw reveal circle
			if (mRevealCircleRadius > 0)
			{
				mPaint.Color =new Color(mCurrentBackgroundColor == mBackgroundColor ? mBackgroundDeepColor : mBackgroundColor);
				canvas.DrawCircle(arcBounds.CenterX(), arcBounds.CenterY(), mRevealCircleRadius, mPaint);
			}

			//Draw mother oval
			mPaint.Color = new Color (mCurrentOvalColor);

			int motherSaveCount = canvas.Save();
			canvas.Rotate(mRotateDegrees, mMotherPosition[0], mMotherPosition[1]);
			canvas.DrawPath(CreateMotherPath(), mPaint);
			canvas.DrawPath(CreateLinkPath(), mPaint);
			canvas.RestoreToCount(motherSaveCount);

			int childSaveCount = canvas.Save();
			canvas.Rotate(mRotateDegrees, mChildPosition[0], mChildPosition[1]);
			canvas.DrawPath(CreateChildPath(), mPaint);
			canvas.RestoreToCount(childSaveCount);
			canvas.RestoreToCount(SaveCount);

	//    canvas.DrawPath(mMotherMovePath, mPaint);
	//    canvas.DrawPath(mChildMovePath, mPaint);
	//    canvas.DrawLine(mMotherPosition[0], mMotherPosition[1], mChildPosition[0], mChildPosition[1], mPaint);
		}

		private Path CreateMotherPath()
		{
			mMotherOvalPath.Reset();

			mMotherOvalPath.AddOval(new RectF(mMotherPosition[0] - mMotherOvalHalfWidth, mMotherPosition[1] - mMotherOvalHalfHeight, mMotherPosition[0] + mMotherOvalHalfWidth, mMotherPosition[1] + mMotherOvalHalfHeight), Path.Direction.Cw);

			return mMotherOvalPath;
		}

		private Path CreateChildPath()
		{
			float bezierOffset = mChildOvalRadius * OVAL_BEZIER_FACTOR;

			Path path = new Path();
			path.MoveTo(mChildPosition[0], mChildPosition[1] - mChildOvalRadius);
			//left_top arc
			path.CubicTo(mChildPosition[0] - bezierOffset - mChildLeftXOffset, mChildPosition[1] - mChildOvalRadius, mChildPosition[0] - mChildOvalRadius - mChildLeftXOffset, mChildPosition[1] - bezierOffset + mChildLeftYOffset, mChildPosition[0] - mChildOvalRadius - mChildLeftXOffset, mChildPosition[1]);
			//left_bottom arc
			path.CubicTo(mChildPosition[0] - mChildOvalRadius - mChildLeftXOffset, mChildPosition[1] + bezierOffset - mChildLeftYOffset, mChildPosition[0] - bezierOffset - mChildLeftXOffset, mChildPosition[1] + mChildOvalRadius, mChildPosition[0], mChildPosition[1] + mChildOvalRadius);

			//right_bottom arc
			path.CubicTo(mChildPosition[0] + bezierOffset + mChildRightXOffset, mChildPosition[1] + mChildOvalRadius, mChildPosition[0] + mChildOvalRadius + mChildRightXOffset, mChildPosition[1] + bezierOffset - mChildRightYOffset, mChildPosition[0] + mChildOvalRadius + mChildRightXOffset, mChildPosition[1]);
			//right_top arc
			path.CubicTo(mChildPosition[0] + mChildOvalRadius + mChildRightXOffset, mChildPosition[1] - bezierOffset + mChildRightYOffset, mChildPosition[0] + bezierOffset + mChildRightXOffset, mChildPosition[1] - mChildOvalRadius, mChildPosition[0], mChildPosition[1] - mChildOvalRadius);

			return path;
		}

		private Path CreateLinkPath()
		{
			Path path = new Path();
			float bezierOffset = mMotherOvalHalfWidth * OVAL_BEZIER_FACTOR;

			float distance = (float) Math.Sqrt(Math.Pow(mMotherPosition[0] - mChildPosition[0], 2.0f) + Math.Pow(mMotherPosition[1] - mChildPosition[1], 2.0f));
			if (distance <= mMotherOvalHalfWidth + mChildOvalRadius * 1.2f && distance >= mMotherOvalHalfWidth - mChildOvalRadius * 1.2f)
			{
				float maxOffsetY = 2 * mChildOvalRadius * 1.2f;
				float offsetRate = (distance - (mMotherOvalHalfWidth - mChildOvalRadius * 1.2f)) / maxOffsetY;

				float mMotherOvalOffsetY = mMotherOvalHalfHeight - offsetRate * (mMotherOvalHalfHeight - mChildOvalRadius) * 0.85f;

				mMotherOvalPath.AddOval(new RectF(mMotherPosition[0] - mMotherOvalHalfWidth, mMotherPosition[1] - mMotherOvalOffsetY, mMotherPosition[0] + mMotherOvalHalfWidth, mMotherPosition[1] + mMotherOvalOffsetY), Path.Direction.Cw);

				float mMotherXOffset = distance - mMotherOvalHalfWidth + mChildOvalRadius;
				float distanceUltraLeft = (float) Math.Sqrt(Math.Pow(mMotherPosition[0] - mMotherOvalHalfWidth - mChildPosition[0], 2.0f) + Math.Pow(mMotherPosition[1] - mChildPosition[1], 2.0f));
				float distanceUltraRight = (float) Math.Sqrt(Math.Pow(mMotherPosition[0] + mMotherOvalHalfWidth - mChildPosition[0], 2.0f) + Math.Pow(mMotherPosition[1] - mChildPosition[1], 2.0f));

				path.MoveTo(mMotherPosition[0], mMotherPosition[1] + mMotherOvalOffsetY);
				if (distanceUltraRight < distanceUltraLeft)
				{
					//right_bottom arc
					path.CubicTo(mMotherPosition[0] + bezierOffset + mMotherXOffset, mMotherPosition[1] + mMotherOvalOffsetY, mMotherPosition[0] + distance + mChildOvalRadius, mMotherPosition[1] + mChildOvalRadius * 1.5f, mMotherPosition[0] + distance + mChildOvalRadius, mMotherPosition[1]);
					//right_top arc
					path.CubicTo(mMotherPosition[0] + distance + mChildOvalRadius, mMotherPosition[1] - mChildOvalRadius * 1.5f, mMotherPosition[0] + bezierOffset + mMotherXOffset, mMotherPosition[1] - mMotherOvalOffsetY, mMotherPosition[0], mMotherPosition[1] - mMotherOvalOffsetY);
				}
				else
				{
					//left_bottom arc
					path.CubicTo(mMotherPosition[0] - bezierOffset - mMotherXOffset, mMotherPosition[1] + mMotherOvalOffsetY, mMotherPosition[0] - distance - mChildOvalRadius, mMotherPosition[1] + mChildOvalRadius * 1.5f, mMotherPosition[0] - distance - mChildOvalRadius, mMotherPosition[1]);
					//left_top arc
					path.CubicTo(mMotherPosition[0] - distance - mChildOvalRadius, mMotherPosition[1] - mChildOvalRadius * 1.5f, mMotherPosition[0] - bezierOffset - mMotherXOffset, mMotherPosition[1] - mMotherOvalOffsetY, mMotherPosition[0], mMotherPosition[1] - mMotherOvalOffsetY);
				}
				path.LineTo(mMotherPosition[0], mMotherPosition[1] + mMotherOvalOffsetY);
			}

			return path;
		}

		protected internal override void ComputeRender(float renderProgress)
		{
			if (mCurrentBounds.IsEmpty)
			{
				return;
			}

			if (mMotherMovePath.IsEmpty)
			{
				mMotherMovePath.Set(CreateMotherMovePath());
				mMotherMovePathMeasure.SetPath(mMotherMovePath, false);

				mChildMovePath.Set(CreateChildMovePath());
				mChildMovePathMeasure.SetPath(mChildMovePath, false);
			}

			//mother oval
			float motherMoveProgress = MOTHER_MOVE_INTERPOLATOR.GetInterpolation(renderProgress);
			mMotherMovePathMeasure.GetPosTan(GetCurrentMotherMoveLength(motherMoveProgress), mMotherPosition, null);
			mMotherOvalHalfWidth = mMaxMotherOvalSize;
			mMotherOvalHalfHeight = mMaxMotherOvalSize * GetMotherShapeFactor(motherMoveProgress);

			//child Oval
			float childMoveProgress = CHILD_MOVE_INTERPOLATOR.GetInterpolation(renderProgress);
			mChildMovePathMeasure.GetPosTan(GetCurrentChildMoveLength(childMoveProgress), mChildPosition, null);
			SetupChildParams(childMoveProgress);

            mRotateDegrees = (int)(DensityUtil.Radian2Degrees(Math.Atan((mMotherPosition[1] - mChildPosition[1]) / (mMotherPosition[0] - mChildPosition[0]))));

			mRevealCircleRadius = GetCurrentRevealCircleRadius(renderProgress);
			mCurrentOvalColor = GetCurrentOvalColor(renderProgress);
			mCurrentBackgroundColor = GetCurrentBackgroundColor(renderProgress);
		}

		private void SetupChildParams(float input)
		{
			mChildOvalRadius = mBasicChildOvalRadius;

			mChildRightXOffset = 0.0f;
			mChildLeftXOffset = 0.0f;

			if (input <= STAGE_CHILD_PRE_FORWARD_TOP_LEFT)
			{
				if (input >= 0.25)
				{
					float shapeProgress = (input - 0.25f) / 0.01f;
					mChildLeftXOffset = (1.0f - shapeProgress) * mChildOvalRadius * 0.25f;
				}
				else
				{
					mChildLeftXOffset = mChildOvalRadius * 0.25f;
				}
			}
			else if (input <= STAGE_CHILD_FORWARD_TOP_LEFT)
			{
				if (input > 0.275f && input < 0.285f)
				{
					float shapeProgress = (input - 0.275f) / 0.01f;
					mChildLeftXOffset = shapeProgress * mChildOvalRadius * 0.25f;
				}
				else if (input > 0.285f)
				{
					mChildLeftXOffset = mChildOvalRadius * 0.25f;
				}
			}
			else if (input <= STAGE_CHILD_PRE_BACKWARD_TOP_LEFT)
			{
				if (input > 0.38f)
				{
					float radiusProgress = (input - 0.38f) / 0.04f;
					mChildOvalRadius = mBasicChildOvalRadius * (1.0f + radiusProgress);
				}
			}
			else if (input <= STAGE_CHILD_BACKWARD_TOP_LEFT)
			{
				if (input < 0.46f)
				{
					float radiusProgress = (input - 0.42f) / 0.04f;
					mChildOvalRadius = mBasicChildOvalRadius * (2.0f - radiusProgress);
				}
			}
			else if (input <= STAGE_CHILD_FORWARD_BOTTOM_LEFT)
			{
				if (input > 0.65f)
				{
					float radiusProgress = (input - 0.65f) / 0.05f;
					mChildOvalRadius = mBasicChildOvalRadius * (1.0f + radiusProgress);
				}
			}
			else if (input <= STAGE_CHILD_BACKWARD_BOTTOM_LEFT)
			{
				if (input < 0.71f)
				{
					mChildOvalRadius = mBasicChildOvalRadius * 2.0f;
				}
				else if (input < 0.76f)
				{
					float radiusProgress = (input - 0.71f) / 0.05f;
					mChildOvalRadius = mBasicChildOvalRadius * (2.0f - radiusProgress);
				}
			}
			else
			{
			}

			mChildRightYOffset = mChildRightXOffset / 2.5f;
			mChildLeftYOffset = mChildLeftXOffset / 2.5f;
		}

		private float GetMotherShapeFactor(float input)
		{

			float shapeProgress;
			if (input <= STAGE_MOTHER_FORWARD_TOP_LEFT)
			{
				shapeProgress = input / STAGE_MOTHER_FORWARD_TOP_LEFT;
			}
			else if (input <= STAGE_MOTHER_BACKWARD_TOP_LEFT)
			{
				shapeProgress = (input - STAGE_MOTHER_FORWARD_TOP_LEFT) / (STAGE_MOTHER_BACKWARD_TOP_LEFT - STAGE_MOTHER_FORWARD_TOP_LEFT);
			}
			else if (input <= STAGE_MOTHER_FORWARD_BOTTOM_LEFT)
			{
				shapeProgress = (input - STAGE_MOTHER_BACKWARD_TOP_LEFT) / (STAGE_MOTHER_FORWARD_BOTTOM_LEFT - STAGE_MOTHER_BACKWARD_TOP_LEFT);
			}
			else if (input <= STAGE_MOTHER_BACKWARD_BOTTOM_LEFT)
			{
				shapeProgress = (input - STAGE_MOTHER_FORWARD_BOTTOM_LEFT) / (STAGE_MOTHER_BACKWARD_BOTTOM_LEFT - STAGE_MOTHER_FORWARD_BOTTOM_LEFT);
			}
			else
			{
				shapeProgress = 1.0f;
			}

			return shapeProgress < 0.5f ? 1.0f - (1.0f - MAX_MATHER_SHAPE_CHANGE_FACTOR) * shapeProgress * 2.0f : MAX_MATHER_SHAPE_CHANGE_FACTOR + (1.0f - MAX_MATHER_SHAPE_CHANGE_FACTOR) * (shapeProgress - 0.5f) * 2.0f;
		}

		private float GetCurrentMotherMoveLength(float input)
		{
			float currentStartDistance = 0.0f;
			float currentStageDistance = 0.0f;
			float currentStateStartProgress = 0.0f;
			float currentStateEndProgress = 0.0f;

			if (input > 0.0f)
			{
				currentStartDistance += currentStageDistance;
				currentStageDistance = mStageMotherForwardTopLeftLength;
				currentStateStartProgress = 0.0f;
				currentStateEndProgress = STAGE_MOTHER_FORWARD_TOP_LEFT;
			}

			if (input > STAGE_MOTHER_FORWARD_TOP_LEFT)
			{
				currentStartDistance += currentStageDistance;
				currentStageDistance = mStageMotherBackwardTopLeftLength;
				currentStateStartProgress = STAGE_MOTHER_FORWARD_TOP_LEFT;
				currentStateEndProgress = STAGE_MOTHER_BACKWARD_TOP_LEFT;
			}

			if (input > STAGE_MOTHER_BACKWARD_TOP_LEFT)
			{
				currentStartDistance += currentStageDistance;
				currentStageDistance = mStageMotherForwardBottomLeftLength;
				currentStateStartProgress = STAGE_MOTHER_BACKWARD_TOP_LEFT;
				currentStateEndProgress = STAGE_MOTHER_FORWARD_BOTTOM_LEFT;
			}

			if (input > STAGE_MOTHER_FORWARD_BOTTOM_LEFT)
			{
				currentStartDistance += currentStageDistance;
				currentStageDistance = mStageMotherBackwardBottomLeftLength;
				currentStateStartProgress = STAGE_MOTHER_FORWARD_BOTTOM_LEFT;
				currentStateEndProgress = STAGE_MOTHER_BACKWARD_BOTTOM_LEFT;
			}

			if (input > STAGE_MOTHER_BACKWARD_BOTTOM_LEFT)
			{
				return currentStartDistance + currentStageDistance;
			}

			return currentStartDistance + (input - currentStateStartProgress) / (currentStateEndProgress - currentStateStartProgress) * currentStageDistance;
		}

		private float GetCurrentChildMoveLength(float input)
		{
			float currentStartDistance = 0.0f;
			float currentStageDistance = 0.0f;
			float currentStateStartProgress = 0.0f;
			float currentStateEndProgress = 0.0f;

			if (input > 0.0f)
			{
				currentStartDistance += currentStageDistance;
				currentStageDistance = mStageChildPreForwardTopLeftLength;
				currentStateStartProgress = 0.0f;
				currentStateEndProgress = STAGE_CHILD_PRE_FORWARD_TOP_LEFT;
			}

			if (input > STAGE_CHILD_PRE_FORWARD_TOP_LEFT)
			{
				currentStartDistance += currentStageDistance;
				currentStageDistance = mStageChildForwardTopLeftLength;
				currentStateStartProgress = STAGE_CHILD_PRE_FORWARD_TOP_LEFT;
				currentStateEndProgress = STAGE_CHILD_FORWARD_TOP_LEFT;
			}

			if (input > STAGE_CHILD_FORWARD_TOP_LEFT)
			{
				currentStartDistance += currentStageDistance;
				currentStageDistance = mStageChildPreBackwardTopLeftLength;
				currentStateStartProgress = STAGE_CHILD_FORWARD_TOP_LEFT;
				currentStateEndProgress = STAGE_CHILD_PRE_BACKWARD_TOP_LEFT;
			}

			if (input > STAGE_CHILD_PRE_BACKWARD_TOP_LEFT)
			{
				currentStartDistance += currentStageDistance;
				currentStageDistance = mStageChildBackwardTopLeftLength;
				currentStateStartProgress = STAGE_CHILD_PRE_BACKWARD_TOP_LEFT;
				currentStateEndProgress = STAGE_CHILD_BACKWARD_TOP_LEFT;
			}

			if (input > STAGE_CHILD_BACKWARD_TOP_LEFT)
			{
				currentStartDistance += currentStageDistance;
				currentStageDistance = mStageChildForwardBottomLeftLength;
				currentStateStartProgress = STAGE_CHILD_BACKWARD_TOP_LEFT;
				currentStateEndProgress = STAGE_CHILD_FORWARD_BOTTOM_LEFT;
			}

			if (input > STAGE_CHILD_FORWARD_BOTTOM_LEFT)
			{
				currentStartDistance += currentStageDistance;
				currentStageDistance = mStageChildBackwardBottomLeftLength;
				currentStateStartProgress = STAGE_CHILD_FORWARD_BOTTOM_LEFT;
				currentStateEndProgress = STAGE_CHILD_BACKWARD_BOTTOM_LEFT;
			}

			if (input > STAGE_CHILD_BACKWARD_BOTTOM_LEFT)
			{
				return currentStartDistance + currentStageDistance;
			}

			return currentStartDistance + (input - currentStateStartProgress) / (currentStateEndProgress - currentStateStartProgress) * currentStageDistance;
		}

		private Path CreateMotherMovePath()
		{
			Path path = new Path();

			float CenterX = mCurrentBounds.CenterX();
			float CenterY = mCurrentBounds.CenterY();
			float currentPathLength = 0.0f;

			path.MoveTo(CenterX, CenterY);
			//forward top left
			path.QuadTo(CenterX - mMotherOvalHalfWidth * 2.0f, CenterY, CenterX - mMotherOvalHalfWidth * 2.0f, CenterY - mMotherOvalHalfHeight);
			mStageMotherForwardTopLeftLength = GetRestLength(path, currentPathLength);
			currentPathLength += mStageMotherForwardTopLeftLength;

			//backward top left
			path.QuadTo(CenterX - mMotherOvalHalfWidth * 1.0f, CenterY - mMotherOvalHalfHeight, CenterX, CenterY);
			mStageMotherBackwardTopLeftLength = GetRestLength(path, currentPathLength);
			currentPathLength += mStageMotherBackwardTopLeftLength;
			//forward bottom left
			path.QuadTo(CenterX, CenterY + mMotherOvalHalfHeight, CenterX - mMotherOvalHalfWidth / 2, CenterY + mMotherOvalHalfHeight * 1.1f);
			mStageMotherForwardBottomLeftLength = GetRestLength(path, currentPathLength);
			currentPathLength += mStageMotherForwardBottomLeftLength;
			//backward bottom left
			path.QuadTo(CenterX - mMotherOvalHalfWidth / 2, CenterY + mMotherOvalHalfHeight * 0.6f, CenterX, CenterY);
			mStageMotherBackwardBottomLeftLength = GetRestLength(path, currentPathLength);

			return path;
		}

		private Path CreateChildMovePath()
		{
			Path path = new Path();

			float CenterX = mCurrentBounds.CenterX();
			float CenterY = mCurrentBounds.CenterY();
			float currentPathLength = 0.0f;

			//start
			path.MoveTo(CenterX, CenterY);
			//pre forward top left
			path.LineTo(CenterX + mMotherOvalHalfWidth * 0.75f, CenterY);
			mStageChildPreForwardTopLeftLength = GetRestLength(path, currentPathLength);
			currentPathLength += mStageChildPreForwardTopLeftLength;
			//forward top left
			path.QuadTo(CenterX - mMotherOvalHalfWidth * 0.5f, CenterY, CenterX - mMotherOvalHalfWidth * 2.0f, CenterY - mMotherOvalHalfHeight);
			mStageChildForwardTopLeftLength = GetRestLength(path, currentPathLength);
			currentPathLength += mStageChildForwardTopLeftLength;
			//pre backward top left
			path.LineTo(CenterX - mMotherOvalHalfWidth * 2.0f + mMotherOvalHalfWidth * 0.2f, CenterY - mMotherOvalHalfHeight);
			path.QuadTo(CenterX - mMotherOvalHalfWidth * 2.5f, CenterY - mMotherOvalHalfHeight * 2, CenterX - mMotherOvalHalfWidth * 1.5f, CenterY - mMotherOvalHalfHeight * 2.25f);
			mStageChildPreBackwardTopLeftLength = GetRestLength(path, currentPathLength);
			currentPathLength += mStageChildPreBackwardTopLeftLength;
			//backward top left
			path.QuadTo(CenterX - mMotherOvalHalfWidth * 0.2f, CenterY - mMotherOvalHalfHeight * 2.25f, CenterX, CenterY);
			mStageChildBackwardTopLeftLength = GetRestLength(path, currentPathLength);
			currentPathLength += mStageChildBackwardTopLeftLength;
			//forward bottom left
			path.CubicTo(CenterX, CenterY + mMotherOvalHalfHeight, CenterX - mMotherOvalHalfWidth, CenterY + mMotherOvalHalfHeight * 2.5f, CenterX - mMotherOvalHalfWidth * 1.5f, CenterY + mMotherOvalHalfHeight * 2.5f);
			mStageChildForwardBottomLeftLength = GetRestLength(path, currentPathLength);
			currentPathLength += mStageChildForwardBottomLeftLength;
			//backward bottom left
			path.CubicTo(CenterX - mMotherOvalHalfWidth * 2.0f, CenterY + mMotherOvalHalfHeight * 2.5f, CenterX - mMotherOvalHalfWidth * 3.0f, CenterY + mMotherOvalHalfHeight * 0.8f, CenterX, CenterY);
			mStageChildBackwardBottomLeftLength = GetRestLength(path, currentPathLength);

			return path;
		}

		private int GetCurrentRevealCircleRadius(float input)
		{
			int result = 0;
			if (input > 0.44f && input < 0.48f)
			{
				result = (int)((input - 0.44f) / 0.04f * mMaxRevealCircleRadius);
			}

			if (input > 0.81f && input < 0.85f)
			{
				result = (int)((input - 0.81f) / 0.04f * mMaxRevealCircleRadius);
			}

			return result;
		}

		private int GetCurrentBackgroundColor(float input)
		{
			return input < 0.48f || input > 0.85f ? mBackgroundColor : mBackgroundDeepColor;
		}

		private int GetCurrentOvalColor(float input)
		{
			int result;

			if (input < 0.5f)
			{
				result = mOvalColor;
			}
			else if (input < 0.75f)
			{
				float colorProgress = (input - 0.5f) / 0.2f;
				result = EvaluateColorChange(colorProgress, mOvalColor, mOvalDeepColor);
			}
			else if (input < 0.85f)
			{
				result = mOvalDeepColor;
			}
			else
			{
				float colorProgress = (input - 0.9f) / 0.1f;
				result = EvaluateColorChange(colorProgress, mOvalDeepColor, mOvalColor);
			}

			return result;
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

		private float GetRestLength(Path path, float startD)
		{
			Path tempPath = new Path();
			PathMeasure pathMeasure = new PathMeasure(path, false);

			pathMeasure.GetSegment(startD, pathMeasure.Length, tempPath, true);

			pathMeasure.SetPath(tempPath, false);

			return pathMeasure.Length;
		}

		protected internal override int Alpha
        {
            set => mPaint.Alpha = value;
        }

        protected internal override ColorFilter ColorFilter
        {
            set => mPaint.SetColorFilter(value);
        }

		private class MotherMoveInterpolator :Java.Lang.Object, Interpolator
		{
			private readonly CircleBroodLoadingRenderer outerInstance;

			public MotherMoveInterpolator(CircleBroodLoadingRenderer outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public  float GetInterpolation(float input)
			{
				float result;

				if (input <= outerInstance.STAGE_MOTHER_FORWARD_TOP_LEFT)
				{
					result = outerInstance.ACCELERATE_INTERPOLATOR10.GetInterpolation(input * 2.941f) / 2.941f;
				}
				else if (input <= outerInstance.STAGE_MOTHER_BACKWARD_TOP_LEFT)
				{
					result = 0.34f + outerInstance.DECELERATE_INTERPOLATOR10.GetInterpolation((input - 0.34f) * 6.25f) / 6.25f;
				}
				else if (input <= outerInstance.STAGE_MOTHER_FORWARD_BOTTOM_LEFT)
				{
					result = 0.5f + outerInstance.ACCELERATE_INTERPOLATOR03.GetInterpolation((input - 0.5f) * 6.666f) / 4.0f;
				}
				else if (input <= outerInstance.STAGE_MOTHER_BACKWARD_BOTTOM_LEFT)
				{
					result = 0.75f + outerInstance.DECELERATE_INTERPOLATOR03.GetInterpolation((input - 0.65f) * 5.46f) / 4.0f;
				}
				else
				{
					result = 1.0f;
				}

				return result;
			}
		}

		private class ChildMoveInterpolator : Java.Lang.Object, Interpolator
		{
			private readonly CircleBroodLoadingRenderer outerInstance;

			public ChildMoveInterpolator(CircleBroodLoadingRenderer outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public  float GetInterpolation(float input)
			{
				float result;

				if (input < outerInstance.STAGE_CHILD_DELAY)
				{
					return 0.0f;
				}
				else if (input <= outerInstance.STAGE_CHILD_PRE_FORWARD_TOP_LEFT)
				{
					result = outerInstance.DECELERATE_INTERPOLATOR10.GetInterpolation((input - 0.1f) * 6.25f) / 3.846f;
				}
				else if (input <= outerInstance.STAGE_CHILD_FORWARD_TOP_LEFT)
				{
					result = 0.26f + outerInstance.ACCELERATE_INTERPOLATOR10.GetInterpolation((input - 0.26f) * 12.5f) / 12.5f;
				}
				else if (input <= outerInstance.STAGE_CHILD_PRE_BACKWARD_TOP_LEFT)
				{
					result = 0.34f + outerInstance.DECELERATE_INTERPOLATOR08.GetInterpolation((input - 0.34f) * 12.5f) / 12.5f;
				}
				else if (input <= outerInstance.STAGE_CHILD_BACKWARD_TOP_LEFT)
				{
					result = 0.42f + outerInstance.ACCELERATE_INTERPOLATOR08.GetInterpolation((input - 0.42f) * 12.5f) / 12.5f;
				}
				else if (input <= outerInstance.STAGE_CHILD_FORWARD_BOTTOM_LEFT)
				{
					result = 0.5f + outerInstance.DECELERATE_INTERPOLATOR05.GetInterpolation((input - 0.5f) * 5.0f) / 5.0f;
				}
				else if (input <= outerInstance.STAGE_CHILD_BACKWARD_BOTTOM_LEFT)
				{
					result = 0.7f + outerInstance.ACCELERATE_INTERPOLATOR05.GetInterpolation((input - 0.7f) * 5.0f) / 3.33f;
				}
				else
				{
					result = 1.0f;
				}

				return result;
			}
		}

	}
}