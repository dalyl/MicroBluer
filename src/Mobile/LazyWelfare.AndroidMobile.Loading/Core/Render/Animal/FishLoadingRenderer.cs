namespace LazyWelfare.AndroidMobile.Loading.Render.Animal
{

    using Context = Android.Content.Context;
    using Canvas = Android.Graphics.Canvas;
    using Color = Android.Graphics.Color;
    using ColorFilter = Android.Graphics.ColorFilter;
    using Paint = Android.Graphics.Paint;
    using LinearGradient = Android.Graphics.LinearGradient;
    using Path = Android.Graphics.Path;
	using Rect = Android.Graphics.Rect;
    using RectF = Android.Graphics.RectF;
    using Shader = Android.Graphics.Shader;
	using Matrix = Android.Graphics.Matrix;
    using PathMeasure = Android.Graphics.PathMeasure;
    using Region = Android.Graphics.Region;
    using DashPathEffect = Android.Graphics.DashPathEffect;
    using Interpolator = Android.Views.Animations.IInterpolator;
    using AccelerateInterpolator = Android.Views.Animations.AccelerateInterpolator;
    using DecelerateInterpolator = Android.Views.Animations.DecelerateInterpolator;
    using Android.Graphics;
    using System;

    using FastOutSlowInInterpolator = Android.Support.V4.View.Animation.FastOutSlowInInterpolator;
    using DisplayMetrics = Android.Util.DisplayMetrics;
    using TypedValue = Android.Util.TypedValue;




	public class FishLoadingRenderer : LoadingRenderer
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			FISH_INTERPOLATOR = new FishInterpolator(this);
			FISH_MOVE_POINTS_RATE = 1.0f / FISH_MOVE_POINTS.Length;
		}

		private Interpolator FISH_INTERPOLATOR;

		private const float DEFAULT_PATH_FULL_LINE_SIZE = 7.0f;
		private static readonly float DEFAULT_PATH_DOTTED_LINE_SIZE = DEFAULT_PATH_FULL_LINE_SIZE / 2.0f;
		private static readonly float DEFAULT_RIVER_HEIGHT = DEFAULT_PATH_FULL_LINE_SIZE * 8.5f;
		private static readonly float DEFAULT_RIVER_WIDTH = DEFAULT_PATH_FULL_LINE_SIZE * 5.5f;

		private static readonly float DEFAULT_FISH_EYE_SIZE = DEFAULT_PATH_FULL_LINE_SIZE * 0.5f;
		private static readonly float DEFAULT_FISH_WIDTH = DEFAULT_PATH_FULL_LINE_SIZE * 3.0f;
		private static readonly float DEFAULT_FISH_HEIGHT = DEFAULT_PATH_FULL_LINE_SIZE * 4.5f;

		private const float DEFAULT_WIDTH = 200.0f;
		private const float DEFAULT_HEIGHT = 150.0f;
		private const float DEFAULT_RIVER_BANK_WIDTH = DEFAULT_PATH_FULL_LINE_SIZE;

		private const long ANIMATION_DURATION = 800;
		private static readonly float DOTTED_LINE_WIDTH_COUNT = (8.5f + 5.5f - 2.0f) * 2.0f * 2.0f;
		private static readonly float DOTTED_LINE_WIDTH_RATE = 1.0f / DOTTED_LINE_WIDTH_COUNT;

		private readonly float[] FISH_MOVE_POINTS = new float[]{DOTTED_LINE_WIDTH_RATE * 3.0f, DOTTED_LINE_WIDTH_RATE * 6.0f, DOTTED_LINE_WIDTH_RATE * 15f, DOTTED_LINE_WIDTH_RATE * 18f, DOTTED_LINE_WIDTH_RATE * 27.0f, DOTTED_LINE_WIDTH_RATE * 30.0f, DOTTED_LINE_WIDTH_RATE * 39f, DOTTED_LINE_WIDTH_RATE * 42f};

		private float FISH_MOVE_POINTS_RATE;

		private static readonly int DEFAULT_COLOR = Color.ParseColor("#fffefed6");

		private readonly Paint mPaint = new Paint();
		private readonly RectF mTempBounds = new RectF();

		private readonly float[] mFishHeadPos = new float[2];

		private Path mRiverPath;
		private PathMeasure mRiverMeasure;

		private float mFishRotateDegrees;

		private float mRiverBankWidth;
		private float mRiverWidth;
		private float mRiverHeight;
		private float mFishWidth;
		private float mFishHeight;
		private float mFishEyeSize;
		private float mPathFullLineSize;
		private float mPathDottedLineSize;

		private int mColor;

		private FishLoadingRenderer(Context context) : base(context)
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
			mWidth = DensityUtil.dip2px(context, DEFAULT_WIDTH);
			mHeight = DensityUtil.dip2px(context, DEFAULT_HEIGHT);
			mRiverBankWidth = DensityUtil.dip2px(context, DEFAULT_RIVER_BANK_WIDTH);

			mPathFullLineSize = DensityUtil.dip2px(context, DEFAULT_PATH_FULL_LINE_SIZE);
			mPathDottedLineSize = DensityUtil.dip2px(context, DEFAULT_PATH_DOTTED_LINE_SIZE);
			mFishWidth = DensityUtil.dip2px(context, DEFAULT_FISH_WIDTH);
			mFishHeight = DensityUtil.dip2px(context, DEFAULT_FISH_HEIGHT);
			mFishEyeSize = DensityUtil.dip2px(context, DEFAULT_FISH_EYE_SIZE);
			mRiverWidth = DensityUtil.dip2px(context, DEFAULT_RIVER_WIDTH);
			mRiverHeight = DensityUtil.dip2px(context, DEFAULT_RIVER_HEIGHT);

			mColor = DEFAULT_COLOR;

			mDuration = ANIMATION_DURATION;
		}

		private void SetupPaint()
		{
			mPaint.AntiAlias = true;
			mPaint.StrokeWidth = mRiverBankWidth;
			mPaint.SetStyle(Paint.Style.Stroke);
			mPaint.StrokeJoin = Paint.Join.Miter;
            mPaint.SetPathEffect(new DashPathEffect(new float[] { mPathFullLineSize, mPathDottedLineSize }, mPathDottedLineSize));
		}

		protected internal override void Draw(Canvas canvas, Rect bounds)
		{
			int saveCount = canvas.Save();
			RectF arcBounds = mTempBounds;
			arcBounds.Set(bounds);

			mPaint.Color = new Color (mColor);

			//calculate fish clip bounds
			//clip the width of the fish need to increase mPathDottedLineSize * 1.2f
			RectF fishRectF = new RectF(mFishHeadPos[0] - mFishWidth / 2.0f - mPathDottedLineSize * 1.2f, mFishHeadPos[1] - mFishHeight / 2.0f, mFishHeadPos[0] + mFishWidth / 2.0f + mPathDottedLineSize * 1.2f, mFishHeadPos[1] + mFishHeight / 2.0f);
			Matrix matrix = new Matrix();
			matrix.PostRotate(mFishRotateDegrees, fishRectF.CenterX(), fishRectF.CenterY());
			matrix.MapRect(fishRectF);

			//draw river
			int riverSaveCount = canvas.Save();
			mPaint.SetStyle (Paint.Style.Stroke);
			canvas.ClipRect(fishRectF, Region.Op.Difference);
			canvas.DrawPath(createRiverPath(arcBounds), mPaint);
			canvas.RestoreToCount(riverSaveCount);

			//draw fish
			int fishSaveCount = canvas.Save();
			mPaint.SetStyle (Paint.Style.Fill);
			canvas.Rotate(mFishRotateDegrees, mFishHeadPos[0], mFishHeadPos[1]);
			canvas.ClipPath(CreateFishEyePath(mFishHeadPos[0], mFishHeadPos[1] - mFishHeight * 0.06f), Region.Op.Difference);
			canvas.DrawPath(CreateFishPath(mFishHeadPos[0], mFishHeadPos[1]), mPaint);
			canvas.RestoreToCount(fishSaveCount);

			canvas.RestoreToCount(saveCount);
		}

		private float CalculateRotateDegrees(float fishProgress)
		{
			if (fishProgress < FISH_MOVE_POINTS_RATE * 2)
			{
				return 90;
			}

			if (fishProgress < FISH_MOVE_POINTS_RATE * 4)
			{
				return 180;
			}

			if (fishProgress < FISH_MOVE_POINTS_RATE * 6)
			{
				return 270;
			}

			return 0.0f;
		}

		protected internal override void ComputeRender(float renderProgress)
		{
			if (mRiverPath == null)
			{
				return;
			}

			if (mRiverMeasure == null)
			{
				mRiverMeasure = new PathMeasure(mRiverPath, false);
			}

			float fishProgress = FISH_INTERPOLATOR.GetInterpolation(renderProgress);

			mRiverMeasure.GetPosTan(mRiverMeasure.Length * fishProgress, mFishHeadPos, null);
			mFishRotateDegrees = CalculateRotateDegrees(fishProgress);
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
		}

		private Path CreateFishEyePath(float fishEyeCenterX, float fishEyeCenterY)
		{
			Path path = new Path();
			path.AddCircle(fishEyeCenterX, fishEyeCenterY, mFishEyeSize, Path.Direction.Cw);

			return path;
		}

		private Path CreateFishPath(float fishCenterX, float fishCenterY)
		{
			Path path = new Path();

			float fishHeadX = fishCenterX;
			float fishHeadY = fishCenterY - mFishHeight / 2.0f;

			//the head of the fish
			path.MoveTo(fishHeadX, fishHeadY);
			//the left body of the fish
			path.QuadTo(fishHeadX - mFishWidth * 0.333f, fishHeadY + mFishHeight * 0.222f, fishHeadX - mFishWidth * 0.333f, fishHeadY + mFishHeight * 0.444f);
			path.LineTo(fishHeadX - mFishWidth * 0.333f, fishHeadY + mFishHeight * 0.666f);
			path.LineTo(fishHeadX - mFishWidth * 0.5f, fishHeadY + mFishHeight * 0.8f);
			path.LineTo(fishHeadX - mFishWidth * 0.5f, fishHeadY + mFishHeight);

			//the tail of the fish
			path.LineTo(fishHeadX, fishHeadY + mFishHeight * 0.9f);

			//the right body of the fish
			path.LineTo(fishHeadX + mFishWidth * 0.5f, fishHeadY + mFishHeight);
			path.LineTo(fishHeadX + mFishWidth * 0.5f, fishHeadY + mFishHeight * 0.8f);
			path.LineTo(fishHeadX + mFishWidth * 0.333f, fishHeadY + mFishHeight * 0.666f);
			path.LineTo(fishHeadX + mFishWidth * 0.333f, fishHeadY + mFishHeight * 0.444f);
			path.QuadTo(fishHeadX + mFishWidth * 0.333f, fishHeadY + mFishHeight * 0.222f, fishHeadX, fishHeadY);

			path.Close();

			return path;
		}

		private Path createRiverPath(RectF arcBounds)
		{
			if (mRiverPath != null)
			{
				return mRiverPath;
			}

			mRiverPath = new Path();

			RectF rectF = new RectF(arcBounds.CenterX() - mRiverWidth / 2.0f, arcBounds.CenterY() - mRiverHeight / 2.0f, arcBounds.CenterX() + mRiverWidth / 2.0f, arcBounds.CenterY() + mRiverHeight / 2.0f);

			rectF.Inset(mRiverBankWidth / 2.0f, mRiverBankWidth / 2.0f);

			mRiverPath.AddRect(rectF, Path.Direction.Cw);

			return mRiverPath;
		}

		private class FishInterpolator : Java.Lang.Object,Interpolator
		{
			private readonly FishLoadingRenderer outerInstance;

			public FishInterpolator(FishLoadingRenderer outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public  float GetInterpolation(float input)
			{
				int index = ((int)(input / outerInstance.FISH_MOVE_POINTS_RATE));
				if (index >= outerInstance.FISH_MOVE_POINTS.Length)
				{
					index = outerInstance.FISH_MOVE_POINTS.Length - 1;
				}

				return outerInstance.FISH_MOVE_POINTS[index];
			}
		}

		public class Builder
		{
			internal Context mContext;

			public Builder(Context mContext)
			{
				this.mContext = mContext;
			}

			public virtual FishLoadingRenderer build()
			{
				FishLoadingRenderer loadingRenderer = new FishLoadingRenderer(mContext);
				return loadingRenderer;
			}
		}
	}

}