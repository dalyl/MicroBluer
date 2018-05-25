using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace LazyWelfare.AndroidMobile
{

    public class TryCatchTrust : TryCatch
    {

        Action Begin { get; }

        Action End { get; }

        public TryCatchTrust(Action<string> showMessage = null, Action begin = null, Action end = null) : base(showMessage)
        {
            Begin = begin;
            End = end;
        }

        public override void Invoke(Action invoke, string message = "")
        {
            base.Invoke(() => Begin?.Invoke());
            base.Invoke(invoke, message);
            base.Invoke(() => End?.Invoke());
        }

        public override T Invoke<T>(T exceptionValue, Func<T> invoke, string message = "")
        {
            try
            {
                base.Invoke(() => Begin?.Invoke());
                return base.Invoke(exceptionValue, invoke, message);
            }
            finally
            {
                base.Invoke(() => End?.Invoke());
            }
        }
    }
}