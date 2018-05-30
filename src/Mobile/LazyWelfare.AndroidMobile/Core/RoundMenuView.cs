

namespace LazyWelfare.AndroidMobile.Core
{
    using Context = Android.Content.Context;
    using Bitmap = Android.Graphics.Bitmap;
    using Canvas = Android.Graphics.Canvas;
    using Matrix = Android.Graphics.Matrix;
    using Paint = Android.Graphics.Paint;
    using RectF = Android.Graphics.RectF;
    using IAttributeSet = Android.Util.IAttributeSet;
    using MotionEvent = Android.Views.MotionEvent;
    using View = Android.Views.View;
    using System;
    using System.Collections.Generic;
    using Android.Graphics;
    using Android.Views;

    /// <summary>
    /// 仿遥控器上下左右ok圆形菜单
    ///</summary>
    public class RoundMenuView : View
    {

        /// <summary>
        /// 变量
        /// </summary>
        private int coreX; //中心点的坐标X
        private int coreY; //中心点的坐标Y

        private IList<RoundMenu> roundMenus; //菜单列表
        private bool isCoreMenu = false; //是否有中心按钮
        private int coreMenuColor; //中心按钮的默认背景--最好不要透明色
        private int coreMenuStrokeColor; //中心按钮描边颜色
        private int coreMenuStrokeSize; //中心按钮描边粗细
        private int coreMenuSelectColor; //中心按钮选中时的背景颜色
        private Bitmap coreBitmap; //OK图片
        private IOnClickListener onCoreClickListener; //中心按钮的点击回调

        private float deviationDegree; //偏移角度
        private int onClickState = -2; //-2是无点击，-1是点击中心圆，其他是点击菜单
        private int roundRadius; //中心圆的半径
        private double radiusDistance; //半径的长度比（中心圆半径=大圆半径*radiusDistance）
        private long touchTime; //按下时间，抬起的时候判定一下，超过300毫秒算点击

        public RoundMenuView(Context context) : base(context)
        {
        }

        public RoundMenuView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public RoundMenuView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
        }

        protected  override void OnDraw(Canvas canvas)
        {
            coreX = Width / 2;
            coreY = Height / 2;
            roundRadius = (int)(Width / 2 * radiusDistance); //计算中心圆圈半径

            RectF rect = new RectF(0, 0, Width, Height);
            if (roundMenus != null && roundMenus.Count > 0)
            {
                float sweepAngle = 360 / roundMenus.Count; //每个弧形的角度
                deviationDegree = sweepAngle / 2; //其实的偏移角度，如果4个扇形的时候是X形状，而非+,设为0试试就知道什么意思了
                for (int i = 0; i < roundMenus.Count; i++)
                {
                    RoundMenu roundMenu = roundMenus[i];
                    //填充
                    Paint paint = new Paint();
                    paint.AntiAlias = true;
                    if (onClickState == i)
                    {
                        //选中
                        paint.Color = new Color(roundMenu.selectSolidColor);
                    }
                    else
                    {
                        //未选中
                        paint.Color =new Color(roundMenu.solidColor);
                    }
                    canvas.DrawArc(rect, deviationDegree + (i * sweepAngle), sweepAngle, true, paint);

                    //画描边
                    paint = new Paint();
                    paint.AntiAlias = true;
                    paint.StrokeWidth = roundMenu.strokeSize;
                    paint.SetStyle (Paint.Style.Stroke);
                    paint.Color = new Color (roundMenu.strokeColor);
                    canvas.DrawArc(rect, deviationDegree + (i * sweepAngle), sweepAngle, roundMenu.useCenter, paint);

                    //画图案
                    Matrix matrix = new Matrix();
                    matrix.PostTranslate((float)((coreX + Width / 2 * roundMenu.iconDistance) - (roundMenu.icon.Width / 2)), coreY - (roundMenu.icon.Height / 2));
                    matrix.PostRotate(((i + 1) * sweepAngle), coreX, coreY);
                    canvas.DrawBitmap(roundMenu.icon, matrix, null);
                }
            }

            //画中心圆圈
            if (isCoreMenu)
            {
                //填充
                RectF rect1 = new RectF(coreX - roundRadius, coreY - roundRadius, coreX + roundRadius, coreY + roundRadius);
                Paint paint = new Paint();
                paint.AntiAlias = true;
                paint.StrokeWidth = coreMenuStrokeSize;
                if (onClickState == -1)
                {
                    paint.Color = new Color (coreMenuSelectColor);
                }
                else
                {
                    paint.Color = new Color (coreMenuColor);
                }
                canvas.DrawArc(rect1, 0, 360, true, paint);

                //画描边
                paint = new Paint();
                paint.AntiAlias = true;
                paint.StrokeWidth = coreMenuStrokeSize;
                paint.SetStyle(Paint.Style.Stroke);
                paint.Color = new Color (coreMenuStrokeColor);
                canvas.DrawArc(rect1, 0, 360, true, paint);
                if (coreBitmap != null)
                {
                    //画中心圆圈的"OK"图标
                    canvas.DrawBitmap(coreBitmap, coreX - coreBitmap.Width / 2, coreY - coreBitmap.Height / 2, null); //在 0，0坐标开始画入src
                }
            }
        }


        public override bool OnTouchEvent(MotionEvent @event)
        {
            switch (@event.Action)
            {
                case  MotionEventActions.Down:
                    touchTime = (DateTime.Now).Ticks;
                    float textX = @event.RawX;
                    float textY = @event.RawY;
                    int distanceLine = (int)GetDisForTwoSpot(coreX, coreY, textX, textY); //距离中心点之间的直线距离
                    if (distanceLine <= roundRadius)
                    {
                        //点击的是中心圆；按下点到中心点的距离小于中心园半径，那就是点击中心园了
                        onClickState = -1;
                    }
                    else if (distanceLine <= Width / 2)
                    {
                        //点击的是某个扇形；按下点到中心点的距离大于中心圆半径小于大圆半径，那就是点击某个扇形了
                        float sweepAngle = 360 / roundMenus.Count; //每个弧形的角度
                        int angle = GetRotationBetweenLines(coreX, coreY, textX, textY);
                        //这个angle的角度是从正Y轴开始，而我们的扇形是从正X轴开始，再加上偏移角度，所以需要计算一下
                        angle = (angle + 360 - 90 - (int)deviationDegree) % 360;
                        onClickState = (int)(angle / sweepAngle); //根据角度得出点击的是那个扇形
                    }
                    else
                    {
                        //点击了外面
                        onClickState = -2;
                    }
                    Invalidate();
                    break;
                case  MotionEventActions.Up:
                    if (((DateTime.Now).Ticks - touchTime) < 300)
                    {
                        //点击小于300毫秒算点击
                        IOnClickListener onClickListener = null;
                        if (onClickState == -1)
                        {
                            onClickListener = onCoreClickListener;
                        }
                        else if (onClickState >= 0 && onClickState < roundMenus.Count)
                        {
                            onClickListener = roundMenus[onClickState].onClickListener;
                        }
                        if (onClickListener != null)
                        {
                            onClickListener.OnClick(this);
                        }
                    }
                    onClickState = -2;
                    Invalidate();
                    break;
            }
            return true;
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="roundMenu"> </param>
        public virtual void AddRoundMenu(RoundMenu roundMenu)
        {
            if (roundMenu == null)
            {
                return;
            }
            if (roundMenus == null)
            {
                roundMenus = new List<RoundMenu>();
            }
            roundMenus.Add(roundMenu);
            Invalidate();
        }

        /// <summary>
        /// 添加中心菜单按钮
        /// </summary>
        /// <param name="coreMenuColor"> </param>
        /// <param name="coreMenuSelectColor"> </param>
        /// <param name="onClickListener"> </param>
        public virtual void SetCoreMenu(int coreMenuColor, int coreMenuSelectColor, int coreMenuStrokeColor, int coreMenuStrokeSize, double radiusDistance, Bitmap bitmap, IOnClickListener onClickListener)
        {
            isCoreMenu = true;
            this.coreMenuColor = coreMenuColor;
            this.radiusDistance = radiusDistance;
            this.coreMenuSelectColor = coreMenuSelectColor;
            this.coreMenuStrokeColor = coreMenuStrokeColor;
            this.coreMenuStrokeSize = coreMenuStrokeSize;
            coreBitmap = bitmap;
            this.onCoreClickListener = onClickListener;
            Invalidate();
        }

        /// <summary>
        /// 获取两条线的夹角
        /// </summary>
        /// <param name="centerX"> </param>
        /// <param name="centerY"> </param>
        /// <param name="xInView"> </param>
        /// <param name="yInView">
        /// @return </param>
        public static int GetRotationBetweenLines(float centerX, float centerY, float xInView, float yInView)
        {
            double rotation = 0;

            double k1 = (double)(centerY - centerY) / (centerX * 2 - centerX);
            double k2 = (double)(yInView - centerY) / (xInView - centerX);
            double tmpDegree = Math.Atan((Math.Abs(k1 - k2)) / (1 + k1 * k2)) / Math.PI * 180;

            if (xInView > centerX && yInView < centerY)
            { //第一象限
                rotation = 90 - tmpDegree;
            }
            else if (xInView > centerX && yInView > centerY) //第二象限
            {
                rotation = 90 + tmpDegree;
            }
            else if (xInView < centerX && yInView > centerY)
            { //第三象限
                rotation = 270 - tmpDegree;
            }
            else if (xInView < centerX && yInView < centerY)
            { //第四象限
                rotation = 270 + tmpDegree;
            }
            else if (xInView == centerX && yInView < centerY)
            {
                rotation = 0;
            }
            else if (xInView == centerX && yInView > centerY)
            {
                rotation = 180;
            }
            return (int)rotation;
        }

        /// <summary>
        /// 求两个点之间的距离
        /// 
        /// @return
        /// </summary>
        public static double GetDisForTwoSpot(float x1, float y1, float x2, float y2)
        {
            float width, height;
            if (x1 > x2)
            {
                width = x1 - x2;
            }
            else
            {
                width = x2 - x1;
            }

            if (y1 > y2)
            {
                height = y2 - y1;
            }
            else
            {
                height = y2 - y1;
            }
            return Math.Sqrt((width * width) + (height * height));
        }

        /// <summary>
        /// 扇形的对象类
        /// </summary>
        public class RoundMenu
        {
            public bool useCenter = true; //扇形是否画连接中心点的直线
            public int solidColor = 0x00000000; //背景颜色,默认透明
            public int selectSolidColor = 0x00000000; //背景颜色,默认透明
            public int strokeColor = 0x00000000; //描边颜色,默认透明
            public int strokeSize = 1; //描边的宽度,默认1
            public Bitmap icon; //菜单的图片
            public IOnClickListener onClickListener; //点击监听
            public double iconDistance = 0.63; //图标距离中心点的距离
        }
    }
}