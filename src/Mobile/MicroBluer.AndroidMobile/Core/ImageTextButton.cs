using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MicroBluer.AndroidMobile
{
    public class ImageTextButton : RelativeLayout
    {

        private ImageView imgView;
        private TextView textView;

        public ImageTextButton(Context context) : this(context, null)
        {

        }

        public ImageTextButton(Context context, IAttributeSet attributeSet) : base(context, attributeSet)
        {
            //LayoutInflater.From(context).Inflate(Resource.Layout.ImageTextButton, this, true);
            //this.imgView = (ImageView)FindViewById(Resource.Id.imgview);
            //this.textView = (TextView)FindViewById(Resource.Id.textview);
            //this.Clickable = true;
            //this.Focusable = true;
        }
         

        public void SetImgResource(int resourceID)
        {
            this.imgView.SetImageResource(resourceID);
        }

        public void SetText(string text)
        {
            var content = text.ToArray();
            this.textView.SetText(content, 0, content.Length);
        }

        public void SetTextColor(Color color)
        {
            this.textView.SetTextColor(color);
        }

        public void SetTextSize(float size)
        {
            this.textView.SetTextSize(ComplexUnitType.Pt, size);
        }

    }
}