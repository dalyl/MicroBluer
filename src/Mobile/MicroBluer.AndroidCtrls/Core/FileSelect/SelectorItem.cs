namespace MicroBluer.AndroidCtrls.FileSelect
{
    public class SelectorItem
    {

        public string Name { get; set; }

        public string FullName { get; set; }

        public string Parent { get; set; }

        public string Extension { get; set; }

        public int Icon { get; set; }

        public bool IsSelectable { get; set; } = false;
        public bool IsDirectory { get; set; } = false;

    }
}