
namespace LazyWelfare.AndroidCtrls.PopMenu
{
    using System;

    /// <summary>
    /// Used popupWindow 的menu列表项的bean类
    /// </summary>

    public class PopupMenuItem
    {

        public int ItemImage { get; set; } = 0;

        public string ItemText { get; set; }

        public Action Click { get; set; }

    }
}

