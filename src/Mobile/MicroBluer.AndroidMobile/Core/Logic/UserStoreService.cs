namespace MicroBluer.AndroidMobile.Logic
{
    using System.Text;
    using System.Text.RegularExpressions;
    using MicroBluer.AndroidMobile.Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class UserStoreService : StoreService<string>
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
            var keys = Shared.AllKeys();
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
            JObject item = JObject.FromObject(model);
            var attrs = item.Properties();
            foreach (var attr in attrs)
            {
                if (attr.Value == null) continue;
                var value = attr.Value.ToString();
                if (string.IsNullOrEmpty(value)) continue;
                SetAttr(attr.Name, value);
            }
            ActiveContext.ExpireSercvice<UserModel>();
            return true;


            // var json = JsonConvert.SerializeObject(model).Replace("{", "").Replace("}", "");
            //var attrItems = json.Split(',');
            //foreach (var attr in attrItems)
            //{
            //    var parts = attr.Split(':');
            //    var key = parts[0].Trim('"');
            //    var value = parts[1].Trim('"');
            //    if (string.IsNullOrEmpty(value)|| value=="null") continue;
            //    SetAttr(key, value);
            //}
            //ActiveContext.ExpireSercvice<UserModel>();
            //return true;
        }

        public string GetAttr(string attr)
        {
            var key = CreateKey(attr);
            return base.Get(key);
        }

        public bool SetAttr(string attr,string attrVlaue)
        {
            
            var key = CreateKey(attr.Trim('"'));
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