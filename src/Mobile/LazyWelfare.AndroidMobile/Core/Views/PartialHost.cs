namespace LazyWelfare.AndroidMobile.Views
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class PartialHost
    {

        static Dictionary<string, string> _ViewKeys = null;
        static ConcurrentDictionary<string, IPartialView> Views = new ConcurrentDictionary<string, IPartialView>();

        static PartialHost()
        {
            var ass = Assembly.GetAssembly(typeof(PartialHost));
            var types = GetInterfaceTypes<IPartialView>(ass);
            _ViewKeys = types.ToDictionary<Type, string, string>(s => s.Name, s => s.FullName);
        }

        static IEnumerable<Type> GetInterfaceTypes<T>(Assembly assembly)
        {
            var interfaceType = typeof(T);
            var fullName = interfaceType.FullName;
            var findedTypes = new List<Type>();
            Type[] types = null;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types;
            }
            foreach (var type in types.Where(t => t != null))
            {
                var interfaces = type.GetInterfaces();
                if (type.IsClass && !type.IsAbstract && interfaces.Length > 0)
                {
                    var iexist = false;
                    foreach (var one in interfaces)
                    {
                        iexist = iexist || one.FullName == fullName;

                    }
                    if (iexist) findedTypes.Add(type);
                }
            }
            return findedTypes;
        }

        static IPartialView GetInstance(string viewName)
        {
            if (Views.Keys.Contains(viewName) == false)
            {
                var ass = Assembly.GetAssembly(typeof(PartialHost));
                var typeName = _ViewKeys[viewName];
                var instance = ass.CreateInstance(typeName);
                var view = instance is IPartialView ? instance as IPartialView : null;
                Views.TryAdd(viewName, view);
                return view;
            }
            return Views[viewName];
        }

        public static string Dispatch(PartialActivity context, string url, string args)
        {
            if (_ViewKeys.Keys.Contains(url) == false) return string.Empty;
            var view = GetInstance(url);
            if (view == null) return string.Empty;
            view.PushRequest(context, args);
            return view.GenerateString(args);
        }
    }

}