namespace LazyWelfare.AndroidMobile.Logic
{
    using LazyWelfare.AndroidMobile.Models;
    using System;
    using System.IO;
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

        public bool PullMapFiles(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                var models = GetList();
                models.ForEach(it => MoveMap(it));
            }
            else {
                var model = Get(args);
                if (model == null) return ActiveContext.Try.Show(false,"参数无效");
                MoveMap(model);
            }
            return true;
        }

        public bool PushMapFiles(string args)
        {
            var model = Get(args);
            if (model == null) return ActiveContext.Try.Show(false, "参数无效");
            RestoreMap(model);
            return true;
        }

        bool RestoreMap(FolderMapModel item)
        {
            var src = $"{ActiveContext.Activity.FilesDir}\\{item.InnerFolder}";
            if (Directory.Exists(src) == false) return ActiveContext.Try.Show(false, $"{item.Name} 还原文件夹不存在");
            var dest = item.MapFolder;
            MoveDir(src, dest);
            return true;
        }

        bool MoveMap(FolderMapModel item)
        {
            var src = item.MapFolder;
            if (Directory.Exists(src) == false) return ActiveContext.Try.Show(false, $"{item.Name} 源文件夹不存在");
            var dest = $"{ActiveContext.Activity.FilesDir}\\{item.InnerFolder}";
            MoveDir(src, dest);
            return true;
        }

        void MoveDir(string srcDir, string destDir)
        {
            MoveFile(srcDir, destDir);
            var dirs = Directory.GetDirectories(srcDir);
            foreach (var dir in dirs)
            {
                var dest = dir.Replace(srcDir, destDir);
                MoveDir(dir, dest);
                MoveFile(dir, dest);
            }
        }

        void MoveFile(string srcDir, string destDir)
        {
            if (Directory.Exists(destDir) == false) Directory.CreateDirectory(destDir);
            var files = Directory.GetFiles(srcDir);
            foreach (var file in files)
            {
                var destfile = file.Replace(srcDir, destDir);
                File.Move(file, destfile);
            }
        }

    }


    
}