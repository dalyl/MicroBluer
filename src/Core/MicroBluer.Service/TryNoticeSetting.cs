namespace MicroBluer.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TryNoticeSetting
    {

        public void Reset(TryNoticeSetting setting)
        {

        }

        public Action<string> ShowFailMessage { private get; set; }

        public Action<string> ShowSuccessMessage { private get; set; }

        public Action<string, string> ShowInfo { private get; set; }

        public Action<string> ShowMessage { private get; set; }

        public Action<Exception> ExceptionHandler { get; set; }


        public void ExceptionInvoke(Exception ex)
        {
            if (ExceptionHandler == null)
            {
                ShowMessage(ex.Message);
            }
            else
            {
                ExceptionHandler(ex);
            }
        }

        public void Show(string tip, string title, string message)
        {

        }

        public void Show(string title, string message)
        {
            if (ShowInfo == null)
            {
                ShowMessage?.Invoke(message);
            }
            else
            {
                ShowInfo(title, message);
            }

        }

    }
}
