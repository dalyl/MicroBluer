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

namespace LazyWelfare.AndroidMobile
{
    public class TryCatch
    {
        Action<string> ShowMessage { get; set; }

        public TryCatch(Action<string> showMessage = null)
        {
            ShowMessage = showMessage;
        }

        public void Show(string message)
        {
            ShowMessage?.Invoke(message);
        }

        public void Throw(string message)
        {
            throw (new TryCatchException(message));
        }

        public void Invoke(Action invoke)
        {
            try
            {
                invoke();
            }
            catch (TryCatchException ex)
            {
                Show(ex.Message);
            }
            catch (Exception ex)
            {
                Show(ex.Message);
            }
        }
        public T Show<T>(T defaultResult, string message)
        {
            ShowMessage?.Invoke(message);
            return defaultResult;
        }
        public T Throw<T>(string message)
        {
            throw (new TryCatchException(message));
        }

        public T Invoke<T>(T exceptionValue, Func<T> invoke)
        {
            try
            {
                return invoke();
            }
            catch (TryCatchException ex)
            {
                return Show(exceptionValue, ex.Message);
            }
            catch (Exception ex)
            {
                return Show(exceptionValue, ex.Message);
            }
        }

    }
}