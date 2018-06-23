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
                var item = new ExplerItem
                {
                    FullName = it.Path,
                    Name = it.Name,
                    Parent = it.Parent,
                    IsDirectory = it.IsDirectory,
                    Size = it.GetFileSize(),
                    Icon = GetFileIcon(it),
                };
                this.Add(item);
            }
        }
      

        public int GetFileIcon(File file)
        {
            if (file.IsDirectory) return Resource.Drawable.expleror_folder;
            var extension = file.GetExtension();
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