namespace LazyWelfare.AndroidCtrls.FileSelect
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

    public class SelectorItemCollection : List<SelectorItem>
    {

        public SelectorItemCollection() { }

        public SelectorItemCollection(string root, SelectorType type)
        {
            LoadDirectories(root, type);
            LoadFiles(root, type);
        }

        public void Add(string root, SelectorType type)
        {
            this.Clear();
            LoadDirectories(root, type);
            LoadFiles(root, type);
        }

        public void LoadDirectories(string root, SelectorType type)
        {
            var dirs = Directory.GetDirectories(root).OrderBy(it => it); 
            foreach (var dir in dirs)
            {
                var info = new DirectoryInfo(dir);
                var item = new SelectorItem
                {
                    FullName = dir,
                    Name = info.Name,
                    Parent = info.Parent.FullName,
                    IsSelectable = type == SelectorType.Directory,
                    IsDirectory = true,
                    Icon = Resource.Drawable.selector_folder,

                };
                this.Add(item);
            }
        }

        public void LoadFiles(string root, SelectorType type)
        {
            var filter = GetFileFilter(type);
            var files = (string.IsNullOrEmpty(filter) ? Directory.GetFiles(root) : Directory.GetFiles(root, filter)).OrderBy(it => it);
            foreach (var file in files)
            {
                var info = new FileInfo(file);
                var item = new SelectorItem
                {
                    FullName = file,
                    Name = info.Name,
                    Extension = info.Extension,
                    Parent = info.Directory.FullName,
                    IsSelectable = type != SelectorType.Directory,
                    IsDirectory = false,
                    Icon = GetFileIcon(info.Extension)
                };
                this.Add(item);
            }
        }

        public string GetFileFilter(SelectorType type)
        {
            switch (type)
            {
                case SelectorType.Image: return "(*.jpg|*.jpge|*.png|*.gif|*.ico)";
            }
            return string.Empty;
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
