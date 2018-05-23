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

namespace LazyWelfare.AndroidMobile.Logic
{
    public class HostStoreService : SharedStoreService<HostModel>
    {

        const string KeyPro = "_host_";

        const string guidPattern = "[?a-zA-Z0-9]{8}-[?a-zA-Z0-9]{4}-[?a-zA-Z0-9]{4}-[?a-zA-Z0-9]{4}-[?a-zA-Z0-9]{12}";

        readonly static string KeyRegex = $@"^{KeyPro}{guidPattern}$" ;

        protected override string SharedFileName { get; } = "hostFile";


        public HostStoreService(Context context) : base(context) { }

        public HostStoreService() : base() { }


        public List<HostModel> GetList()
        {
            var list = new List<HostModel>();
            var keys = AllKeys();
            var regPid = new Regex(KeyRegex, RegexOptions.IgnoreCase);
            foreach (var one in keys)
            {
                var match = regPid.Match(one);
                if (match.Success)
                {
                    var model =base.Get(one);
                    if (model != null) list.Add(model);
                }
            }
            return list;
        }

        string CreateKey(string guid)
        {
            return $"{KeyPro}{guid}";
        }

        string CreateKey(Guid guid)
        {
            return CreateKey(guid.ToString());
        }

        public new HostModel Get(string guid)
        {
            var key = CreateKey (guid) ;
            return base.Get(key);
        }


        public void Add(string addr)
        {
            var id = Guid.NewGuid();
            var model = new HostModel
            {
                Domain = id,
                Name= addr,
                Address = addr,
            };
            var key = $"{model.Domain}";
            base.Save(key,model);
        }

        public bool Save(HostModel model)
        {
            var key = CreateKey(model.Domain);
            return base.Save(key, model);
        }

        public new bool Delete(string guid)
        {
            var key = CreateKey (guid) ;
            return base.Delete(key);
        }
    }
}