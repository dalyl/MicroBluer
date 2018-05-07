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
        public T GetModel(Guid domain)
        {
            var key = "";
            return GetModel(key);
        }

        public T GetModel(string key)
        {
            var value = StoreService.Provider.Shared.GetValue(key, string.Empty);
            if (string.IsNullOrEmpty(value)) return default(T);
            return JsonConvert.DeserializeObject<T>(value);
        }

        public bool StoreModel(T model)
        {
            if (model == null) return false;
            var key = "";
            var value = JsonConvert.SerializeObject(model);
            StoreService.Provider.Shared.PutValue(key, value);
            return true;
        }
    }
}