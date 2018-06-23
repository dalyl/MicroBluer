namespace MicroBluer.AndroidMobile.Views
{
    using System;
    using Android.OS;
    using MicroBluer.AndroidUtils;

    public class PartialBackgroudWorkerAsyncTask : AsyncTask
    {
        protected PartialActivity _activity { get; }

        protected virtual Action _invoke { get; }
        protected virtual Action _before { get; }
        protected virtual Action _after { get; }

        public PartialBackgroudWorkerAsyncTask(PartialActivity viewActivity)
        {
            _activity = viewActivity;
        }


        public PartialBackgroudWorkerAsyncTask(PartialActivity viewActivity, Action invoke, Action before = null, Action after = null)
        {
            _activity = viewActivity;
            _invoke = invoke;
            _before = before;
            _after = after;
        }

        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            TryCatch.Current.Invoke(_invoke);
            return true;
        }

        protected override void OnPreExecute()
        {
            base.OnPreExecute();
            _activity.ShowMaskLayer();
            _before?.Invoke();
        }


        protected override void OnPostExecute(Java.Lang.Object result)
        {
            base.OnPostExecute(result);
            _after?.Invoke();
            _activity.HideMaskLayer();
        }
    }

}