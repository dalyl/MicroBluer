using System;
using System.Collections.Generic;

namespace MicroBluer.AndroidUtils.Database
{
    using ContentObserver = Android.Database.ContentObserver;
    using Android.OS;
    using System;
    public class AnonymousContentObserver : ContentObserver
    {
        Action<bool> Change { get; }
        public AnonymousContentObserver(Handler handler, Action<bool> change) : base(handler)
        {
            Change = change;
        }

        public override void OnChange(bool selfChange)
        {
            base.OnChange(selfChange);
            Change(selfChange);
        }
    }

}