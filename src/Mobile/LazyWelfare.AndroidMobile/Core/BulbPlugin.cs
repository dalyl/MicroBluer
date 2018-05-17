 

namespace LazyWelfare.AndroidMobile
{
    using Activity = Android.App.Activity;
    using Intent = Android.Content.Intent;
    using Uri = Android.Net.Uri;
    using Fragment = Android.Support.V4.App.Fragment;
    using System.Collections.Generic;
    using System.Linq;
    using LazyWelfare.AndroidMobile.ImageSelect.Model;
    using LazyWelfare.AndroidMobile.ImageSelect.Engine;
    using LazyWelfare.AndroidMobile.ImageSelect;
    using LazyWelfare.AndroidMobile.Utils;
    using Java.Lang;
    using Android.Content;

    public sealed class Picker
    {
        private const string INITIALIZE_PICKER_ERROR = "Try to initialize Picker which had already been initialized before";
        private static bool hasInitPicker;
        private readonly System.WeakReference<Activity> mContext;
        private readonly System.WeakReference<Fragment> mFragment;
        private readonly List<MimeType> MimeTypes;
        private readonly SelectionSpec mSelectionSpec;
        private LoadEngine engine; //图片加载器 glide  imageloder picasso
        private IList<Uri> mResumeList;

        internal Picker(Activity activity, Fragment fragment, List<MimeType> mimeType)
        {
            mContext = new System.WeakReference<Activity>(activity);
            if (fragment != null)
            {
                mFragment = new System.WeakReference<Fragment>(fragment);
            }
            else
            {
                mFragment = null;
            }
            MimeTypes = mimeType;
            mSelectionSpec = new SelectionSpec();
            mResumeList = new List<Uri>();
        }

  

        internal Picker(Activity activity, Fragment fragment)
        {
            mContext = new System.WeakReference<Activity>(activity);
            if (fragment != null)
            {
                mFragment = new System.WeakReference<Fragment>(fragment);
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
        public Picker SetEngine(LoadEngine engine)
        {
            this.engine = engine;
            return this;
        }

        /// <summary>
        /// set the first item open camera
        /// </summary>
        /// <param name="mEnableCamera">
        /// @return </param>
        public Picker EnableCamera(bool mEnableCamera)
        {
            mSelectionSpec.IsEnableCamera= mEnableCamera;
            return this;
        }

        /// <summary>
        /// set if should start the camera by default
        /// </summary>
        /// <param name="mStartWithCamera">
        /// @return </param>
        public Picker StartCamera(bool mStartWithCamera)
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
        public Picker Count(int min, int max)
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
        public Picker Count(int max)
        {
            mSelectionSpec.MinSelectable = 0;
            mSelectionSpec.MaxSelectable = max;
            return this;
        }

        public Picker SingleChoice()
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
        public Picker Quality(int minPixel, int maxPixel)
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
        public Picker Resume(IList<Uri> uriList)
        {
            if (uriList == null)
            { // nothing to do.
                return this;
            }
            ((List<Uri>)mResumeList).AddRange(uriList);
            return this;
        }


        public void ForResult(System.Action<List<Uri>> OnSelectCompleted)
        {
            SetImageSelectCompleted(OnSelectCompleted);
            ForResult(-999);
        }

        void SetImageSelectCompleted(System.Action<List<Uri>> OnSelectCompleted)
        {
            if (OnSelectCompleted == null) return;
            System.Action<List<Uri>> act = urls =>
            {
                OnSelectCompleted(urls);
                ImageSelectActivity.OnSelectCompleted = null;
            };
            ImageSelectActivity.OnSelectCompleted = act;
        }

        /// <summary>
        /// Start to select photo.
        /// </summary>
        /// <param name="requestCode"> identity of the requester activity. </param>
        public void ForResult(int requestCode)
        {
            if (engine == null)
            {
                throw new ExceptionInInitializerError(LoadEngine_Fields.INITIALIZE_ENGINE_ERROR);
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

        public static Picker From(Activity activity)
        {
            if (hasInitPicker)
            {
                throw new ExceptionInInitializerError(INITIALIZE_PICKER_ERROR);
            }
            hasInitPicker = true;
            return new Picker(activity, null);
        }

        public static Picker From(Activity activity, List<MimeType> mimeType)
        {
            if (hasInitPicker)
            {
                throw new ExceptionInInitializerError(INITIALIZE_PICKER_ERROR);
            }
            hasInitPicker = true;
            return new Picker(activity, null, mimeType);
        }

        public static Picker From(Fragment fragment)
        {
            if (hasInitPicker)
            {
                throw new ExceptionInInitializerError(INITIALIZE_PICKER_ERROR);
            }
            hasInitPicker = true;
            return new Picker(fragment.Activity, fragment);
        }

        public static Picker From(Fragment fragment, List<MimeType> mimeType)
        {
            if (hasInitPicker)
            {
                throw new ExceptionInInitializerError(INITIALIZE_PICKER_ERROR);
            }
            hasInitPicker = true;
            return new Picker(fragment.Activity, fragment, mimeType);
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


        /// <returns> the actual requester context. </returns>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Nullable android.app.Activity getActivity()
        internal Activity Activity
        {
            get
            {
                return mContext.Get();
            }
        }

        /// <returns> the fragment that is responsible for result handling. </returns>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Nullable android.support.v4.app.Fragment getFragment()
        internal Fragment Fragment
        {
            get
            {
                return mFragment?.Get();
            }
        }

    }
}