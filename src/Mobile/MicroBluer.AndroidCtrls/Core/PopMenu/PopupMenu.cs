namespace MicroBluer.AndroidCtrls.PopMenu
{
    using System;
    using System.Collections.Generic;
    using Android.App;
    using Android.Content;
    using Android.Graphics;
    using Android.Graphics.Drawables;
    using Android.Support.V7.Widget;
    using Android.Views;
    using Android.Widget;

    /// <summary>
    /// Used PopupWindow样式的菜单项列表
    /// </summary>
    public class PopupMenu
    {

        private static readonly string TAG = typeof(PopupMenu).Name;

        /// <summary>
        /// 菜单的弹出窗口 
        /// </summary>
        private static PopupWindow popupWindow = null;

        /// <summary>
        /// 显示popupWindow弹出框
        /// </summary>
        public static void ShowPopupWindows(Context context, View spinnerview, List<PopupMenuItem> mArrayList, PopupMenuSetting setting)
        {

            if (popupWindow != null)
            {
                if (popupWindow.IsShowing)
                {
                    popupWindow.Dismiss();
                    popupWindow = null;
                }
            }
            //一个自定义的布局，作为显示的内容
            View popupWindowView = LayoutInflater.From(context).Inflate(Resource.Layout.PopupMenuLayout, null);

            /// <summary>
            /// 在初始化contentView的时候，强制绘制contentView，并且马上初始化contentView的尺寸。
            /// 另外一个点需要注意：popwindow_layout.xml的根Layout必须为LinearLayout；如果为RelativeLayout的话，会导致程序崩溃。
            /// </summary>
            popupWindowView.Measure((int)MeasureSpecMode.Unspecified, (int)MeasureSpecMode.Unspecified);
            //用于获取单个列表项的高度【用于计算大于n个列表项的时候，列表的总高度值n * listitemView.getMeasuredHeight()】
            View listitemView = LayoutInflater.From(context).Inflate(Resource.Layout.PopupMenuItem, null);
            listitemView.Measure((int)MeasureSpecMode.Unspecified, (int)MeasureSpecMode.Unspecified);

            //列表
            RecyclerView mListView = (RecyclerView)popupWindowView.FindViewById(Resource.Id.menu_list);
            //设置布局管理器
            LinearLayoutManager linerLayoutManager = new LinearLayoutManager(context);
            mListView.SetLayoutManager (linerLayoutManager);

            PopupWindowMenuListAdapter mPopupMenuAdapter = new PopupWindowMenuListAdapter(context, mArrayList);

            mListView.SetAdapter (mPopupMenuAdapter);
            mPopupMenuAdapter.SetOnItemClickLitener(new AnonymousItemClickLitener(position => mArrayList[position].Click()));

            //实例化PopupWindow【宽度为自身宽度，高度为自身高度】
            //popupWindow = new PopupWindow(popupWindowView, ViewGroup.LayoutParams.WRAP_CONTENT, ViewGroup.LayoutParams.WRAP_CONTENT);
            //设置当大于6个列表项的时候，设置列表总高度值为6个列表项的高度值
            if (mArrayList.Count <= 6)
            {
                popupWindow = new PopupWindow(popupWindowView, ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                //popupWindow = new PopupWindow(popupWindowView, spinnerview.getMeasuredWidth(), ViewGroup.LayoutParams.WRAP_CONTENT);//spinnerview.getMeasuredWidth()用于设置跟依附的控件的宽度相等
            }
            else
            {
                popupWindow = new PopupWindow(popupWindowView, ViewGroup.LayoutParams.WrapContent, listitemView.MeasuredHeight * 6);
                //popupWindow = new PopupWindow(popupWindowView, spinnerview.getMeasuredWidth(), listitemView.getMeasuredHeight() * 6);//spinnerview.getMeasuredWidth()用于设置跟依附的控件的宽度相等
            }

            popupWindow.Touchable = true; //设置可以触摸
            popupWindow.Focusable = true; //代表可以允许获取焦点的，如果有输入框的话，可以聚焦

            //监听popWindow的隐藏时执行的操作--这个不错
            popupWindow.SetOnDismissListener (new AnonymousOnDismissListener());

            //下面两个参数是实现点击点击外面隐藏popupwindow的
            //这里设置显示PopuWindow之后在外面点击是否有效。如果为false的话，那么点击PopuWindow外面并不会关闭PopuWindow。当然这里很明显只能在Touchable下才能使用。不设置此项则下面的捕获window外touch事件就无法触发。
            popupWindow.OutsideTouchable = true;

            // 如果不设置PopupWindow的背景，无论是点击外部区域还是Back键都无法dismiss弹框
            // 我觉得这里是API的一个bug
            //方式一
             
            ColorDrawable dw = new ColorDrawable(new Color(0x00000000)); //设置背景为透明
            popupWindow.SetBackgroundDrawable(dw);

            //int xPos = - popupWindow.getWidth() / 2 + view.getWidth() / 2;//X轴的偏移值:xoff表示x轴的偏移，正值表示向右，负值表示向左；
            int xPos = 0; //X轴的偏移值:xoff表示x轴的偏移，正值表示向左，负值表示向右；
            int yPos = 0; //Y轴的偏移值相对某个控件的位置，有偏移;yoff表示相对y轴的偏移，正值是向下，负值是向上；

            //=======展现在控件的下方
            //相对于当前view进行位置设置
            popupWindow.ShowAsDropDown(spinnerview, xPos, yPos);
        }

        private class AnonymousItemClickLitener : Java.Lang.Object,PopupWindowMenuListAdapter.IOnItemClickLitener
        {
            Action<int> ItemClick { get; }
            public AnonymousItemClickLitener(Action<int> click)
            {
                ItemClick = click;
            }

            public  void OnItemClick(int position)
            {
                PopupMenu.ClosePopupWindows(); //关闭列表对话框，注意会执行onDismiss()方法
                ItemClick(position);
            }
        }

        private class AnonymousOnDismissListener :Java.Lang.Object, PopupWindow.IOnDismissListener
        {
            Action Dismiss { get; }

            public AnonymousOnDismissListener()
            {
            }

            public  void OnDismiss()
            {
                //执行还原原始状态的操作，比如选中状态颜色高亮显示[去除],不能使用NotifyDataSetInvalidated()，否则会出现popwindow显示错位的情况

            }
        }

        /// <summary>
        /// 关闭菜单弹出框
        /// </summary>
        public static void ClosePopupWindows()
        {
            if (popupWindow != null)
            {
                if (popupWindow.IsShowing)
                {
                    popupWindow.Dismiss();
                    popupWindow = null;
                }
            }
        }

        /// <summary>
        /// Android中Popwindow使用以及背景色变灰【不好用】
        /// 【有问题，首先在DialogFragment中无法实现这个效果，只能在activity和fragment中调用
        /// 其次，调用这个代码后，打开其他的对话框的时候灰色背景会出现突然闪现的问题】
        /// https://blog.csdn.net/zz6880817/article/details/52189699
        /// https://blog.csdn.net/chenbing81/article/details/52059979
        /// </summary>

        /// <summary>
        /// 改变背景颜色 
        /// </summary>
        /// <param name="bgAlpha"> 屏幕透明度0.0-1.0 1表示完全不透明 </param>
        private static void SetBackgroundAlpha(Context context, float? bgAlpha)
        {
            var lp = ((Activity)context).Window.Attributes;
            lp.Alpha = bgAlpha.Value;
            ((Activity)context).Window.AddFlags( WindowManagerFlags.DimBehind);
            ((Activity)context).Window.Attributes = lp;
        }
    }

}