namespace LazyWelfare.AndroidMobile.ImageSelect.Engine
{
    using Context = Android.Content.Context;
    using Parcel = Android.OS.Parcel;
    using GridView = Android.Widget.GridView;
    using ImageView = Android.Widget.ImageView;
    using Android.Runtime;
    using Android.OS;
    using Java.Lang;

    // using Glide = com.bumptech.glide.Glide;

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

        public override void DisplayImage(string path, ImageView imageView)
        {
            //ChargeInit(imageView.Context);
            //Glide.with(imageView.Context).load(path).centerCrop().error(img_loading).placeholder(img_loading).dontAnimate().into(imageView);
        }

        public override void DisplayCameraItem(ImageView imageView)
        {
            //ChargeInit(imageView.Context);
            //Glide.with(imageView.Context).load(img_camera).centerCrop().error(img_camera).placeholder(img_camera).dontAnimate().into(imageView);
        }

        //private void ChargeInit(Context context)
        //{
        //    if (Glide.get(context) == null)
        //    {
        //        throw new ExceptionInInitializerError(LoadEngine_Fields.INITIALIZE_ENGINE_ERROR);
        //    }
        //}

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

        protected internal GlideEngine(Parcel @in)
        {
            this.img_loading = @in.ReadInt();
            this.img_camera = @in.ReadInt();
        }

        public class GlideEngineParcelableCreator : Java.Lang.Object, IParcelableCreator
        {
            public Java.Lang.Object CreateFromParcel(Parcel source)
            {
                return new GlideEngine(source);
            }

            public Java.Lang.Object[] NewArray(int size)
            {
                return new Java.Lang.Object[size];
            }
        }


        [Java.Interop.ExportField("CREATOR")]
        public static GlideEngineParcelableCreator Creator() => new GlideEngineParcelableCreator();
    }
}