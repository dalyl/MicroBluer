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
    public abstract class SharedStoreService<T> : SharedStoreService
    {

        protected T Get(string key)
        {
            var value = GetValue(key, string.Empty);
            if (string.IsNullOrEmpty(value)) return default(T);
            return JsonConvert.DeserializeObject<T>(value);
        }

        protected bool Save(string key, T model)
        {
            if (model == null) return false;
            var value = JsonConvert.SerializeObject(model);
            PutValue(key, value);
            return true;
        }

        protected bool Delete(string key)
        {
            Remove(key);
            return true;
        }

    }

    public abstract class SharedStoreService
    {

        protected abstract string SharedFileName { get; }

        private Context Context { get; } = ActiveContext.Activity;

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

        protected void Remove(string key, FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(SharedFileName, mode).Edit();
            sp.Remove(key);
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