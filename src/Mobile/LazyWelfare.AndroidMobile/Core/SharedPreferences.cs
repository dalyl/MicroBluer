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

namespace LazyWelfare.AndroidMobile
{

    public class SharedPreferences
    {
        Context Context { get; set; }

        string File { get; set; }

        public SharedPreferences(Context context, string name)
        {
            Context = context;
            File = name;
        }

        public IEnumerable<string> AllKeys(FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(File, mode);
            return sp.All.Keys;
        }


        public void PutValue(string key, int value, FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(File, mode).Edit();
            sp.PutInt(key, value);
            sp.Apply();
        }

        public void PutValue(string key, bool value, FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(File, mode).Edit();
            sp.PutBoolean(key, value);
            sp.Apply();
        }

        public void PutValue(string key, string value, FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(File, mode).Edit();
            sp.PutString(key, value);
            sp.Apply();
        }

        public int GetValue(string key, int defValue, FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(File, mode);
            var value = sp.GetInt(key, defValue);
            return value;
        }

        public bool GetValue(string key, bool defValue, FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(File, mode);
            var value = sp.GetBoolean(key, defValue);
            return value;
        }

        public string GetValue(string key, string defValue, FileCreationMode mode = FileCreationMode.Private)
        {
            var sp = Context.GetSharedPreferences(File, mode);
            var value = sp.GetString(key, defValue);
            return value;
        }
    }
}