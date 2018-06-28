namespace MicroBluer.AndroidCtrls.FileExpler
{
    using Java.IO;
    using MicroBluer.AndroidUtils;
    using MicroBluer.AndroidUtils.IO;
    using System.Collections.Generic;
    using System.Linq;
    using Resource = MicroBluer.AndroidCtrls.Resource;

    public class ExplerItemCollection : List<ExplerItem>
    {
        string[] Extensions { get; set; } = null;

        public ExplerItemCollection(string[] extensions=null)
        {
            Extensions = extensions;
        }

        public void Add(List<string> roots)
        {
            this.Clear();
            if (roots == null) return;
            foreach (var path in roots)
            {
                var it = new File(path);
                if (it.Exists() == false)
                {
                    TryCatch.Current.Show($"'{path}':不存在的路径");
                    continue;
                }
                var extension = GetExtension(it);
                if (it.IsFile && Extensions != null && Extensions.Contains(extension) == false) continue;
                var item = new ExplerItem
                {
                    FullName = it.Path,
                    Name = it.Name,
                    Parent = it.Parent,
                    IsDirectory = it.IsDirectory,
                    Extension = extension,
                    Size = it.GetFileSize(Extensions),
                    Icon = GetFileIcon(it),
                };
                if (Extensions != null && item.Size == 0) continue;
                this.Add(item);
            }
        }

        public void Add(string root)
        {
            this.Clear();
            var rootFile = new File(root);
            var items = rootFile.ListFiles();
            if (items == null) return;
            foreach (var it in items)
            {
                var extension = GetExtension(it);
                if (it.IsFile && Extensions != null && Extensions.Contains(extension) == false) continue;
                var item = new ExplerItem
                {
                    FullName = it.Path,
                    Name = it.Name,
                    Parent = it.Parent,
                    IsDirectory = it.IsDirectory,
                    Extension = extension,
                    Size = it.GetFileSize(Extensions),
                    Icon = GetFileIcon(it),
                };
                if (Extensions != null && item.Size==0) continue;
                this.Add(item);
            }
        }

        public string GetExtension(File file)
        {
            if (file.IsDirectory) return string.Empty;
            return file.GetExtension();
        }

        public int GetFileIcon(File file)
        {
            var extension = GetExtension(file);
            if (string.IsNullOrEmpty(extension)) return Resource.Drawable.expleror_folder;
            return GetFileIcon(extension);
        }

        public int GetFileIcon(string extension)
        {
            var lower = extension.ToLower();
            switch (lower)
            {
                case ".jpg": return Resource.Drawable.file_jpg;
                case ".jpeg": return Resource.Drawable.file_jpg;
                case ".png": return Resource.Drawable.file_png;
                case ".gif": return Resource.Drawable.file_gif;
            }
            return Resource.Drawable.file_none;
        }
    }
}