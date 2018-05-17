using Android.OS;
using Android.Runtime;
using Android.Widget;
using Java.Lang;

namespace LazyWelfare.AndroidMobile.ImageSelect.Engine
{
    public abstract class LoadEngine:Object, IParcelable
    {

        public abstract void DisplayCameraItem(ImageView imageView);

        public abstract void DisplayImage(string path, ImageView imageView);

        public abstract void Scrolling(GridView view);

        public virtual int DescribeContents()
        {
            return 0;
        }

        public virtual void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
          
        }
    }

    public static class LoadEngine_Fields
    {
        public const string INITIALIZE_ENGINE_ERROR = "initialize error,image load engine can not be null";
    }
}