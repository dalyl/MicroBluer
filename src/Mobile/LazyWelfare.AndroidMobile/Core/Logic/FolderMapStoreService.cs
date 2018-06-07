namespace LazyWelfare.AndroidMobile.Logic
{
    using LazyWelfare.AndroidMobile.Models;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class FolderMapStoreService : StoreService<FolderMapModel>
    {
        protected override string SharedFileName { get; } = "folderFile";

        const string KeyPro = "_folder_";

        const string guidPattern = "[?a-zA-Z0-9]{8}-[?a-zA-Z0-9]{4}-[?a-zA-Z0-9]{4}-[?a-zA-Z0-9]{4}-[?a-zA-Z0-9]{12}";

        readonly static string KeyRegex = $@"^{KeyPro}{guidPattern}$";

        public List<FolderMapModel> GetList()
        {
            var list = new List<FolderMapModel>();
            var keys = Shared.AllKeys();
            var regPid = new Regex(KeyRegex, RegexOptions.IgnoreCase);
            foreach (var one in keys)
            {
                var match = regPid.Match(one);
                if (match.Success)
                {
                    var model = base.Get(one);
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

        public new FolderMapModel Get(string guid)
        {
            var key = CreateKey(guid);
            return base.Get(key);
        }

        public void Add(string name, string map, string inner)
        {
            var id = Guid.NewGuid();
            var model = new FolderMapModel
            {
                Guid = id,
                Name = name,
                MapFolder = map,
                InnerFolder = inner,
            };
            var key = $"{id}";
            base.Save(key, model);
        }

        public bool Save(FolderMapModel model)
        {
            var key = CreateKey(model.Guid);
            return base.Save(key, model);
        }

        public new bool Delete(string guid)
        {
            var key = CreateKey(guid);
            return base.Delete(key);
        }
    }
}