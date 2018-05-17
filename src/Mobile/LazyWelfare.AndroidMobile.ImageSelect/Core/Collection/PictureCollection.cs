using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.Content.PM;
using LazyWelfare.AndroidMobile.ImageSelect.Model;
using LazyWelfare.AndroidMobile.ImageSelect.Utils;
using LazyWelfare.AndroidMobile.ImageSelect.Adapter;
using LazyWelfare.AndroidMobile.ImageSelect.Loader;
using Java.Lang;
using Android.Database;

namespace LazyWelfare.AndroidMobile.ImageSelect.Collection
{

public class PictureCollection :Object, Android.Support.V4.App.LoaderManager.ILoaderCallbacks
{
		private const int LOADER_ID = 2;
		private static readonly string ARGS_ALBUM = BundleUtils.BuildKey<PictureCollection>("ARGS_ALBUM");
		private System.WeakReference<Context> mContext;
		private Android.Support.V4.App.LoaderManager mLoaderManager;
		private PictureAdapter albumPhotoAdapter;
		private SelectionSpec selectionSpec;

		public  Android.Support.V4.Content.Loader OnCreateLoader(int id, Bundle args)
		{
            Context context =mContext.Get();
			if (context == null)
			{
				return null;
			}
			var album = args.GetParcelable(ARGS_ALBUM) as Album;
			if (album == null)
			{
				return null;
			}
			return PictureLoader.NewInstance(context, album, selectionSpec);
		}

		public  void OnLoadFinished(Android.Support.V4.Content.Loader loader, Object data)
		{
			Context context =mContext.Get();
			if (context == null)
			{
				return;
			}
            var cursor = data as ICursor;
			albumPhotoAdapter.SwapCursor(cursor);
        }

		public  void OnLoaderReset(Android.Support.V4.Content.Loader loader)
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
			load(album);
		}

		public virtual void load(Album target)
		{
			Bundle args = new Bundle();
			args.PutParcelable(ARGS_ALBUM, target);
			mLoaderManager.InitLoader(LOADER_ID, args, this);
		}

		public virtual void resetLoad(Album target)
		{
			Bundle args = new Bundle();
			args.PutParcelable(ARGS_ALBUM, target);
			mLoaderManager.RestartLoader(LOADER_ID, args, this);
		}

	}
}