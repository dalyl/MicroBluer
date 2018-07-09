namespace MicroBluer.Common
{
    using System;


    public class TryNotice 
    {

        public static readonly TryNotice Current = new TryNotice();

        public static void InitCurrent(Action<string> showMessage = null)
        {
            Current.Setting.ShowMessage = showMessage;
        }

        public static void InitCurrent(TryNoticeSetting setting)
        {
            Current.Setting.Reset(setting);
        }

        public TryNotice(Action<string> showMessage = null) {
            Setting = new TryNoticeSetting
            {
                ShowMessage = showMessage
            };
        }

        public TryNotice(TryNoticeSetting setting) { Setting = setting ?? new TryNoticeSetting(); }

        public TryNoticeSetting Setting { get; }


        public void Show(string message) => Setting.Show(string.Empty, message);

        public void Throw(string message)
        {
            throw (new TryCatchException(message));
        }

        public virtual void Invoke(Action invoke, string message = "")
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
                Show(string.IsNullOrEmpty(message) ? ex.Message : message);
            }
        }

        public T Show<T>(T defaultResult, string message)
        {
            Show(message);
            return defaultResult;
        }

        public bool Show(bool defaultResult, string message, string failMessage)
        {
            if (defaultResult)
                Show(message);
            else
                Show(failMessage);
            return defaultResult;
        }

        public bool Show(Func<bool> invoke, string message, string failMessage)
        {
            var defaultResult = Invoke(false, invoke);
            if (defaultResult)
                Show(message);
            else
                Show(failMessage);
            return defaultResult;
        }

        public T Throw<T>(string message)
        {
            throw (new TryCatchException(message));
        }

        public virtual T Invoke<T>(T exceptionValue, Func<T> invoke, string message = "")
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
                return Show(exceptionValue, string.IsNullOrEmpty(message) ? ex.Message : message);
            }
            finally
            {
            }
        }

    }
}