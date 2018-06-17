 

namespace MicroBluer.AndroidCtrls.ImageExpleror
{
    using Object = Java.Lang.Object;
    using Activity = Android.App.Activity;
    using Intent = Android.Content.Intent;
    using Uri = Android.Net.Uri;
    using Fragment = Android.Support.V4.App.Fragment;
    using System.Collections.Generic;
    using System.Linq;
    using MicroBluer.AndroidCtrls.ImageExpleror.Model;
    using MicroBluer.AndroidCtrls.ImageExpleror.Engine;
    using MicroBluer.AndroidCtrls.ImageExpleror.Utils;
    using Android.Content;
    using MicroBluer.AndroidUtils.Extension;
    using System;
    using Java.Lang;

    public sealed class BulbPlugin
    {
        private const string INITIALIZE_PICKER_ERROR = "Try to initialize Picker which had already been initialized before";
        private static bool hasInitPicker;
        private WeakReference<Activity> mContext { get; }
        private  WeakReference<Fragment> mFragment { get; }
        private  List<MimeType> MimeTypes { get; }
        private  SelectionSpec mSelectionSpec { get; }
        private LoadEngine engine; //图片加载器 glide  imageloder picasso
        private IList<Uri> mResumeList;

        internal BulbPlugin(Activity activity, Fragment fragment, List<MimeType> mimeType)
        {
            mContext = new WeakReference<Activity>(activity);
            if (fragment != null)
            {
                mFragment = new WeakReference<Fragment>(fragment);
            }
            else
            {
                mFragment = null;
            }
            MimeTypes = mimeType;
            mSelectionSpec = new SelectionSpec();
            mResumeList = new List<Uri>();
        }

  

        internal BulbPlugin(Activity activity, Fragment fragment)
        {
            mContext = new WeakReference<Activity>(activity);
            if (fragment != null)
            {
                mFragment = new WeakReference<Fragment>(fragment);
            }
            else
            {
                mFragment = null;
            }
            MimeTypes = MimeType.AllOf();
            mSelectionSpec = new SelectionSpec();
            mResumeList = new List<Uri>();
        }


        /// <summary>
        /// set iamge  load engine
        /// </summary>
        /// <param name="engine">
        /// @return </param>
        public BulbPlugin SetEngine(LoadEngine engine)
        {
            this.engine = engine;
            return this;
        }

        /// <summary>
        /// set the first item open camera
        /// </summary>
        /// <param name="mEnableCamera">
        /// @return </param>
        public BulbPlugin EnableCamera(bool mEnableCamera)
        {
            mSelectionSpec.IsEnableCamera= mEnableCamera;
            return this;
        }

        /// <summary>
        /// set if should start the camera by default
        /// </summary>
        /// <param name="mStartWithCamera">
        /// @return </param>
        public BulbPlugin StartCamera(bool mStartWithCamera)
        {
            mSelectionSpec.StartWithCamera(mStartWithCamera);
            return this;
        }

        /// <summary>
        /// Sets the limitation of a selectable count within the specified range.
        /// </summary>
        /// <param name="min"> minimum value to select. </param>
        /// <param name="max"> maximum value to select. </param>
        /// <returns> the specification builder context. </returns>
        public BulbPlugin Count(int min, int max)
        {
            mSelectionSpec.MinSelectable = min;
            mSelectionSpec.MaxSelectable = max;
            return this;
        }

        /// <summary>
        /// Sets the limitation of a selectable count within the specified range.
        /// </summary>
        /// <param name="max"> maximum value to select. </param>
        /// <returns> the specification builder context. </returns>
        public BulbPlugin Count(int max)
        {
            mSelectionSpec.MinSelectable = 0;
            mSelectionSpec.MaxSelectable = max;
            return this;
        }

        public BulbPlugin SingleChoice()
        {
            Count(0, 1);
            return this;
        }

        //TODO : Wait achieve
        /// <summary>
        /// Sets the limitation of a selectable image quality by pixel count within the specified range.
        /// </summary>
        /// <param name="minPixel"> minimum value to select. </param>
        /// <param name="maxPixel"> maximum value to select. </param>
        /// <returns> the specification builder context. </returns>
        public BulbPlugin Quality(int minPixel, int maxPixel)
        {
            mSelectionSpec.MinPixels = minPixel;
            mSelectionSpec.MaxPixels = maxPixel;
            return this;
        }

        /// <summary>
        /// Sets the default selection to resume photo picking activity.
        /// </summary>
        /// <param name="uriList"> to set selected as default. </param>
        /// <returns> the specification builder context. </returns>
        public BulbPlugin Resume(IList<Uri> uriList)
        {
            if (uriList == null)
            { // nothing to do.
                return this;
            }
            ((List<Uri>)mResumeList).AddRange(uriList);
            return this;
        }


        public void ForResult(Action<List<Uri>> OnSelectCompleted)
        {
            SetImageSelectCompleted(OnSelectCompleted);
            ForResult(-999);
        }

        void SetImageSelectCompleted(Action<List<Uri>> OnSelectCompleted)
        {
            if (OnSelectCompleted == null) return;
            ImageSelectActivity.OnSelectCompleted = urls =>
             {
                 OnSelectCompleted(urls);
                 ImageSelectActivity.OnSelectCompleted = null;
             };
        }

        /// <summary>
        /// Start to select photo.
        /// </summary>
        /// <param name="requestCode"> identity of the requester activity. </param>
        public void ForResult(int requestCode)
        {
            if (engine == null)
            {
                throw new ExceptionInInitializerError(LoadEngine.INITIALIZE_ENGINE_ERROR);
            }

            Activity activity = Activity;
            if (activity == null)
            {
                return; // cannot continue;
            }
            mSelectionSpec.MimeTypes = MimeTypes;
            mSelectionSpec.Engine = engine;
            Intent intent = new Intent(activity, typeof(ImageSelectActivity));
            intent.PutExtra(ImageSelectActivity.EXTRA_SELECTION_SPEC, mSelectionSpec);
            //        intent.putExtra(ImageSelectActivity.EXTRA_ENGINE, (Serializable) engine);

            intent.PutParcelableArrayListExtra(ImageSelectActivity.EXTRA_RESUME_LIST, mResumeList.AsIParcelableList());

            Fragment fragment = Fragment;
            if (fragment != null)
            {
                fragment.StartActivityForResult(intent, requestCode);
            }
            else
            {
                activity.StartActivityForResult(intent, requestCode);
            }
            hasInitPicker = false;
        }

        public static BulbPlugin From(Activity activity)
        {
            if (hasInitPicker)
            {
                throw new ExceptionInInitializerError(INITIALIZE_PICKER_ERROR);
            }
            hasInitPicker = true;
            return new BulbPlugin(activity, null);
        }

        public static BulbPlugin From(Activity activity, List<MimeType> mimeType)
        {
            if (hasInitPicker)
            {
                throw new ExceptionInInitializerError(INITIALIZE_PICKER_ERROR);
            }
            hasInitPicker = true;
            return new BulbPlugin(activity, null, mimeType);
        }

        public static BulbPlugin From(Fragment fragment)
        {
            if (hasInitPicker)
            {
                throw new ExceptionInInitializerError(INITIALIZE_PICKER_ERROR);
            }
            hasInitPicker = true;
            return new BulbPlugin(fragment.Activity, fragment);
        }

        public static BulbPlugin From(Fragment fragment, List<MimeType> mimeType)
        {
            if (hasInitPicker)
            {
                throw new ExceptionInInitializerError(INITIALIZE_PICKER_ERROR);
            }
            hasInitPicker = true;
            return new BulbPlugin(fragment.Activity, fragment, mimeType);
        }

        public static Uri GetSingleResult(Intent data)
        {
            var urls= data.GetParcelableArrayListExtra(ImageSelectActivity.EXTRA_RESULT_SELECTION) as List<Uri>;
            return urls?.FirstOrDefault();
        }

        public static List<Uri> GetResult(Intent data)
        {
            return data.GetParcelableArrayListExtra(ImageSelectActivity.EXTRA_RESULT_SELECTION) as List<Uri>;
        }

        public static List<string> GetResult(ContentResolver resolver, Intent data)
        {
            List<Uri> uris = data.GetParcelableArrayListExtra(ImageSelectActivity.EXTRA_RESULT_SELECTION) as List<Uri>;
            List<string> paths = new List<string>();
            if (uris == null) return paths;
            foreach (Uri uri in uris)
            {
                paths.Add(PhotoMetadataUtils.GetPath(resolver, uri));
            }
            return paths;
        }


        internal Activity Activity
        {
            get
            {
                return mContext.Get();
            }
        }

        internal Fragment Fragment
        {
            get
            {
                return mFragment?.Get();
            }
        }

    }
}