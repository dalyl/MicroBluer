namespace MicroBluer.AndroidUtils.IO
{
    using System.Collections.Generic;
    using System.Linq;
    using Java.IO;
    using Java.Text;

    public static class FileExtension
    {

        public static List<string> GetDirectories(string path, params string[] extensions)
        {
            List<string> result = new List<string>();
            File root = new File(path);
            if (root.IsDirectory == false) return new List<string>();
            var contents = root.ListFiles();

            var dirs= contents.Where(s => s.IsDirectory);
            foreach (var dir in dirs)
            {
                var subs = GetDirectoriesWithSelf(dir, extensions);
                result.AddRange(subs);
            }

            return result;
        }

        /// <summary>
        /// 获取包含指定后缀文件的文件夹路径
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDirectoriesWithSelf(string path, params string[] extensions)
        {
            File root = new File(path);
            if (root.IsDirectory == false) return new List<string>();
            return GetDirectoriesWithSelf(root, extensions);
        }

        /// <summary>
        /// 获取包含指定后缀文件的文件夹路径
        /// </summary>
        /// <returns></returns>
        static List<string> GetDirectoriesWithSelf(File root, params string[] extensions)
        {
            List<string> result = new List<string>();
            if (root.IsDirectory == false) return result;
            File[] contents = root.ListFiles();
            if (contents == null) return result;

            ///文件夹含有 指定后缀文件返回其本身
            var exist = contents.Where(s => s.IsFile).Select(s => extensions.Contains(GetExtension(s))).FirstOrDefault(s => s == true);
            if (exist)
            {
                result.Add(root.Path);
                return result;
            }

            ///否则，扫描其子文件
            var dirs = contents.Where(s => s.IsDirectory);
            foreach (var dir in dirs)
            {
                var subs = GetDirectoriesWithSelf(dir, extensions);
                result.AddRange(subs);
            }

            return result;
        }

        /// <summary>
        /// 获取文件后缀
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static string GetExtension(this File f)
        {
            if (f.IsFile == false) return string.Empty;
            var length = f.Name.Length;
            var index = f.Name.LastIndexOf(".");
            return index > 0?f.Name.Substring(index, length - index).ToLower():string.Empty;
        }

        /// <summary>
        ///  获取文件大小 
        /// </summary>
        public static long GetFileSize(this File f,params string[] extensions)
        {
            long size = 0;
            TryCatch.Current.Invoke(() =>
            {
                if (f == null) return;
                if (f.IsDirectory) size = f.GetFilesSize(extensions);
                else size = f.Length();
            });
            return size;
        }
     

        /// <summary>
        ///* 获取文件夹大小 ** 
        /// </summary>
        static long GetFilesSize(this File f, params string[] extensions)
        {
            long size = 0;
            if (f == null) return size;
            if (f.IsFile) f.GetFileSize();
            File[] flist = f.ListFiles();
            if (flist == null) return size;
            foreach (var item in flist)
            {
                TryCatch.Current.Invoke(() =>
                {
                    if (item.IsDirectory)
                    {
                        size = size + GetFilesSize(item, extensions);
                    }
                    else
                    {
                        var ext = GetExtension(item);
                        if (extensions != null && extensions.Contains(ext) == false) return;
                        size = size + item.Length();
                    }
                });
            }
            return size;
        }

        /// <summary>
        ///* 转换文件大小单位(b/kb/mb/gb) ** 
        /// </summary>
        public static string FormetFileSize(this long fileS)
        {
            // 转换文件大小
            DecimalFormat df = new DecimalFormat("#.00");
            string fileSizeString = "";
            if (fileS < 1024)
            {
                fileSizeString = df.Format((double)fileS) + "B";
            }
            else if (fileS < 1048576)
            {
                fileSizeString = df.Format((double)fileS / 1024) + "K";
            }
            else if (fileS < 1073741824)
            {
                fileSizeString = df.Format((double)fileS / 1048576) + "M";
            }
            else
            {
                fileSizeString = df.Format((double)fileS / 1073741824) + "G";
            }
            return fileSizeString;
        }

        /// <summary>
        ///* 获取文件个数 **
        /// </summary>
        public static long Getlist(this  File f)
        { // 递归求取目录文件个数
            long size = 0;
            File[] flist = f.ListFiles();
            size = flist.Length;
            foreach (var item in flist)
            {
                if (item.IsDirectory)
                {
                    size = size + Getlist(item);
                    size--;
                }
            }
            return size;
        }

    }
}
