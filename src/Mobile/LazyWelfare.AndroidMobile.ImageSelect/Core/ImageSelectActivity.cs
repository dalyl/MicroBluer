using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using LazyWelfare.AndroidMobile.ImageSelect.Model;
using LazyWelfare.AndroidMobile.ImageSelect.Utils;
using LazyWelfare.AndroidMobile.ImageSelect.Collection;
using Android.Support.V4.App;
using Java.Lang;
using Android.Views.Animations;

namespace LazyWelfare.AndroidMobile.ImageSelect
{
    using Action = System.Action;

    [Activity(Theme = "@android:style/Theme.NoTitleBar")]
    public class ImageSelectActivity : FragmentActivity, AlbumCollection.OnDirectorySelectListener
    {
        public static string EXTRA_RESULT_SELECTION = BundleUtils.BuildKey<ImageSelectActivity>("EXTRA_RESULT_SELECTION");
        public static string EXTRA_SELECTION_SPEC = BundleUtils.BuildKey<ImageSelectActivity>("EXTRA_SELECTION_SPEC");
        public static string EXTRA_RESUME_LIST = BundleUtils.BuildKey<ImageSelectActivity>("EXTRA_RESUME_LIST");
        //    public static final String EXTRA_ENGINE = BundleUtils.buildKey( "EXTRA_ENGINE");

        public static string STATE_CAPTURE_PHOTO_URI = BundleUtils.BuildKey<ImageSelectActivity>("STATE_CAPTURE_PHOTO_URI");

        private RelativeLayout rlTop;
        private TextView mFoldName;
        private View mListViewGroup;
        private ListView mListView;
        private GridView mGridView;
        public static int REQUEST_CODE_CAPTURE = 3;

        private MediaStoreCompat MediaStoreCompat { get; set; }

        private Button commit;
        private ImageView galleryTip;
        private SelectionSpec selectionSpec;
        private ImageView btnBack;
        private AlbumCollection albumCollection = new AlbumCollection();
        private PictureCollection mPhotoCollection = new PictureCollection();

        private SelectedUriCollection mCollection { get; set; }


        private string mCapturePhotoUriHolder;


        public static  System.Action<List<Uri>> OnSelectCompleted;


        private View.IOnClickListener mOnClickFoldName { get; set; }


        public ImageSelectActivity()
        {
            mCollection = new SelectedUriCollection();
            mOnClickFoldName = new ButtonClickListener(() => {
                if (mListViewGroup.Visibility == ViewStates.Visible)
                {
                    HideFolderList();
                }
                else
                {
                    ShowFolderList();
                }
            });
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ImageSelect);

            mCapturePhotoUriHolder = savedInstanceState != null ? savedInstanceState.GetString(STATE_CAPTURE_PHOTO_URI) : "";

            var Extra_selection = Intent.GetParcelableExtra(ImageSelectActivity.EXTRA_SELECTION_SPEC);
            selectionSpec = Extra_selection as SelectionSpec;
          //  selectionSpec = (SelectionSpec)Intent.GetParcelableExtra(ImageSelectActivity.EXTRA_SELECTION_SPEC);
            MediaStoreCompat = new MediaStoreCompat(this, new Handler(Looper.MainLooper));

            mCollection.OnCreate(savedInstanceState);
            mCollection.PrepareSelectionSpec(selectionSpec);

            var Extra_uris = Intent.GetParcelableArrayListExtra(EXTRA_RESUME_LIST);
            var uris = Extra_uris as IEnumerable<Uri>;
            if (uris != null) mCollection.AddRange(uris);

            mCollection.SetOnSelectionChange(new SelectionChange(commit));

            mGridView = (GridView)FindViewById(Resource.Id.gridView);
            mListView = (ListView)FindViewById(Resource.Id.listView);
            btnBack = (ImageView)FindViewById(Resource.Id.btn_back);
            mListViewGroup = FindViewById(Resource.Id.listViewParent);
            mListViewGroup.SetOnClickListener(mOnClickFoldName);
            mFoldName = (TextView)FindViewById(Resource.Id.foldName);
            galleryTip = (ImageView)FindViewById(Resource.Id.gallery_tip);
            LinearLayout selectFold = (LinearLayout)FindViewById(Resource.Id.selectFold);
            commit = (Button)FindViewById(Resource.Id.commit);
            commit.SetText("确定(0/" + selectionSpec.MaxSelectable + ")", TextView.BufferType.Normal);
            if (selectionSpec.IsSingleChoose)
            {
                commit.Visibility = ViewStates.Gone;
            }
            mFoldName.SetText("最近图片", TextView.BufferType.Normal);
            selectFold.SetOnClickListener(mOnClickFoldName);

            albumCollection.onCreate(this, this, selectionSpec, mListView);
            albumCollection.LoadAlbums();
            mPhotoCollection.OnCreate(this, mGridView, mCollection, selectionSpec);
            mPhotoCollection.LoadAllPhoto();

            commit.SetOnClickListener(new ButtonClickListener(() =>
            {
                if (mCollection.IsEmpty)
                {
                    Toast.MakeText(ApplicationContext, "未选择图片", ToastLength.Long).Show();
                }
                else
                {
                    SetResult();
                }
            }));

            btnBack.SetOnClickListener(new ButtonClickListener(() => this.Finish()));

            if (selectionSpec.WillStartCamera()) ShowCameraAction();

        }

        public void SetResult()
        {
            if (OnSelectCompleted != null)
                OnSelectCompleted(mCollection);
            else {
                Intent intent = new Intent();
                intent.PutParcelableArrayListExtra(ImageSelectActivity.EXTRA_RESULT_SELECTION, mCollection.AsIParcelableList());
                SetResult(Result.Ok, intent);
            }
            Finish();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            mCollection.OnSaveInstanceState(outState);
            albumCollection.onSaveInstanceState(outState);
            outState.PutString(STATE_CAPTURE_PHOTO_URI, mCapturePhotoUriHolder);
            base.OnSaveInstanceState(outState);
        }


        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            albumCollection.OnRestoreInstanceState(savedInstanceState);
        }


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == REQUEST_CODE_CAPTURE && resultCode == Result.Ok)
            {
                Uri captured = MediaStoreCompat.GetCapturedPhotoUri(data, mCapturePhotoUriHolder);
                if (captured != null)
                {
                    mCollection.Add(captured);
                    MediaStoreCompat.CleanUp(mCapturePhotoUriHolder);
                    if (mCollection.IsSingleChoose)
                    {
                        SetResult();
                    }
                }
            }
        }

        public void PrepareCapture(string uri)
        {
            mCapturePhotoUriHolder = uri;
        }

        private void ShowFolderList()
        {
            galleryTip.SetImageResource(Resource.Drawable.gallery_up);
            mListViewGroup.Visibility = ViewStates.Visible;
            mListView.Visibility = ViewStates.Visible;
            Animation animation = AnimationUtils.LoadAnimation(this, Resource.Animation.listview_up);
            Animation fadeIn = AnimationUtils.LoadAnimation(this, Resource.Animation.listview_fade_in);

            mListView.StartAnimation(animation);
            mListViewGroup.StartAnimation(fadeIn);
            //mListViewGroup.setVisibility(View.VISIBLE);
        }

        private void HideFolderList()
        {
            galleryTip.SetImageResource(Resource.Drawable.gallery_down);
            Animation animation = AnimationUtils.LoadAnimation(this, Resource.Animation.listview_down);
            Animation fadeOut = AnimationUtils.LoadAnimation(this, Resource.Animation.listview_fade_out);
            animation.SetAnimationListener(new AnimationListener(end: (Ani) => mListViewGroup.Visibility = ViewStates.Invisible));

            mListView.StartAnimation(animation);
            mListViewGroup.StartAnimation(fadeOut);
        }

        public MediaStoreCompat GetMediaStoreCompat()
        {
            return MediaStoreCompat;
        }


        protected override void OnDestroy()
        {
            MediaStoreCompat.Destroy();
            albumCollection.OnDestroy();
            mPhotoCollection.OnDestroy();
            base.OnDestroy();
        }

        public override void OnBackPressed()
        {
            if (mCollection.IsEmpty)
            {
                SetResult(Result.Canceled);
                base.OnBackPressed();
            }
        }
        /**
         * 选择相机
         */
        public void ShowCameraAction()
        {
            mCapturePhotoUriHolder = MediaStoreCompat.InvokeCameraCapture(this, ImageSelectActivity.REQUEST_CODE_CAPTURE);
        }


        public void OnSelect(Album album)
        {
            HideFolderList();
            mFoldName.SetText(album.GetDisplayName(this),TextView.BufferType.Normal);
            mPhotoCollection.resetLoad(album);
        }


        public void OnReset(Album album)
        {
            mPhotoCollection.load(album);
        }


        class SelectionChange : SelectedUriCollection.IOnSelectionChange
        {
            private Button _commit;
            public SelectionChange(Button commit)
            {
                _commit = commit;
            }
            public void OnChange(int maxCount, int selectCount)
            {
                _commit.SetText("确定(" + selectCount + "/" + maxCount + ")", TextView.BufferType.Normal);
            }
        }

        class ButtonClickListener : Object, View.IOnClickListener
        {
            Action Invoke { get; set; }
            public ButtonClickListener(Action invoke)
            {
                Invoke = invoke;
            }
            public void OnClick(View v)
            {
                Invoke();
            }
        }

        class AnimationListener : Object, Animation.IAnimationListener
        {
            System.Action<Animation> Start = null;
            System.Action<Animation> End = null;
            System.Action<Animation> Repeat = null;
            public AnimationListener(System.Action<Animation> start = null, System.Action<Animation> end = null, System.Action<Animation> repeat = null)
            {
                Start = start;
                End = end;
                Repeat = repeat;
            }

            public void OnAnimationStart(Animation animation)
            {
                Start?.Invoke(animation);
            }


            public void OnAnimationEnd(Animation animation)
            {
                End?.Invoke(animation);
            }


            public void OnAnimationRepeat(Animation animation)
            {
                Repeat?.Invoke(animation);
            }
        }




    }

}