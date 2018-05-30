namespace LazyWelfare.AndroidMobile.ImageSelect.Widget
{
    using Context = Android.Content.Context;
    using Bitmap = Android.Graphics.Bitmap;
    using Color = Android.Graphics.Color;
    using PorterDuff = Android.Graphics.PorterDuff;
    using AttributeSet = Android.Util.IAttributeSet;
    using View = Android.Views.View;
    using ImageView = Android.Widget.ImageView;
    using RelativeLayout = Android.Widget.RelativeLayout;
    using Android.Views;
    using LazyWelfare.AndroidMobile.ImageSelect.Collection;
    using LazyWelfare.AndroidMobile.ImageSelect.Model;
    using LazyWelfare.AndroidUtils.View;

    public class GridViewItemRelativeLayout : RelativeLayout
    {

        private ImageView imageView;
        private ImageView imageCheck;
        private Picture item;
        internal SelectedUriCollection mCollection;

        public GridViewItemRelativeLayout(Context context) : this(context, null)
        {
        }

        public GridViewItemRelativeLayout(Context context, AttributeSet attrs) : this(context, attrs, 0)
        {
        }

        public GridViewItemRelativeLayout(Context context, AttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, widthMeasureSpec);
        }

        public virtual void SetImageView(ImageView imageView, ImageView imageCheck, SelectedUriCollection mCollection)
        {
            this.imageView = imageView;
            this.imageView.SetMinimumWidth(Width);
            this.imageView.SetMinimumHeight(Height);
            this.imageCheck = imageCheck;
            this.mCollection = mCollection;
            this.imageView.SetOnClickListener (new AnonymousOnClickListener(v=> {
                var a = this.mCollection.IsCountOver();
                var b = !this.mCollection.IsSelected(this.item.BuildContentUri());
                if (a && b)
                {
                    return;
                }

                if (this.item.Capture)
                {
                    ((ImageSelectActivity)v.Context).ShowCameraAction();
                    return;
                }
                else if (this.mCollection.IsSingleChoose)
                {
                    this.mCollection.Add(this.item.BuildContentUri());
                    ((ImageSelectActivity)v.Context).SetResult();
                    return;
                }
                if (this.mCollection.IsSelected(this.item.BuildContentUri()))
                {
                    this.mCollection.Remove(this.item.BuildContentUri());
                    this.imageCheck.SetImageResource(Resource.Drawable.pick_photo_checkbox_normal);
                    this.imageView.ClearColorFilter();
                }
                else
                {
                    this.mCollection.Add(this.item.BuildContentUri());
                    this.imageCheck.SetImageResource(Resource.Drawable.pick_photo_checkbox_check);
                    this.imageView.SetColorFilter(Color.Gray, PorterDuff.Mode.Multiply);
                }
            }));
        }

       
        public virtual Picture Item
        {
            set
            {
                this.item = value;
                imageView.ClearColorFilter();
                imageCheck.SetImageResource(Resource.Drawable.pick_photo_checkbox_normal);
                if (mCollection.IsSelected(value.BuildContentUri()))
                {
                    imageView.SetColorFilter(Color.Gray, PorterDuff.Mode.Multiply);
                    imageCheck.SetImageResource(Resource.Drawable.pick_photo_checkbox_check);
                }
                imageCheck.Visibility = mCollection.IsSingleChoose || value.Capture ? ViewStates.Gone : ViewStates.Visible;
                DisPlay();
            }
        }

        private void DisPlay()
        {
            if (item.Capture)
            {
                mCollection.Engine.DisplayCameraItem(imageView);
            }
            else
            {
                mCollection.Engine.DisplayImage(item.BuildContentUri().ToString(), imageView);
            }
        }
    }
}