namespace MicroBluer.AndroidCtrls.FileExpler
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class ExplerItemCollection : List<ExplerItem>
    {
        public ExplerItemCollection() { }

        public ExplerItemCollection(string root)
        {
            LoadDirectories(root);
            LoadFiles(root);
        }

        public void Add(string root)
        {
            this.Clear();
            LoadDirectories(root);
            LoadFiles(root);
        }

        public void LoadDirectories(string root)
        {
            var dirs = Directory.GetDirectories(root).OrderBy(it => it);
            foreach (var dir in dirs)
            {
                var info = new DirectoryInfo(dir);
                var item = new ExplerItem
                {
                    FullName = dir,
                    Name = info.Name,
                    Parent = info.Parent.FullName,
                    IsDirectory = true,
                    Icon = Resource.Drawable.expleror_folder,
                };
                this.Add(item);
            }
        }

        public void LoadFiles(string root)
        {
            var files = Directory.GetFiles(root).OrderBy(it => it);
            foreach (var file in files)
            {
                var info = new FileInfo(file);
                var item = new ExplerItem
                {
                    FullName = file,
                    Name = info.Name,
                    Extension = info.Extension,
                    Parent = info.Directory.FullName,
                    IsDirectory = false,
                    Icon = GetFileIcon(info.Extension)
                };
                this.Add(item);
            }
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