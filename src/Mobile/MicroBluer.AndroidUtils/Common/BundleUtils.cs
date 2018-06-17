namespace MicroBluer.AndroidUtils.Common
{
    using System;

    public class BundleUtils
    {
        public static String BuildKey<T>(string name)
        {
            return $"{typeof(T).Namespace}.{typeof(T).Name}.{name}";
        }
    }
}