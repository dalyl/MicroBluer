namespace MicroBluer.AndroidMobile.Logic
{
    using MicroBluer.AndroidMobile.Models;
    using System;
    using System.Linq;
    using System.IO;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using MicroBluer.AndroidUtils;
    using MicroBluer.AndroidUtils.IO;

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

        public bool Add(string name, string map, string inner)
        {
            var id = Guid.NewGuid();
            var model = new FolderMapModel
            {
                Guid = id,
                Name = name,
                MapFolder = map,
                InnerFolder = inner,
            };
            var key = CreateKey(model.Guid);
            base.Save(key, model);
            return true;
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

        public bool CollectMapFiles(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                var models = GetList();
                models.ForEach(it => MoveMap(it));
            }
            else {
                var model = Get(args);
                if (model == null) TryCatch.Current.Throw("参数无效");
                MoveMap(model);
            }
            return true;
        }

        public bool RevertMapFiles(string args)
        {
            var model = Get(args);
            if (model == null) return TryCatch.Current.Show(false, "参数无效");
            RestoreMap(model);
            return true;
        }

        bool RestoreMap(FolderMapModel item)
        {
            var src = $"{ActiveContext.User.Root}/{item.InnerFolder}";
            if (Directory.Exists(src) == false) return TryCatch.Current.Show(false, $"{item.Name} 还原文件夹不存在");
            var dest = item.MapFolder;
            MoveDir(src, dest);
            return TryCatch.Current.Show(true, $"{item.Name} 还原完成"); ;
        }

        public List<string> ScanPath(string root, string[] exts)
        {
            var exculdes = ActiveContext.FolderExcludeStore.GetList().Select(s => s.Path);
            var maps = GetList().Select(s => s.MapFolder);
            return ScanPath(root, exts, maps, exculdes);
        }

        List<string> ScanPath(string root, string[] exts,IEnumerable<string> maps, IEnumerable<string> exculdes)
        {
            var dirs = FileExtension.GetDirectories(root, exts);
            var dirsInMaps = dirs.Where(s => maps.Contains(s)).ToArray();
            foreach (var dir in dirsInMaps)
            {
                dirs.Remove(dir);
            }

            var dirsInExculdes = dirs.Where(s => exculdes.Contains(s)).ToArray();
            foreach (var dir in dirsInExculdes)
            {
                dirs.Remove(dir);
                var subs = ScanPath(dir, exts, maps, exculdes);
                dirs.AddRange(subs);
            }

            return dirs;
        }

        bool MoveMap(FolderMapModel item)
        {
            var src = item.MapFolder;
            if (Directory.Exists(src) == false) return TryCatch.Current.Show(false, $"{item.Name} 源文件夹不存在");
            var dest = $"{ActiveContext.User.Root}/{item.InnerFolder}";
            MoveDir(src, dest);
            return TryCatch.Current.Show(true, $"{item.Name} 归档完成"); ;
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