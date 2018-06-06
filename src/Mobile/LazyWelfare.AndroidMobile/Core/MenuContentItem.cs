namespace LazyWelfare.AndroidMobile
{
    using Java.Lang;

    public class MenuContentItem:Object
    {

        public virtual int Id { get; set; }
        public virtual string Text { get; set; }
        public virtual int ImageView { get; set; }

        public MenuContentItem()
        {
        }

        public MenuContentItem(int imageView, string text, int id)
        {
            this.ImageView = imageView;
            this.Text = text;
            this.Id = id;
        }

    }

}