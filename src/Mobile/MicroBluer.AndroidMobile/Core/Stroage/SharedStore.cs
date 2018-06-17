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

namespace MicroBluer.AndroidMobile.Stroage
{

    public sealed class SharedStore
    {
        public SharedStore(string storeFile) { SharedFile = storeFile; }

        public  string SharedFile { get; }

        private Context Context { get; } = ActiveContext.Activity;

        public IEnumerable<string> AllKeys(FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(SharedFile, mode);
            return sp.All.Keys;
        }


        public void PutValue(string key, int value, FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(SharedFile, mode).Edit();
            sp.PutInt(key, value);
            sp.Apply();
        }

        public void PutValue(string key, bool value, FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(SharedFile, mode).Edit();
            sp.PutBoolean(key, value);
            sp.Apply();
        }

        public void PutValue(string key, string value, FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(SharedFile, mode).Edit();
            sp.PutString(key, value);
            sp.Apply();
        }

        public void Remove(string key, FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(SharedFile, mode).Edit();
            sp.Remove(key);
            sp.Apply();
        }


        public int GetValue(string key, int defValue, FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(SharedFile, mode);
            var value = sp.GetInt(key, defValue);
            return value;
        }

        public bool GetValue(string key, bool defValue, FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(SharedFile, mode);
            var value = sp.GetBoolean(key, defValue);
            return value;
        }

        public string GetValue(string key, string defValue, FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(SharedFile, mode);
            var value = sp.GetString(key, defValue);
            return value;
        }
    }
}