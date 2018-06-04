using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LazyWelfare.AndroidMobile.Models;
using Newtonsoft.Json;

namespace LazyWelfare.AndroidMobile.Logic
{
    public class UserStoreService : SharedStoreService<string>
    {
        const string KeyPro = "_user_";

        const string attrPattern = @"(\w+)";

        readonly static string KeyRegex = $@"^{KeyPro}{attrPattern}$";

        protected override string SharedFileName { get; } = "userFile";

        string CreateKey(string attr)
        {
            return $"{KeyPro}{attr}";
        }

        public  UserModel Get()
        {
            var keys = AllKeys();
            var regPid = new Regex(KeyRegex, RegexOptions.IgnoreCase);
            var json = new StringBuilder();
            json.Append("{");
            foreach (var one in keys)
            {
                var match = regPid.Match(one);
                if (match.Success)
                {
                    var value = base.Get(one);
                    var attr = match.Groups[1].Value;
                    json.Append($"{attr}:'{value}',");
                }
            }
            json.Append("}");
            var result = json.ToString().Replace(",}", "}");
            return JsonConvert.DeserializeObject<UserModel>(result);
        }

        public bool Save(UserModel model)
        {
            var json = JsonConvert.SerializeObject(model).Replace("{","").Replace("}","");
            var attrItems = json.Split(',');
            foreach (var attr in attrItems)
            {
                var parts = attr.Split(',');
                SetAttr(parts[0],parts[1]);
            }
            return true;
        }

        public string GetAttr(string attr)
        {
            var key = CreateKey(attr);
            return base.Get(key);
        }

        public  bool SetAttr(string attr,string attrVlaue)
        {
            var key = CreateKey(attr);
            return base.Save(key, attrVlaue);
        }

        public bool SetHost(string value)
        {
            var key = "Host";
            var result= base.Save(key, value);
            if (result) {
                ActiveContext.ExpireSercvice<UserModel>();
                ActiveContext.ExpireSercvice<HostModel>();
            }
            return result;
        }
         
    }
}