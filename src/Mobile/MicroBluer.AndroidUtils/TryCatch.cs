namespace MicroBluer.AndroidUtils
{
    using System;
   
    public class TryCatch 
    {

        public static readonly TryCatch Current = new TryCatch();

        public static void InitCurrent(Action<string> showMessage = null)
        {
            Current.ShowMessage = showMessage;
        }

        public TryCatch(Action<string> showMessage = null) { ShowMessage = showMessage; }

        Action<string> ShowMessage { get; set; }
       
        public void Show(string message) => ShowMessage?.Invoke(message);

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