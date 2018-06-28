namespace MicroBluer.AndroidMobile.Logic
{
    using MicroBluer.AndroidMobile.Models;
    using System.IO;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using MicroBluer.AndroidUtils;
    using MicroBluer.AndroidUtils.IO;
    using System;

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

        static readonly Dictionary<string, string[]> ExtensionDictionary = new Dictionary<string, string[]> {
                 { "image",new string[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp" } },
                 { "video",new string[] { ".mp3", ".wma", ".wav", ".amr", ".m4a", ".m4r", ".ape", ".flac" } },
                 { "music",new string[] {  ".mp4",".rm",".rmvb",".mpeg1", ".mpeg2", ".mpeg3", ".mpeg4", ".3gp", ".flv" } },
            };

        public List<string> ScanFileMaps(string tpye)
        {
            var dirs = new List<string>();
            if (ExtensionDictionary.ContainsKey(tpye) == false) TryCatch.Current.Throw("参数无效");
            var exts = ExtensionDictionary[tpye];
            return FileExtension.GetPaths(Android.OS.Environment.ExternalStorageDirectory.Path, exts);
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