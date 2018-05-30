namespace LazyWelfare.AndroidMobile.ImageSelect.Engine
{
    using Android.OS;
    using Android.Runtime;
    using Android.Widget;
    using LazyWelfare.AndroidUtils.Parcel;

    public class CustomEngine : LoadEngine
    {
        public override void ConvertFromParcel(Parcel data)
        {
            
        }

        public override void DisplayImage(string path, ImageView imageView)
        {
        }

        public override void DisplayCameraItem(ImageView imageView)
        {

        }

        public override void Scrolling(GridView view)
        {

        }
       

        public CustomEngine()
        {

        }

        protected internal CustomEngine(Parcel data)
        {
            ConvertFromParcel(data);
        }


        [Java.Interop.ExportField("CREATOR")]
        public static ParcelableCreator<CustomEngine> Creator() => new ParcelableCreator<CustomEngine>();
     
    }



}