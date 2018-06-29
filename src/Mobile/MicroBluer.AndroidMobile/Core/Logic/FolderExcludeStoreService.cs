namespace MicroBluer.AndroidMobile.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using MicroBluer.AndroidMobile.Models;

    public class FolderExcludeStoreService : StoreService<FolderExcludeModel>
    {
        protected override string SharedFileName { get; } = "folderExcludeFile";

        const string KeyPro = "_folderExclude_";

        const string guidPattern = "[?a-zA-Z0-9]{8}-[?a-zA-Z0-9]{4}-[?a-zA-Z0-9]{4}-[?a-zA-Z0-9]{4}-[?a-zA-Z0-9]{12}";

        readonly static string KeyRegex = $@"^{KeyPro}{guidPattern}$";

        public List<FolderExcludeModel> GetList()
        {
            var list = new List<FolderExcludeModel>();
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

        public new FolderExcludeModel Get(string guid)
        {
            var key = CreateKey(guid);
            return base.Get(key);
        }

        public void Add(string path, FolderKind kind)
        {
            var id = Guid.NewGuid();
            var model = new FolderExcludeModel
            {
                Guid = id,
                Path = path,
                Type = kind,
            };
            var key = $"{id}";
            base.Save(key, model);
        }

        public bool Save(FolderExcludeModel model)
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