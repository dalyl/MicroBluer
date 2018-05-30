namespace LazyWelfare.AndroidMobile.ImageSelect.Collection
{
    using Object = Java.Lang.Object;
    using Loader=Android.Support.V4.Content.Loader;
    using Android.Content;
    using Android.OS;
    using Android.Widget;
    using Android.Support.V4.App;
    using LazyWelfare.AndroidMobile.ImageSelect.Model;
    using LazyWelfare.AndroidMobile.ImageSelect.Adapter;
    using LazyWelfare.AndroidMobile.ImageSelect.Loader;
    using Android.Database;
    using LazyWelfare.AndroidUtils.Common;
    using LazyWelfare.AndroidUtils.Extension;

    public class PictureCollection :Object, LoaderManager.ILoaderCallbacks
{
		private const int LOADER_ID = 2;
		private static readonly string ARGS_ALBUM = BundleUtils.BuildKey<PictureCollection>("ARGS_ALBUM");
		private System.WeakReference<Context> mContext;
		private LoaderManager mLoaderManager;
		private PictureAdapter albumPhotoAdapter;
		private SelectionSpec selectionSpec;

		public Loader OnCreateLoader(int id, Bundle args)
		{
            Context context =mContext.Get();
			if (context == null)
			{
				return null;
			}
            if (!(args.GetParcelable(ARGS_ALBUM) is Album album))
            {
                return null;
            }
            return PictureLoader.NewInstance(context, album, selectionSpec);
		}

		public  void OnLoadFinished(Loader loader, Object data)
		{
			Context context =mContext.Get();
			if (context == null)
			{
				return;
			}
            var cursor = data as ICursor;
			albumPhotoAdapter.SwapCursor(cursor);
        }

		public  void OnLoaderReset(Loader loader)
		{
			Context context =mContext.Get();
			if (context == null)
			{
				return;
			}
			albumPhotoAdapter.SwapCursor(null);
		}

		public virtual void OnCreate(FragmentActivity context, GridView gridView, SelectedUriCollection mCollection, SelectionSpec selectionSpec)
		{
			mContext = new System.WeakReference<Context>(context);
			mLoaderManager = context.SupportLoaderManager;
			this.selectionSpec = selectionSpec;
			albumPhotoAdapter = new PictureAdapter(context, null, mCollection);
			mCollection.Engine.Scrolling(gridView);
			gridView.Adapter = albumPhotoAdapter;
		}

		public virtual void OnDestroy()
		{
			mLoaderManager.DestroyLoader(LOADER_ID);
		}

		public virtual void LoadAllPhoto()
		{
			Album album = new Album(Album.ALBUM_ID_ALL, -1, Album.ALBUM_NAME_ALL, "");
			Load(album);
		}

		public virtual void Load(Album target)
		{
			Bundle args = new Bundle();
			args.PutParcelable(ARGS_ALBUM, target);
			mLoaderManager.InitLoader(LOADER_ID, args, this);
		}

		public virtual void ResetLoad(Album target)
		{
			Bundle args = new Bundle();
			args.PutParcelable(ARGS_ALBUM, target);
			mLoaderManager.RestartLoader(LOADER_ID, args, this);
		}

	}
}