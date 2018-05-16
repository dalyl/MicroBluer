using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Database;
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
using LazyWelfare.AndroidMobile.ImageSelect.Loader;
using LazyWelfare.AndroidMobile.Utils;
using Java.Lang;
using LazyWelfare.AndroidMobile.ImageSelect.Adapter;

namespace LazyWelfare.AndroidMobile.ImageSelect.Control
{

    public class AlbumCollection : Java.Lang.Object, Android.Support.V4.App.LoaderManager.ILoaderCallbacks, AdapterView.IOnItemClickListener
    {
        private const int LOADER_ID = 1;
        private static readonly string STATE_CURRENT_SELECTION = BundleUtils.BuildKey<AlbumCollection>("STATE_CURRENT_SELECTION");
        private System.WeakReference<Context> mContext;
        private Android.Support.V4.App.LoaderManager mLoaderManager;
        private OnDirectorySelectListener directorySelectListener;
        private int mCurrentSelection;
        private SelectionSpec selectionSpec;
        private AlbumAdapter albumAdapter;

        public Android.Support.V4.Content.Loader OnCreateLoader(int id, Bundle args)
        {
            Context context = mContext.Get();
            if (context == null)
            {
                return null;
            }
            return AlbumLoader.NewInstance(context, selectionSpec);
        }

        public void OnLoaderReset(Android.Support.V4.Content.Loader loader)
        {
            Context context = mContext.Get();
            if (context == null)
            {
                return;
            }
            albumAdapter.SwapCursor(null);
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: @Override public void onLoadFinished(android.support.v4.content.Loader<android.database.Cursor> loader, final android.database.Cursor data)
        public void OnLoadFinished(Android.Support.V4.Content.Loader loader, Object data)
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

        public virtual void onCreate(FragmentActivity activity, OnDirectorySelectListener directorySelectListener, SelectionSpec selectionSpec, ListView listView)
        {
            mContext = new System.WeakReference<Context>(activity);
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
            mCurrentSelection = savedInstanceState.GetInt(STATE_CURRENT_SELECTION);
        }

        public virtual void onSaveInstanceState(Bundle outState)
        {
            outState.PutInt(STATE_CURRENT_SELECTION, mCurrentSelection);
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


        public virtual int CurrentSelection
        {
            get
            {
                return mCurrentSelection;
            }
        }

        public virtual int StateCurrentSelection
        {
            set
            {
                mCurrentSelection = value;
            }
        }



        public interface OnDirectorySelectListener
        {
            void OnSelect(Album album);

            void OnReset(Album album);
        }

    }

}