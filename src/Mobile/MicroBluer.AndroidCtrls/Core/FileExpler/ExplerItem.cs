namespace MicroBluer.AndroidCtrls.FileExpler
{
    using System.Linq;
     
    public class ExplerItem
    {
        static string[] imgfiles = { ".jpg", ".jpeg", ".png", ".gif" };

        public string Name { get; set; }

        public string FullName { get; set; }

        public string Parent { get; set; }

        public string Extension { get; set; }

        public int Icon { get; set; }

        public long Size { get; set; }

        public bool IsDirectory { get; set; } = false;

        public bool IsPicture {
            get {
                return imgfiles.Contains(Extension);
            }
        }
    }
}