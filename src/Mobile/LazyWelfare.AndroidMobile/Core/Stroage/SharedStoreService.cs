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
    public abstract class SharedStoreService<T>: SharedStoreService
    {

        public SharedStoreService(Context context) : base(context) { }
     

        public virtual T GetModel(string key)
        {
            var value = GetValue(key, string.Empty);
            if (string.IsNullOrEmpty(value)) return default(T);
            return JsonConvert.DeserializeObject<T>(value);
        }

        public bool StoreModel(string key,T model)
        {
            if (model == null) return false;
            var value = JsonConvert.SerializeObject(model);
            PutValue(key, value);
            return true;
        }
    }

    public abstract class SharedStoreService
    {
        Context Context { get; set; }

        protected abstract string SharedFileName { get; }

        public SharedStoreService(Context context)
        {
            Context = context;
        }

        protected IEnumerable<string> AllKeys(FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(SharedFileName, mode);
            return sp.All.Keys;
        }


        protected void PutValue(string key, int value, FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(SharedFileName, mode).Edit();
            sp.PutInt(key, value);
            sp.Apply();
        }

        protected void PutValue(string key, bool value, FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(SharedFileName, mode).Edit();
            sp.PutBoolean(key, value);
            sp.Apply();
        }

        protected void PutValue(string key, string value, FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(SharedFileName, mode).Edit();
            sp.PutString(key, value);
            sp.Apply();
        }

        protected int GetValue(string key, int defValue, FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(SharedFileName, mode);
            var value = sp.GetInt(key, defValue);
            return value;
        }

        protected bool GetValue(string key, bool defValue, FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(SharedFileName, mode);
            var value = sp.GetBoolean(key, defValue);
            return value;
        }

        protected string GetValue(string key, string defValue, FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(SharedFileName, mode);
            var value = sp.GetString(key, defValue);
            return value;
        }
    }
}