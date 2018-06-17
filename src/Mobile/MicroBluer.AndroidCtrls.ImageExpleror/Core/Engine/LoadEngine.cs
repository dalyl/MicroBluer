namespace MicroBluer.AndroidCtrls.ImageExpleror.Engine
{
    using Parcelable = AndroidUtils.Parcel.Parcelable;
    using Android.OS;
    using Android.Runtime;
    using Android.Widget;

    public abstract class LoadEngine: Parcelable
    {
        public const string INITIALIZE_ENGINE_ERROR = "initialize error,image load engine can not be null";

        public abstract void DisplayCameraItem(ImageView imageView);

        public abstract void DisplayImage(string path, ImageView imageView);

        public abstract void Scrolling(GridView view);
    }
}