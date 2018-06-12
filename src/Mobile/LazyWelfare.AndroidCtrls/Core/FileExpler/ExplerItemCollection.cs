namespace LazyWelfare.AndroidCtrls.FileExpler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Runtime;
    using Android.Views;
    using Android.Widget;
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
            var dirs = Directory.GetDirectories(root);
            foreach (var dir in dirs)
            {
                var info = new DirectoryInfo(dir);
                var item = new ExplerItem
                {
                    FullName = dir,
                    Name = info.Name,
                    Parent = info.Parent.FullName,
                    IsDirectory = true,
                    Icon = Resource.Drawable.selector_folder,
                };
                this.Add(item);
            }
        }

        public void LoadFiles(string root)
        {
            var files = Directory.GetFiles(root);
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
                case ".jpg": return Resource.Drawable.selector_file_jpg;
                case ".jpeg": return Resource.Drawable.selector_file_jpg;
                case ".png": return Resource.Drawable.selector_file_png;
                case ".gif": return Resource.Drawable.selector_file_gif;
            }
            return Resource.Drawable.selector_file_none;
        }
    }
}