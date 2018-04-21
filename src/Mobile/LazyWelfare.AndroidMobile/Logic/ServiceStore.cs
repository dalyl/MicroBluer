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

namespace LazyWelfare.AndroidMobile.Logic
{
    public class ServiceStore
    {
        public Context Context { get; private set; }

        public string File { get; private set; }

        public ServiceStore(Context context, string name)
        {
            Context = context;
            File = name;
        }

        public void PutDictionary(Dictionary<string, object> data)//(SharedPreferences存储)保存
        {

            ISharedPreferences sp = Context.GetSharedPreferences(File, FileCreationMode.Append);
            ISharedPreferencesEditor editor = sp.Edit();

            foreach (var one in data)
            {
                var _type = one.GetType().ToString();
                switch (_type)
                {
                    case "System.Boolean": editor.PutBoolean(one.Key, (bool)one.Value); break;
                    case "System.Float": editor.PutFloat(one.Key, (float)one.Value); break;
                    case "System.Int": editor.PutInt(one.Key, (int)one.Value); break;
                    case "System.Long": editor.PutLong(one.Key, (long)one.Value); break;
                    // case "System.String": editor.PutStringSet(one.Key, (string)one.Value);break;
                    case "System.String": editor.PutString(one.Key, (string)one.Value); break;
                }
            }
            editor.Apply();
        }

        public Dictionary<string, object> GetDictionary(Dictionary<string, object> data)//(SharedPreferences)读取
        {
            ISharedPreferences sp = Context.GetSharedPreferences(File, FileCreationMode.Private);
            String[] keys = data.Keys.ToArray<String>();

            foreach (var key in keys)
            {
                var value = data[key];
                var _type = value.GetType().ToString();
                switch (_type)
                {
                    case "System.Boolean": value = sp.GetBoolean(key, (bool)value); break;
                    case "System.Float": value = sp.GetFloat(key, (float)value); break;
                    case "System.Int": value = sp.GetInt(key, (int)value); break;
                    case "System.Long": value = sp.GetLong(key, (long)value); break;
                    // case "System.String": one= sp.PutStringSet(one.Key, (string)one.Value);break;
                    case "System.String": value = sp.GetString(key, (string)value); break;
                }
            }
            return data;
        }

        public void PutValue(string key, int value)
        {
            var sp = Context.GetSharedPreferences(File, FileCreationMode.Private).Edit();
            sp.PutInt(key, value);
            sp.Apply();
        }

        public void PutValue(string key, bool value)
        {
            var sp = Context.GetSharedPreferences(File, FileCreationMode.Private).Edit();
            sp.PutBoolean(key, value);
            sp.Apply();
        }

        public void PutValue(string key, string value)
        {
            var sp = Context.GetSharedPreferences(File, FileCreationMode.Private).Edit();
            sp.PutString(key, value);
            sp.Apply();
        }

        public int GetValue(string key, int defValue)
        {
            var sp = Context.GetSharedPreferences(File, FileCreationMode.Private);
            int value = sp.GetInt(key, defValue);
            return value;
        }

        public bool GetValue(string key, bool defValue)
        {
            var sp = Context.GetSharedPreferences(File, FileCreationMode.Private);
            var value = sp.GetBoolean(key, defValue);
            return value;
        }

        public String GetValue(string key, string defValue)
        {
            var sp = Context.GetSharedPreferences(File, FileCreationMode.Private);
            String value = sp.GetString(key, defValue);
            return value;
        }
    }
}