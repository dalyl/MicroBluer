namespace LazyWelfare.AndroidUtils.Extension
{
    using System.Collections.Generic;
    using System.Linq;
    using Android.OS;

    public static class CollectionExtension
    {
        public static IList<IParcelable> AsIParcelableList<T>(this IList<T> models) where T : IParcelable
        {
            return models.Select(s => s as IParcelable).ToList();
        }
    }
}