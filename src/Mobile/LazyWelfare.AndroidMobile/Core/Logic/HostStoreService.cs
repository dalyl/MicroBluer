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
        const string KeyRegex = @"^mm_[0-9]{8}_[0-9]+_[0-9]+$";

        protected override string SharedFileName { get; } = "hostFile";

        public HostStoreService(Context context) : base(context) { }

        public List<HostModel> GetList()
        {
            var list = new List<HostModel>();
            var keys = AllKeys();
            var regPid = new Regex(KeyRegex, RegexOptions.IgnoreCase);
            foreach (var one in keys)
            {
                //var match = regPid.Match(one);
                //if (match.Success)
                //{
                    var model = GetModel(one);
                    if (model != null) list.Add(model);
               // }
            }
            return list;
        }


        string CreateKey(string guid)
        {
            return $"{guid}";
        }
        string CreateKey(Guid guid)
        {
            return CreateKey(guid.ToString());
        }

        public override HostModel GetModel(string guid)
        {
            var key = CreateKey (guid) ;
            return base.GetModel(key);
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

        public override bool Delete(string guid)
        {
            var key = CreateKey (guid) ;
            return base.Delete(key);
        }
    }
}