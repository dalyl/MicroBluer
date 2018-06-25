namespace MicroBluer.AndroidCtrls.FileExpler
{
    using Java.IO;
    using MicroBluer.AndroidUtils.IO;
    using System.Collections.Generic;
    using System.Linq;

    public class ExplerItemCollection : List<ExplerItem>
    {
        public ExplerItemCollection() { }

        public ExplerItemCollection(string root)
        {
            Add(root);
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
                var item = new ExplerItem
                {
                    FullName = it.Path,
                    Name = it.Name,
                    Parent = it.Parent,
                    IsDirectory = it.IsDirectory,
                    Extension = extension,
                    Size = it.GetFileSize(),
                    Icon = GetFileIcon(it),
                };
                this.Add(item);
            }
        }

        public string GetExtension(File file)
        {
            if (file.IsDirectory) return string.Empty;
            return file.GetExtension().ToLower();
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