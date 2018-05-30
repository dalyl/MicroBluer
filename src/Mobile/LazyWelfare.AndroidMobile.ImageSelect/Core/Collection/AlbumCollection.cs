namespace LazyWelfare.AndroidMobile.ImageSelect.Collection
{
    using Loader= Android.Support.V4.Content.Loader;
    using Object = Java.Lang.Object;
    using System;
    using Android.Database;
    using Android.Content;
    using Android.OS;
    using Android.Views;
    using Android.Widget;
    using Android.Support.V4.App;
    using LazyWelfare.AndroidMobile.ImageSelect.Model;
    using LazyWelfare.AndroidMobile.ImageSelect.Loader;
    using LazyWelfare.AndroidMobile.ImageSelect.Adapter;
    using LazyWelfare.AndroidUtils.Common;
    using LazyWelfare.AndroidUtils.Extension;

    public class AlbumCollection : Object, LoaderManager.ILoaderCallbacks, AdapterView.IOnItemClickListener
    {
        private const int LOADER_ID = 1;
        private static readonly string STATE_CURRENT_SELECTION = BundleUtils.BuildKey<AlbumCollection>("STATE_CURRENT_SELECTION");
        private WeakReference<Context> mContext;
        private LoaderManager mLoaderManager;
        private IOnDirectorySelectListener directorySelectListener;
        private SelectionSpec selectionSpec;
        private AlbumAdapter albumAdapter;

        public virtual int CurrentSelection { get; set; }

        public Loader OnCreateLoader(int id, Bundle args)
        {
            Context context = mContext.Get();
            if (context == null)
            {
                return null;
            }
            return AlbumLoader.NewInstance(context, selectionSpec);
        }

        public void OnLoaderReset(Loader loader)
        {
            Context context = mContext.Get();
            if (context == null)
            {
                return;
            }
            albumAdapter.SwapCursor(null);
        }

        public void OnLoadFinished(Loader loader, Object data)
        {
            Context context = mContext.Get();
            if (context == null)
            {
                return;
            }
            var cursor = data as ICursor;
            albumAdapter.SwapCursor(cursor);
            HandlerUtils.MainHandler.Post(() =>
            {
                if (cursor.Count > 0)
                {
                    cursor.MoveToFirst();
                    Album currentAlbum = Album.ValueOf(cursor);
                    if (directorySelectListener != null)
                    {
                        directorySelectListener.OnReset(currentAlbum);
                    }
                }
            });
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            if (directorySelectListener != null)
            {
                var cursor = (ICursor)parent.GetItemAtPosition(position);
                Album album = Album.ValueOf(cursor);
                directorySelectListener.OnSelect(album);
            }
        }

        public virtual void OnCreate(FragmentActivity activity, IOnDirectorySelectListener directorySelectListener, SelectionSpec selectionSpec, ListView listView)
        {
            mContext = new WeakReference<Context>(activity);
            mLoaderManager = activity.SupportLoaderManager;
            this.directorySelectListener = directorySelectListener;
            this.selectionSpec = selectionSpec;
            albumAdapter = new AlbumAdapter(activity, null);
            listView.Adapter = albumAdapter;
            listView.OnItemClickListener = this;
        }

        public virtual void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            if (savedInstanceState == null)
            {
                return;
            }
            CurrentSelection = savedInstanceState.GetInt(STATE_CURRENT_SELECTION);
        }

        public virtual void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt(STATE_CURRENT_SELECTION, CurrentSelection);
        }

        public virtual void OnDestroy()
        {
            mLoaderManager.DestroyLoader(LOADER_ID);
            directorySelectListener = null;
        }

        public virtual void LoadAlbums()
        {
            mLoaderManager.InitLoader(LOADER_ID, null, this);
        }

        public virtual void ResetLoadAlbums()
        {
            mLoaderManager.RestartLoader(LOADER_ID, null, this);
        }

        public interface IOnDirectorySelectListener
        {
            void OnSelect(Album album);

            void OnReset(Album album);
        }

    }

}