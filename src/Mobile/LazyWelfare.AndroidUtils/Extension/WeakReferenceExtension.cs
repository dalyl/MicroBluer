namespace LazyWelfare.AndroidUtils.Extension
{
    using System;

    public static class WeakReferenceExtension
    {
        public static T Get<T>(this WeakReference<T> model_ref) where T : class
        {
            T model = default(T);
            model_ref.TryGetTarget(out model);
            return model;
        }

    }
}