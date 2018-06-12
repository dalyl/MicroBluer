

namespace LazyWelfare.AndroidCtrls.ImageExpleror.Model
{

    using Parcelable = AndroidUtils.Parcel.Parcelable;
    using System.Collections.Generic;
    using Android.OS;
    using Android.Runtime;
    using Java.Lang;
    using LazyWelfare.AndroidCtrls.ImageExpleror.Utils;
    using LazyWelfare.AndroidCtrls.ImageExpleror.Engine;
    using LazyWelfare.AndroidUtils.Parcel;
    using LazyWelfare.AndroidUtils.Extension;

    public class SelectionSpec: Parcelable
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
            ConvertFromParcel(source);
        }

        public override void ConvertFromParcel(Parcel data)
        {
            MinSelectable = data.ReadInt();
            MaxSelectable = data.ReadInt();
            MinPixels = data.ReadLong();
            MaxPixels = data.ReadLong();
            IsEnableCamera = data.ReadBoolean();
            mStartWithCamera = data.ReadBoolean();
            Engine = data.ReadParcelable(Class.FromType(typeof(LoadEngine)).ClassLoader) as LoadEngine;
            List<MimeType> list = new List<MimeType>();
            data.ReadList(list, Class.FromType(typeof(MimeType)).ClassLoader);
            MimeTypes = list;
        }

        public override void WriteToParcel(Parcel data, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            data.WriteInt(MinSelectable);
            data.WriteInt(MaxSelectable);
            data.WriteLong(MinPixels);
            data.WriteLong(MaxPixels);
            data.WriteBoolean(IsEnableCamera);
            data.WriteBoolean(mStartWithCamera);
            data.WriteParcelable(this.Engine, ParcelableWriteFlags.None);
            data.WriteList(MimeTypes);
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
     
     
        [Java.Interop.ExportField("CREATOR")]
        public static ParcelableCreator<SelectionSpec> Creator() => new ParcelableCreator<SelectionSpec>();

    }


}