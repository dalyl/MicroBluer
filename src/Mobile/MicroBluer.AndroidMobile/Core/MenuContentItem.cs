namespace MicroBluer.AndroidMobile
{
    using System;

    public class MenuContentItem : Java.Lang.Object
    {

        public virtual int Id { get; set; }
        public virtual string Text { get; set; }
        public virtual int ImageView { get; set; }
        public virtual Action OnClick { get; }

        public MenuContentItem()
        {
        }

        public MenuContentItem(int imageView, string text, int id, Action click = null)
        {
            this.ImageView = imageView;
            this.Text = text;
            this.Id = id;
            this.OnClick = click;
        }

        public virtual void Click ()
        {
            OnClick?.Invoke();
        }
    }

}