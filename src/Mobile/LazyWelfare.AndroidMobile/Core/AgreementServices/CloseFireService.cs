

namespace LazyWelfare.AndroidMobile.AgreementServices
{
    using System;
    using Android.App;
    using Android.Content;

    public class CloseFireService: IAgreementService
    {
        public TimePickerDialog View { get; private set; }

        public CloseFireService(Context context)
        {
            View = new TimePickerDialog(context, SetTime, DateTime.Now.Hour, DateTime.Now.Minute, true);
        }

        void SetTime(object sender, TimePickerDialog.TimeSetEventArgs e)
        {

        }

        public bool Execute()
        {
            if (View != null) return ActiveContext.Try.Throw<bool>("控件视图没有正确实例化");
            View.Show();
            return true;
        }
    }

}