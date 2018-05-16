using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using LazyWelfare.AndroidMobile.Utils;
using LazyWelfare.AndroidMobile.ImageSelect.Engine;

namespace LazyWelfare.AndroidMobile.ImageSelect.Model
{
    public class SelectionSpec:Java.Lang.Object, IParcelable
    {

        /// <summary>
        ///   最大选择数量
        /// </summary>
        public int MaxSelectable { get; set; }

        /// <summary>
        /// 最小选择数量
        /// </summary>
        public int MinSelectable { get; set; }

        /// <summary>
        /// 最小size
        /// </summary>
        public long MinPixels { get; set; }

        /// <summary>
        /// 最大size
        /// </summary>
        public long MaxPixels { get; set; }

        /// <summary>
        /// 是否可用相机
        /// </summary>
        public bool IsEnableCamera { get; set; }

        private bool mStartWithCamera;

        /// <summary>
        /// 图片加载器 glide  imageloder picasso
        /// </summary>
        public LoadEngine Engine { get; set; }

        public List<MimeType> MimeTypes { get;  set; }


        public SelectionSpec()
        {
            MinSelectable = 0;
            MaxSelectable = 1;
            MinPixels = 0L;
            MaxPixels = Long.MaxValue;
            IsEnableCamera = false;
            mStartWithCamera = false;
        }

        SelectionSpec(Parcel source)
        {
            MinSelectable = source.ReadInt();
            MaxSelectable = source.ReadInt();
            MinPixels = source.ReadLong();
            MaxPixels = source.ReadLong();
            IsEnableCamera = ParcelUtils.ReadBoolean(source);
            mStartWithCamera = ParcelUtils.ReadBoolean(source);
            Engine = source.ReadParcelable(Class.FromType(typeof(LoadEngine)).ClassLoader) as LoadEngine;
            List<MimeType> list = new List<MimeType>();
            source.ReadList(list, Class.FromType(typeof(MimeType)).ClassLoader);
            MimeTypes = list;
        }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest,  [GeneratedEnum] ParcelableWriteFlags  flags)
        {
            dest.WriteInt(MinSelectable);
            dest.WriteInt(MaxSelectable);
            dest.WriteLong(MinPixels);
            dest.WriteLong(MaxPixels);
            ParcelUtils.WriteBoolean(dest, IsEnableCamera);
            ParcelUtils.WriteBoolean(dest, mStartWithCamera);
            dest.WriteParcelable(this.Engine,  ParcelableWriteFlags.None);
            dest.WriteList(MimeTypes);
        }



        public bool WillStartCamera() { return mStartWithCamera; }


        public void StartWithCamera(bool mStartWithCamera) { this.mStartWithCamera = mStartWithCamera; }

        public bool IsSingleChoose
        {
            get{
                if (MinSelectable == 0 && MaxSelectable == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public class SelectionSpecParcelableCreator : Java.Lang.Object, IParcelableCreator
        {
            public Java.Lang.Object CreateFromParcel(Parcel source)
            {
                return new SelectionSpec(source);
            }

            public Java.Lang.Object[] NewArray(int size)
            {
                return new Object[size];
            }
        }

        [Java.Interop.ExportField("CREATOR")]
        public static SelectionSpecParcelableCreator Creator() => new SelectionSpecParcelableCreator();

    }

    
}