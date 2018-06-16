namespace LazyWelfare.AndroidCtrls.PopMenu
{
    using System.Collections.Generic;
    using Android.Content;
    using Android.Views;
    using Android.Widget;
    using Android.Support.V7.Widget;
    using LazyWelfare.AndroidUtils.Views;

    /// <summary>
    /// Used 菜单项列表适配器
    /// </summary>
    public class PopupWindowMenuListAdapter : RecyclerView.Adapter
    {
        /// <summary>
        ///上下文 </summary>
        private Context myContext;
        /// <summary>
        ///自定义列表项标题集合 </summary>
        private List<PopupMenuItem> itemList;
        /// <summary>
        /// 构造函数
        /// </summary>
        public PopupWindowMenuListAdapter(Context context, List<PopupMenuItem> itemlist)
        {
            myContext = context;
            this.itemList = itemlist;
        }
        /// <summary>
        /// 获取总的条目数
        /// </summary>
        public override int ItemCount
        {
            get
            {
                return itemList.Count;
            }
        }
        /// <summary>
        /// 创建ViewHolder
        /// </summary>
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(myContext).Inflate(Resource.Layout.PopupMenuItem, parent, false);
            ItemViewHolder itemViewHolder = new ItemViewHolder(view);
            return itemViewHolder;
        }

        /// <summary>
        /// 声明grid列表项ViewHolder
        /// </summary>
        internal class ItemViewHolder : RecyclerView.ViewHolder
        {
            public ItemViewHolder(View view):base(view)
            {
                itemLayout = view.FindViewById<LinearLayout>(Resource.Id.PopupMenuItem_Layout);
                itemImg = view.FindViewById<ImageView>(Resource.Id.PopupMenuItem_Icon);
                itemText = view.FindViewById<TextView>(Resource.Id.PopupMenuItem_Text);
                itemLine = view.FindViewById(Resource.Id.PopupMenuItem_Line);
            }

            internal LinearLayout itemLayout;
            internal ImageView itemImg;
            internal TextView itemText;
            internal View itemLine;
        }
        /// <summary>
        /// 将数据绑定至ViewHolder
        /// </summary>
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int index)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ItemViewHolder itemViewHold = ((ItemViewHolder)viewHolder);

            //判断属于列表项还是上拉加载区域
            if (viewHolder is ItemViewHolder itemViewHold)
            {
                if (itemList[index].ItemImage != 0)
                {
                    itemViewHold.itemImg.SetImageResource(itemList[index].ItemImage); //赋值图标
                }
                else
                {
                    itemViewHold.itemImg.Visibility = ViewStates.Gone; //如果没有res的id值，则隐藏图标
                }
                itemViewHold.itemText.Text = itemList[index].ItemText; //赋值文本

                if (index == itemList.Count - 1)
                {
                    itemViewHold.itemLine.Visibility = ViewStates.Gone; //隐藏最后一行的底部横线
                }
                //如果设置了回调，则设置点击事件
                if (mOnItemClickLitener != null)
                {
                    itemViewHold.itemLayout.SetOnClickListener(new AnonymousOnClickListener(view =>
                    {
                        int position = itemViewHold.LayoutPosition; //在增加数据或者减少数据时候，position和index就不一样了
                        this.mOnItemClickLitener.OnItemClick(position);
                    }));
                }
            }
        }

        /// <summary>
        /// 添加Item--用于动画的展现
        /// </summary>
        public virtual void AddItem(int position, PopupMenuItem itemModel)
        {
            itemList.Insert(position, itemModel);
            NotifyItemInserted(position);
        }
        /// <summary>
        /// 删除Item--用于动画的展现
        /// </summary>
        public virtual void RemoveItem(int position)
        {
            itemList.RemoveAt(position);
            NotifyItemInserted(position);
        }

        /*=====================添加OnItemClickListener回调================================*/
        public interface IOnItemClickLitener
        {
            void OnItemClick(int position);
        }

        private IOnItemClickLitener mOnItemClickLitener;

        public virtual void SetOnItemClickLitener(IOnItemClickLitener mOnItemClickLitener)
        {
            this.mOnItemClickLitener = mOnItemClickLitener;
        }

    }

}