namespace LazyWelfare.AndroidCtrls.ImageExpleror.Engine
{
    using Context = Android.Content.Context;
    using Parcel = Android.OS.Parcel;
    using GridView = Android.Widget.GridView;
    using ImageView = Android.Widget.ImageView;
    using Android.Runtime;
    using Android.OS;
    using Java.Lang;

    using Glide = Com.Bumptech.Glide.Glide;
    using LazyWelfare.AndroidUtils.Parcel;

    public class GlideEngine : LoadEngine
    {

        private int img_loading;
        private int img_camera;

        public GlideEngine() : this(0, 0)
        {
        }

        public GlideEngine(int img_loading) : this(img_loading, 0)
        {
        }

        public GlideEngine(int img_camera, int img_loading)
        {
            if (img_loading == 0)
            {
                this.img_loading = Resource.Drawable.image_not_exist;
            }
            else
            {
                this.img_loading = img_loading;
            }
            if (img_camera == 0)
            {
                this.img_camera = Resource.Drawable.ic_camera;
            }
            else
            {
                this.img_camera = img_camera;
            }
        }

        internal GlideEngine(Parcel data)
        {
            ConvertFromParcel(data);
        }

        public override void ConvertFromParcel(Parcel data)
        {
            this.img_loading = data.ReadInt();
            this.img_camera = data.ReadInt();
        }

        public override void DisplayImage(string path, ImageView imageView)
        {
            ChargeInit(imageView.Context);
            Glide.With(imageView.Context).Load(path).CenterCrop().Error(img_loading).Placeholder(img_loading).DontAnimate().Into(imageView);
        }

        public override void DisplayCameraItem(ImageView imageView)
        {
            ChargeInit(imageView.Context);
            Glide.With(imageView.Context).Load(img_camera).CenterCrop().Error(img_camera).Placeholder(img_camera).DontAnimate().Into(imageView);
        }

        private void ChargeInit(Context context)
        {
            if (Glide.Get(context) == null)
            {
                throw new ExceptionInInitializerError(LoadEngine.INITIALIZE_ENGINE_ERROR);
            }
        }

        public override void Scrolling(GridView view)
        {

        }

        public override int DescribeContents()
        {
            return 0;
        }

        public override void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteInt(this.img_loading);
            dest.WriteInt(this.img_camera);
        }

        [Java.Interop.ExportField("CREATOR")]
        public static ParcelableCreator<GlideEngine> Creator() => new ParcelableCreator<GlideEngine>();
    }


 

  
}