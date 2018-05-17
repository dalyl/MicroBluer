using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace LazyWelfare.AndroidMobile.ImageSelect.Engine
{
    public class CustomEngine : LoadEngine
    {

        public override void DisplayImage(string path, ImageView imageView)
        {
        }

        public override void DisplayCameraItem(ImageView imageView)
        {

        }

        public override void Scrolling(GridView view)
        {

        }

        public override int DescribeContents()
        {
            return 0;
        }

        public override void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags  flags)
        {
        }

        public CustomEngine()
        {

        }

        protected internal CustomEngine(Parcel @in)
        {
        }

        public class CustomEngineParcelableCreator : Java.Lang.Object, IParcelableCreator
        {
            public Java.Lang.Object CreateFromParcel(Parcel source)
            {
                return new CustomEngine(source);
            }

            public Java.Lang.Object[] NewArray(int size)
            {
                return new Java.Lang.Object[size];
            }
        }


        [Java.Interop.ExportField("CREATOR")]
        public static CustomEngineParcelableCreator Creator() => new CustomEngineParcelableCreator();

    }

    //protected internal override void onActivityResult(int requestCode, int resultCode, Intent data)
    //{
    //    base.onActivityResult(requestCode, resultCode, data);
    //    if (requestCode == REQUEST_CODE_CHOOSE && resultCode == RESULT_OK)
    //    {
    //        mSelected = PicturePickerUtils.obtainResult(data);
    //        foreach (Uri u in mSelected)
    //        {
    //            Log.i("picture", u.Path);
    //        }
    //    }
    //}

}