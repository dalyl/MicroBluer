using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace LazyWelfare.AndroidMobile.Utils
{
    public static class CollectionExtension
    {
        public static IList<IParcelable> AsIParcelableList<T>(this IList<T> models) where T : IParcelable
        {
            //var list = new List<IParcelable>();
            //foreach (var item in models)
            //{
            //    list.Add(item);
            //}
            //return list;
            return models.Select(s => s as IParcelable).ToList();
        }
    }
}