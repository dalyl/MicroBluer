﻿namespace MicroBluer.AndroidCtrls.ImageExpleror.Collection
{
    using Object = Java.Lang.Object;
    using Android.Net;
    using Android.OS;
    using MicroBluer.AndroidCtrls.ImageExpleror.Engine;
    using MicroBluer.AndroidCtrls.ImageExpleror.Model;
    using MicroBluer.AndroidUtils.Common;
    using System.Collections.Generic;

    public class SelectedUriCollection: List<Uri>
    {
        private static string STATE_SELECTION = BundleUtils.BuildKey<SelectedUriCollection>("STATE_SELECTION");
        private static string STATE_SELECTION_POSITION = BundleUtils.BuildKey<SelectedUriCollection>("STATE_SELECTION_POSITION");
        private SelectionSpec mSpec;
        private IOnSelectionChange onSelectionChange;
  
        public void OnCreate(Bundle savedInstanceState)
        {
            if (savedInstanceState != null)
            {
                List<Uri> saved = savedInstanceState.GetParcelableArrayList(STATE_SELECTION) as List<Uri>;
                this.AddRange(saved);
            }
        }

        public void PrepareSelectionSpec(SelectionSpec spec)
        {
            mSpec = spec;
        }

      
        public void OnSaveInstanceState(Bundle outState)
        {
            var list = this as IEnumerable<IParcelable>;
            outState.PutParcelableArrayList(STATE_SELECTION, new List<IParcelable>(list));
        }

        public bool IsEmpty
        {
            get {
                return Count == 0;
            }
        }

        public bool IsSelected(Uri uri)
        {
            return this.Contains(uri);
        }

        public bool IsCountInRange()
        {
            return mSpec.MinSelectable  <= Count && Count <= mSpec.MaxSelectable;
        }

        public bool IsCountOver()
        {
            return Count >= mSpec.MaxSelectable;
        }

        public int MaxCount
        {
            get {
                return mSpec.MaxSelectable;
            }
        }

        public bool IsSingleChoose
        {
            get {
                return mSpec.IsSingleChoose;
            }
        }

        public LoadEngine Engine
        {
            get {

                return mSpec.Engine;
            }
        }

        public void SetOnSelectionChange(IOnSelectionChange onSelectionChange)
        {
            this.onSelectionChange = onSelectionChange;
        }
        

        public interface IOnSelectionChange
        {
            void OnChange(int maxCount, int selectCount);
        }
    }
}