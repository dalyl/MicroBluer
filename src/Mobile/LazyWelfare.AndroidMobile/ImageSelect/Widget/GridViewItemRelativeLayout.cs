using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LazyWelfare.AndroidMobile.ImageSelect.Control;
using LazyWelfare.AndroidMobile.ImageSelect.Model;
using static Android.Views.View;

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

    //using DisplayImageOptions = com.nostra13.universalimageloader.core.DisplayImageOptions;
    //using ImageLoader = com.nostra13.universalimageloader.core.ImageLoader;
    //using ImageScaleType = com.nostra13.universalimageloader.core.assist.ImageScaleType;

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
            this.imageView.SetOnClickListener (new OnClickListenerAnonymousInnerClass(this));
        }

        private class OnClickListenerAnonymousInnerClass :Java.Lang.Object, IOnClickListener
        {
            private readonly GridViewItemRelativeLayout outerInstance;

            public OnClickListenerAnonymousInnerClass(GridViewItemRelativeLayout outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public  void OnClick(View v)
            {
                var a = outerInstance.mCollection.IsCountOver();
                var b = !outerInstance.mCollection.IsSelected(outerInstance.item.BuildContentUri());
                if (a && b)
                {
                    return;
                }

                if (outerInstance.item.Capture)
                {
                    ((ImageSelectActivity)v.Context).ShowCameraAction();
                    return;
                }
                else if (outerInstance.mCollection.IsSingleChoose)
                {
                    outerInstance.mCollection.Add(outerInstance.item.BuildContentUri());
                    ((ImageSelectActivity)v.Context).SetResult();
                    return;
                }
                if (outerInstance.mCollection.IsSelected(outerInstance.item.BuildContentUri()))
                {
                    outerInstance.mCollection.Remove(outerInstance.item.BuildContentUri());
                    outerInstance.imageCheck.SetImageResource (Resource.Drawable.pick_photo_checkbox_normal);
                    outerInstance.imageView.ClearColorFilter();
                }
                else
                {
                    outerInstance.mCollection.Add(outerInstance.item.BuildContentUri());
                    outerInstance.imageCheck.SetImageResource(Resource.Drawable.pick_photo_checkbox_check);
                    outerInstance.imageView.SetColorFilter(Color.Gray, PorterDuff.Mode.Multiply);
                }
            }
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
                disPlay();
            }
        }

        private void disPlay()
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