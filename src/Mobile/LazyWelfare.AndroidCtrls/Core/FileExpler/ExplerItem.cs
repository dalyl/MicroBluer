namespace LazyWelfare.AndroidCtrls.FileExpler
{

     
    public class ExplerItem
    {
        public string Name { get; set; }

        public string FullName { get; set; }

        public string Parent { get; set; }

        public string Extension { get; set; }

        public int Icon { get; set; }

        public bool IsDirectory { get; set; } = false;
    }
}