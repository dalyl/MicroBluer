namespace MicroBluer.AndroidCtrls.FileExpler
{
    using System;
    using System.Collections.Generic;
    using Android.App;
    using Android.OS;
    using Java.IO;
    using MicroBluer.AndroidUtils;
    using MicroBluer.AndroidUtils.IO;

    public class FileSizeAsyncTaskCollection : List<FileSizeAsyncTask>
    {
        public void Add(ExplerAdapter adater, ExplerItem item, int position)
        {
            var one = new FileSizeAsyncTask(adater, item, position);
            this.Add(one);
            one.Owner = this;
            one.Execute();
        }

        public void Add(ExplerAdapter adater, List<ExplerItem> items)
        {
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                Add(adater, item, i);
            }
        }

        public new void Clear()
        {
            this.Cancel(true);
            base.Clear();
        }

        public void Cancel(bool mayInterruptIfRunning = true)
        {
            this.ForEach(it => it.Cancel(mayInterruptIfRunning));
        }
    }

    public class FileSizeAsyncTask : AsyncTask
    {
        public FileSizeAsyncTaskCollection Owner { get; set; }

        protected virtual Action _invoke { get; }
        protected virtual Action _before { get; }
        protected virtual Action _after { get; }

        public ExplerAdapter Adapter { get; }
        public ExplerItem Item { get; }
        public int Position { get; }

        public FileSizeAsyncTask(ExplerAdapter adater,ExplerItem item, int position)
        {
            Adapter = adater;
            Item = item;
            Position = position;
        }

        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            TryCatch.Current.Invoke(ComputerSize);
            return true;
        }

        void ComputerSize()
        {
            var file = new File(Item.FullName);
            Item.Size = file.GetFileSize(Adapter.Extensions);
        }

        protected override void OnPreExecute()
        {
            base.OnPreExecute();
        }

        protected override void OnPostExecute(Java.Lang.Object result)
        {
            base.OnPostExecute(result);
            Adapter.NotifyItemChanged(Position);
            if (Owner != null && Owner.Contains(this)) Owner.Remove(this);
        }

        protected override void OnProgressUpdate(params Java.Lang.Object[] values)
        {
            if (IsCancelled)
            {
                return;
            }
        }
    }

}