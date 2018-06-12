namespace LazyWelfare.AndroidCtrls.LoadingAnimation.Render.Goods
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
    using FastOutSlowInInterpolator = Android.Support.V4.View.Animation.FastOutSlowInInterpolator;
    using LazyWelfare.AndroidUtils.Common;
    using System.Collections.Generic;
    using System;

    public class WaterBottleLoadingRenderer : LoadingRenderer
	{
		private static readonly Interpolator MATERIAL_INTERPOLATOR = new FastOutSlowInInterpolator();

		private const float DEFAULT_Width = 200.0f;
		private const float DEFAULT_Height = 150.0f;
		private const float DEFAULT_STROKE_Width = 1.5f;
		private const float DEFAULT_BOTTLE_Width = 30;
		private const float DEFAULT_BOTTLE_Height = 43;
		private const float WATER_LOWEST_POINT_TO_BOTTLENECK_DISTANCE = 30;

		private const int DEFAULT_WAVE_COUNT = 5;
		private const int DEFAULT_WATER_DROP_COUNT = 25;
		private const int MAX_WATER_DROP_RADIUS = 5;
		private const int MIN_WATER_DROP_RADIUS = 1;

		private static readonly int DEFAULT_BOTTLE_COLOR = Color.ParseColor("#FFDAEBEB");
		private static readonly int DEFAULT_WATER_COLOR = Color.ParseColor("#FF29E3F2");

		private const float DEFAULT_TEXT_SIZE = 7.0f;
		private const string LOADING_TEXT = "loading";
		private const long ANIMATION_DURATION = 11111;

		private readonly Random mRandom = new Random();
		private readonly Paint mPaint = new Paint();
		private readonly RectF mCurrentBounds = new RectF();
		private readonly RectF mBottleBounds = new RectF();
		private readonly RectF mWaterBounds = new RectF();
		private readonly Rect mLoadingBounds = new Rect();
		private readonly IList<WaterDropHolder> mWaterDropHolders = new List<WaterDropHolder>();

		private float mTextSize;
		private float mProgress;
		private float mBottleWidth;
		private float mBottleHeight;
		private float mStrokeWidth;
		private float mWaterLowestPointToBottleneckDistance;

		private int mBottleColor;
		private int mWaterColor;
		private int mWaveCount;

        internal WaterBottleLoadingRenderer(Context context) : base(context)
		{
			Init(context);
			SetupPaint();
		}

		private void Init(Context context)
		{
			mTextSize = DensityUtil.Dip2Px(context, DEFAULT_TEXT_SIZE);

			Width = DensityUtil.Dip2Px(context, DEFAULT_Width);
			Height = DensityUtil.Dip2Px(context, DEFAULT_Height);
			mStrokeWidth = DensityUtil.Dip2Px(context, DEFAULT_STROKE_Width);

			mBottleWidth = DensityUtil.Dip2Px(context, DEFAULT_BOTTLE_Width);
			mBottleHeight = DensityUtil.Dip2Px(context, DEFAULT_BOTTLE_Height);
			mWaterLowestPointToBottleneckDistance = DensityUtil.Dip2Px(context, WATER_LOWEST_POINT_TO_BOTTLENECK_DISTANCE);

			mBottleColor = DEFAULT_BOTTLE_COLOR;
			mWaterColor = DEFAULT_WATER_COLOR;

			mWaveCount = DEFAULT_WAVE_COUNT;

			Duration = ANIMATION_DURATION;
		}

		private void SetupPaint()
		{
			mPaint.AntiAlias = true;
			mPaint.StrokeWidth = mStrokeWidth;
			mPaint.StrokeJoin = Paint.Join.Round;
		}

		protected  override void Draw(Canvas canvas, Rect bounds)
		{
			int saveCount = canvas.Save();

			RectF arcBounds = mCurrentBounds;
			arcBounds.Set(bounds);
			//draw bottle
			mPaint.SetStyle (Paint.Style.Stroke);
			mPaint.Color = new Color (mBottleColor);
			canvas.DrawPath(CreateBottlePath(mBottleBounds), mPaint);

			//draw water
			mPaint.SetStyle (Paint.Style.FillAndStroke);
			mPaint.Color = new Color (mWaterColor);
			canvas.DrawPath(CreateWaterPath(mWaterBounds, mProgress), mPaint);

			//draw water drop
			mPaint.SetStyle(Paint.Style.Fill);
			mPaint.Color = new Color (mWaterColor);
			foreach (WaterDropHolder waterDropHolder in mWaterDropHolders)
			{
				if (waterDropHolder.mNeedDraw)
				{
					canvas.DrawCircle(waterDropHolder.mInitX, waterDropHolder.mCurrentY, waterDropHolder.mRadius, mPaint);
				}
			}

			//draw loading text
			mPaint.Color = new Color (mBottleColor);
			canvas.DrawText(LOADING_TEXT, mBottleBounds.CenterX() - mLoadingBounds.Width() / 2.0f, mBottleBounds.Bottom + mBottleBounds.Height() * 0.2f, mPaint);
			canvas.RestoreToCount(saveCount);
		}

		protected internal override void ComputeRender(float renderProgress)
		{
			if (mCurrentBounds.Width() <= 0)
			{
				return;
			}

			RectF arcBounds = mCurrentBounds;
			//compute gas tube bounds
			mBottleBounds.Set(arcBounds.CenterX() - mBottleWidth / 2.0f, arcBounds.CenterY() - mBottleHeight / 2.0f, arcBounds.CenterX() + mBottleWidth / 2.0f, arcBounds.CenterY() + mBottleHeight / 2.0f);
			//compute pipe body bounds
			mWaterBounds.Set(mBottleBounds.Left + mStrokeWidth * 1.5f, mBottleBounds.Top + mWaterLowestPointToBottleneckDistance, mBottleBounds.Right - mStrokeWidth * 1.5f, mBottleBounds.Bottom - mStrokeWidth * 1.5f);

			//compute wave progress
			float totalWaveProgress = renderProgress * mWaveCount;
			float currentWaveProgress = totalWaveProgress - ((int) totalWaveProgress);

			if (currentWaveProgress > 0.5f)
			{
				mProgress = 1.0f - MATERIAL_INTERPOLATOR.GetInterpolation((currentWaveProgress - 0.5f) * 2.0f);
			}
			else
			{
				mProgress = MATERIAL_INTERPOLATOR.GetInterpolation(currentWaveProgress * 2.0f);
			}

			//init water drop holders
			if (mWaterDropHolders.Count == 0)
			{
				InitWaterDropHolders(mBottleBounds, mWaterBounds);
			}

			//compute the location of these water drops
			foreach (WaterDropHolder waterDropHolder in mWaterDropHolders)
			{
				if (waterDropHolder.mDelayDuration < renderProgress && waterDropHolder.mDelayDuration + waterDropHolder.mDuration > renderProgress)
				{
					float riseProgress = (renderProgress - waterDropHolder.mDelayDuration) / waterDropHolder.mDuration;
					riseProgress = riseProgress < 0.5f ? riseProgress * 2.0f : 1.0f - (riseProgress - 0.5f) * 2.0f;
					waterDropHolder.mCurrentY = waterDropHolder.mInitY - MATERIAL_INTERPOLATOR.GetInterpolation(riseProgress) * waterDropHolder.mRiseHeight;
					waterDropHolder.mNeedDraw = true;
				}
				else
				{
					waterDropHolder.mNeedDraw = false;
				}
			}

			//measure loading text
			mPaint.TextSize = mTextSize;
			mPaint.GetTextBounds(LOADING_TEXT, 0, LOADING_TEXT.Length, mLoadingBounds);
		}

		private Path CreateBottlePath(RectF bottleRect)
		{
			float bottleneckWidth = bottleRect.Width() * 0.3f;
			float bottleneckHeight = bottleRect.Height() * 0.415f;
			float bottleneckDecorationWidth = bottleneckWidth * 1.1f;
			float bottleneckDecorationHeight = bottleneckHeight * 0.167f;

			Path path = new Path();
			//draw the Left side of the bottleneck decoration
			path.MoveTo(bottleRect.CenterX() - bottleneckDecorationWidth * 0.5f, bottleRect.Top);
			path.QuadTo(bottleRect.CenterX() - bottleneckDecorationWidth * 0.5f - bottleneckWidth * 0.15f, bottleRect.Top + bottleneckDecorationHeight * 0.5f, bottleRect.CenterX() - bottleneckWidth * 0.5f, bottleRect.Top + bottleneckDecorationHeight);
			path.LineTo(bottleRect.CenterX() - bottleneckWidth * 0.5f, bottleRect.Top + bottleneckHeight);

			//draw the Left side of the bottle's body
			float radius = (bottleRect.Width() - mStrokeWidth) / 2.0f;
			float CenterY = bottleRect.Bottom - 0.86f * radius;
			RectF bodyRect = new RectF(bottleRect.Left, CenterY - radius, bottleRect.Right, CenterY + radius);
			path.AddArc(bodyRect, 255, -135);

			//draw the Bottom of the bottle
			float bottleBottomWidth = bottleRect.Width() / 2.0f;
			path.LineTo(bottleRect.CenterX() - bottleBottomWidth / 2.0f, bottleRect.Bottom);
			path.LineTo(bottleRect.CenterX() + bottleBottomWidth / 2.0f, bottleRect.Bottom);

			//draw the Right side of the bottle's body
			path.AddArc(bodyRect, 60, -135);

			//draw the Right side of the bottleneck decoration
			path.LineTo(bottleRect.CenterX() + bottleneckWidth * 0.5f, bottleRect.Top + bottleneckDecorationHeight);
			path.QuadTo(bottleRect.CenterX() + bottleneckDecorationWidth * 0.5f + bottleneckWidth * 0.15f, bottleRect.Top + bottleneckDecorationHeight * 0.5f, bottleRect.CenterX() + bottleneckDecorationWidth * 0.5f, bottleRect.Top);

			return path;
		}

		private Path CreateWaterPath(RectF waterRect, float progress)
		{
			Path path = new Path();

			path.MoveTo(waterRect.Left, waterRect.Top);

			//Similar to the way draw the bottle's Bottom sides
			float radius = (waterRect.Width() - mStrokeWidth) / 2.0f;
			float CenterY = waterRect.Bottom - 0.86f * radius;
			float bottleBottomWidth = waterRect.Width() / 2.0f;
			RectF bodyRect = new RectF(waterRect.Left, CenterY - radius, waterRect.Right, CenterY + radius);

			path.AddArc(bodyRect, 187.5f, -67.5f);
			path.LineTo(waterRect.CenterX() - bottleBottomWidth / 2.0f, waterRect.Bottom);
			path.LineTo(waterRect.CenterX() + bottleBottomWidth / 2.0f, waterRect.Bottom);
			path.AddArc(bodyRect, 60, -67.5f);

			//draw the water waves
			float cubicXChangeSize = waterRect.Width() * 0.35f * progress;
			float cubicYChangeSize = waterRect.Height() * 1.2f * progress;

			path.CubicTo(waterRect.Left + waterRect.Width() * 0.80f - cubicXChangeSize, waterRect.Top - waterRect.Height() * 1.2f + cubicYChangeSize, waterRect.Left + waterRect.Width() * 0.55f - cubicXChangeSize, waterRect.Top - cubicYChangeSize, waterRect.Left, waterRect.Top - mStrokeWidth / 2.0f);

			path.LineTo(waterRect.Left, waterRect.Top);

			return path;
		}

		private void InitWaterDropHolders(RectF bottleRect, RectF waterRect)
		{
			float bottleRadius = bottleRect.Width() / 2.0f;
			float lowestWaterPointY = waterRect.Top;
			float twoSidesInterval = 0.2f * bottleRect.Width();
			float atLeastDelayDuration = 0.1f;

			float unitDuration = 0.1f;
			float delayDurationRange = 0.6f;
			int radiusRandomRange = MAX_WATER_DROP_RADIUS - MIN_WATER_DROP_RADIUS;
			float currentXRandomRange = bottleRect.Width() * 0.6f;

			for (int i = 0; i < DEFAULT_WATER_DROP_COUNT; i++)
			{
                WaterDropHolder waterDropHolder = new WaterDropHolder
                {
                    mRadius = MIN_WATER_DROP_RADIUS + mRandom.Next(radiusRandomRange),
                    mInitX = bottleRect.Left + twoSidesInterval + (float)mRandom.NextDouble() * currentXRandomRange
                };
                waterDropHolder.mInitY = lowestWaterPointY + waterDropHolder.mRadius / 2.0f;
				waterDropHolder.mRiseHeight = GetMaxRiseHeight(bottleRadius, waterDropHolder.mRadius, waterDropHolder.mInitX - bottleRect.Left) * (0.2f + 0.8f * (float)mRandom.NextDouble());
				waterDropHolder.mDelayDuration = atLeastDelayDuration + (float)mRandom.NextDouble() * delayDurationRange;
				waterDropHolder.mDuration = waterDropHolder.mRiseHeight / bottleRadius * unitDuration;

				mWaterDropHolders.Add(waterDropHolder);
			}
		}

		private float GetMaxRiseHeight(float bottleRadius, float waterDropRadius, float currentX)
		{
			float coordinateX = currentX - bottleRadius;
			float bottleneckRadius = bottleRadius * 0.3f;
			if (coordinateX - waterDropRadius > -bottleneckRadius && coordinateX + waterDropRadius < bottleneckRadius)
			{
				return bottleRadius * 2.0f;
			}

			return (float)(Math.Sqrt(Math.Pow(bottleRadius, 2.0f) - Math.Pow(coordinateX, 2.0f)) - waterDropRadius);
		}

		protected internal override int Alpha
        {
            set => mPaint.Alpha = value;
        }

        protected internal override ColorFilter ColorFilter
        {
            set => mPaint.SetColorFilter(value);
        }

		private class WaterDropHolder
		{
			public float mCurrentY;
			public float mInitX;
			public float mInitY;
			public float mDelayDuration;
			public float mRiseHeight;
			public float mRadius;
			public float mDuration;
			public bool mNeedDraw;
		}

	}
}