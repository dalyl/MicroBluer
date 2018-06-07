namespace LazyWelfare.AndroidCtrls.Dialogs
{
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Views;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class FolderSelector: AlertDialog
    {
       
        public static event Action OnCanceled;
        public static event Action OnCancelRequested;
        public static event Action<Result> OnSelectorCompleted;

        public FolderSelector(Context context) : base(context)
        {
        }

        public FolderSelector(Context context, int theme) : base(context, theme)
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window.Attributes.Width = ViewGroup.LayoutParams.MatchParent;
            Window.Attributes.Height = ViewGroup.LayoutParams.MatchParent;

            SetCancelable(Cancelable);
            SetCanceledOnTouchOutside(CanceledOnTouchOutside);

            SetContentView(Resource.Layout.FolderSelector);
        }

        /// <summary>
        /// 是否可以取消
        /// </summary>
        public bool Cancelable { get; set; } = false;

        /// <summary>
        /// 是否可以触摸取消
        /// </summary>
        public bool CanceledOnTouchOutside { get; set; } = false;

        public static string SelectSingle(Context context, string older)
        {
            var fetchResult =  OpenDialog(context, older);
            Task.WaitAll(fetchResult);
            var result = fetchResult.Result;
            return result == null ? string.Empty : result.SelectItem;
        }


        public static async Task<string> SelectSingleAsync(Context context,string older)
        {
            var result = await OpenDialog(context, older);
            return result == null ? string.Empty : result.SelectItem;
        }

        public static async Task<string[]> SelectManyAsync(Context context, string[] olders)
        {
            var result = await OpenDialog(context, paths: olders);
            return result == null ? new string[0] : result.SelectItems;
        }

        static Task<Result> OpenDialog(Context context, string path = null, string[] paths = null)
        {
            return Task.Factory.StartNew<Result>(() =>
            {
                Result selected = null;
                var waitSelectedResetEvent = new ManualResetEvent(false);
                var selector = new FolderSelector(context);

                FolderSelector.OnCanceled += () =>
                {
                    selected = new Result { SelectItem = path, SelectItems = paths };
                    waitSelectedResetEvent.Set();
                };
                FolderSelector.OnSelectorCompleted += (Result result) =>
                {
                    selected = result;
                    waitSelectedResetEvent.Set();
                };
                selector.Show();
                waitSelectedResetEvent.WaitOne();
                return selected;
            });
        }

        public override void Dismiss()
        {
            CancelSelect();
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            switch (keyCode)
            {
                case Keycode.Back:
                    CancelSelect();
                    break;
                case Keycode.Focus:
                    return true;
            }

            return base.OnKeyDown(keyCode, e);
        }

        public void CancelSelect()
        {
            base.Dismiss();
            FolderSelector.OnCanceled?.Invoke();
        }



        public class Result
        {
            public string SelectItem { get; set; }

            public string[] SelectItems { get; set; }
        }

    }
}