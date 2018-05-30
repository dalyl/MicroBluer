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
            roundMenu.onClickListener = new OnClickListenerAnonymousInnerClass(Context,roundMenu);
            roundMenuView.AddRoundMenu(roundMenu);

            roundMenu = new RoundMenu
            {
                selectSolidColor = Context.GetColor(Resource.Color.gray_9999),
                strokeColor = Context.GetColor(Resource.Color.gray_9999),
                icon = Context.Drawable2Bitmap(Resource.Drawable.ic_right)
            };
            roundMenu.onClickListener = new OnClickListenerAnonymousInnerClass2(Context, roundMenu);
            roundMenuView.AddRoundMenu(roundMenu);

            roundMenu = new RoundMenu
            {
                selectSolidColor = Context.GetColor(Resource.Color.gray_9999),
                strokeColor = Context.GetColor(Resource.Color.gray_9999),
                icon = Context.Drawable2Bitmap(Resource.Drawable.ic_right)
            };
            roundMenu.onClickListener = new OnClickListenerAnonymousInnerClass3(Context, roundMenu);
            roundMenuView.AddRoundMenu(roundMenu);

            roundMenu = new RoundMenu
            {
                selectSolidColor = Context.GetColor(Resource.Color.gray_9999),
                strokeColor = Context.GetColor(Resource.Color.gray_9999),
                icon = Context.Drawable2Bitmap( Resource.Drawable.ic_right)
            };
            roundMenu.onClickListener = new OnClickListenerAnonymousInnerClass4(Context, roundMenu);
            roundMenuView.AddRoundMenu(roundMenu);

            roundMenuView.SetCoreMenu( Context.GetColor( Resource.Color.gray_f2f2), 
                Context.GetColor( Resource.Color.gray_9999),
                Context.GetColor( Resource.Color.gray_9999), 1, 0.43,
                Context.Drawable2Bitmap( Resource.Drawable.ic_ok), 
                new OnClickListenerAnonymousInnerClass5(Context, roundMenu));

        }

        private class OnClickListenerAnonymousInnerClass :Java.Lang.Object, View.IOnClickListener
        {
            private readonly RoundMenu outerInstance;
            private readonly Context Context;

            public OnClickListenerAnonymousInnerClass(Context context, RoundMenu outerInstance)
            {
                this.Context = context;
                this.outerInstance = outerInstance;
            }

            public  void OnClick(View view)
            {
                Toast.MakeText(Context, "点击了1", ToastLength.Short).Show();
            }
        }

        private class OnClickListenerAnonymousInnerClass2 : Java.Lang.Object, View.IOnClickListener
        {
            private readonly RoundMenu outerInstance;
            private readonly Context Context;

            public OnClickListenerAnonymousInnerClass2(Context context, RoundMenu outerInstance)
            {
                this.Context = context;
                this.outerInstance = outerInstance;
            }

            public  void OnClick(View view)
            {
                Toast.MakeText(Context, "点击了2", ToastLength.Short).Show();
            }
        }

        private class OnClickListenerAnonymousInnerClass3 : Java.Lang.Object, View.IOnClickListener
        {
            private readonly RoundMenu outerInstance;
            private readonly Context Context;

            public OnClickListenerAnonymousInnerClass3(Context context, RoundMenu outerInstance)
            {
                this.Context = context;
                this.outerInstance = outerInstance;
            }

            public  void OnClick(View view)
            {
                Toast.MakeText(Context, "点击了3", ToastLength.Short).Show();
            }
        }

        private class OnClickListenerAnonymousInnerClass4 : Java.Lang.Object, View.IOnClickListener
        {
            private readonly RoundMenu outerInstance;
            private readonly Context Context;

            public OnClickListenerAnonymousInnerClass4(Context context, RoundMenu outerInstance)
            {
                this.Context = context;
                this.outerInstance = outerInstance;
            }

            public  void OnClick(View view)
            {
                Toast.MakeText(Context, "点击了4", ToastLength.Short).Show();
            }
        }

        private class OnClickListenerAnonymousInnerClass5 : Java.Lang.Object, View.IOnClickListener
        {
            private readonly RoundMenu outerInstance;
            private readonly Context Context;

            public OnClickListenerAnonymousInnerClass5(Context context, RoundMenu outerInstance)
            {
                this.Context = context;
                this.outerInstance = outerInstance;
            }

            public  void OnClick(View view)
            {
                Toast.MakeText(Context, "点击了中心圆圈", ToastLength.Short).Show();

            }
        }
    }
}
