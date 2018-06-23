namespace MicroBluer.AndroidUtils.IO
{
    using Java.IO;
    using Java.Text;

    public static class FileExtension
    {

        public static string GetExtension(this File f)
        {
            if (f.IsFile == false) return string.Empty;
            var length = f.Name.Length;
            var index = f.Name.LastIndexOf(".");
            return index > 0?f.Name.Substring(index, length - index):string.Empty;
        }

        /// <summary>
        ///* 获取文件大小 ** 
        /// </summary>
        public static long GetFileSize(this File f)
        {
            long size = 0;
            return TryCatch.Current.Invoke(size, () =>
            {
                if (f == null) return size;
                if (f.IsDirectory) f.GetFilesSize();
                return f.Length();
            });
        }

        /// <summary>
        ///* 获取文件夹大小 ** 
        /// </summary>
        static long GetFilesSize(this File f)
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
                        size = size + GetFilesSize(item);
                    }
                    else
                    {
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
