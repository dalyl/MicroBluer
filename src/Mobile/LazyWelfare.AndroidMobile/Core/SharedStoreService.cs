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
        protected abstract string SharedFileName { get; }

        public T GetModel(Guid domain)
        {
            var key = "";
            return GetModel(key);
        }

        public T GetModel(string key)
        {
            var value = StoreService.Provider.Shared(SharedFileName).GetValue(key, string.Empty);
            if (string.IsNullOrEmpty(value)) return default(T);
            return JsonConvert.DeserializeObject<T>(value);
        }

        public bool StoreModel(string key,T model)
        {
            if (model == null) return false;
            var value = JsonConvert.SerializeObject(model);
            StoreService.Provider.Shared(SharedFileName).PutValue(key, value);
            return true;
        }
    }
}