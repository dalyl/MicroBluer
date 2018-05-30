using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LazyWelfare.AndroidMobile.Core;
using LazyWelfare.AndroidMobile.Utils;
using LazyWelfare.AndroidUtils.View;

namespace LazyWelfare.AndroidMobile
{
    using RoundMenu = RoundMenuView.RoundMenu;

    public class CtrlDialog : ProgressDialog
    {
        public CtrlDialog(Context context) : base(context)
        {
            Draw();
        }

        public CtrlDialog(Context context, int theme) : base(context, theme)
        {
            Draw();
        }

        public void Draw()
        {
            RoundMenuView roundMenuView = new RoundMenuView(Context);
        
            RoundMenu roundMenu = new RoundMenu
            {
                selectSolidColor = Context.GetColor(Resource.Color.gray_9999),
                strokeColor = Context.GetColor(Resource.Color.gray_9999),
                icon = Context.Drawable2Bitmap(Resource.Drawable.ic_right)
            };
            roundMenu.onClickListener = new AnonymousOnClickListener(v=> Toast.MakeText(Context, "点击了1", ToastLength.Short).Show());
            roundMenuView.AddRoundMenu(roundMenu);

            roundMenu = new RoundMenu
            {
                selectSolidColor = Context.GetColor(Resource.Color.gray_9999),
                strokeColor = Context.GetColor(Resource.Color.gray_9999),
                icon = Context.Drawable2Bitmap(Resource.Drawable.ic_right)
            };
            roundMenu.onClickListener = new AnonymousOnClickListener(v => Toast.MakeText(Context, "点击了2", ToastLength.Short).Show());
            roundMenuView.AddRoundMenu(roundMenu);

            roundMenu = new RoundMenu
            {
                selectSolidColor = Context.GetColor(Resource.Color.gray_9999),
                strokeColor = Context.GetColor(Resource.Color.gray_9999),
                icon = Context.Drawable2Bitmap(Resource.Drawable.ic_right)
            };
            roundMenu.onClickListener = new AnonymousOnClickListener(v => Toast.MakeText(Context, "点击了3", ToastLength.Short).Show());
            roundMenuView.AddRoundMenu(roundMenu);

            roundMenu = new RoundMenu
            {
                selectSolidColor = Context.GetColor(Resource.Color.gray_9999),
                strokeColor = Context.GetColor(Resource.Color.gray_9999),
                icon = Context.Drawable2Bitmap( Resource.Drawable.ic_right)
            };
            roundMenu.onClickListener = new AnonymousOnClickListener(v => Toast.MakeText(Context, "点击了4", ToastLength.Short).Show());
            roundMenuView.AddRoundMenu(roundMenu);

            roundMenuView.SetCoreMenu( Context.GetColor( Resource.Color.gray_f2f2), 
                Context.GetColor( Resource.Color.gray_9999),
                Context.GetColor( Resource.Color.gray_9999), 1, 0.43,
                Context.Drawable2Bitmap( Resource.Drawable.ic_ok), 
                new AnonymousOnClickListener(v => Toast.MakeText(Context, "点击了中心圆圈", ToastLength.Short).Show()));

        }
        

        
    }
}
