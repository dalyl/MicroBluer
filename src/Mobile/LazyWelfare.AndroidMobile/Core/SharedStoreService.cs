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
using Newtonsoft.Json;

namespace LazyWelfare.AndroidMobile
{
    public abstract class SharedStoreService<T>
    {

        public SharedStoreService(Context context)
        {
            Shared = new SharedPreferences(context, SharedFileName);
        }

        protected abstract string SharedFileName { get; }

        protected SharedPreferences Shared { get; }

        public virtual T GetModel(string key)
        {
            var value = Shared.GetValue(key, string.Empty);
            if (string.IsNullOrEmpty(value)) return default(T);
            return JsonConvert.DeserializeObject<T>(value);
        }

        public bool StoreModel(string key,T model)
        {
            if (model == null) return false;
            var value = JsonConvert.SerializeObject(model);
            Shared.PutValue(key, value);
            return true;
        }
    }
}