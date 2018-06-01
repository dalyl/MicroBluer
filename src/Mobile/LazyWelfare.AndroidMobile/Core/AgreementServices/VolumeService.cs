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
using LazyWelfare.AndroidAreaView.Core;
using LazyWelfare.AndroidAreaView.Core.Renderer;
using LazyWelfare.Interface;

namespace LazyWelfare.AndroidMobile.AgreementServices
{
    public class VolumeService
    {

        public MapCtrlDialog View { get; }

        public TryCatch Try { get; }

        public decimal Current { get; private set; }

        public VolumeService(Context context)
        {
            View = new MapCtrlDialog(context)
            {
                Render = new VolumeControllerRenderer(context)
                {
                    OnClickTop = () => SetValue(0.1m),
                    OnClickBottom = () => SetValue(-0.1m),
                }
            };
            Current = GetValue();
        }

        decimal GetValue()
        {

            return 0;
        }

        void SetValue(decimal value)
        {
            Current += value;

        }

        public bool Show()
        {
            if (View != null) View.Show();
            return true;
        }
    }
}