using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LazyWelfare.AndroidMobile.Views.Partials;

namespace LazyWelfare.AndroidMobile.Views
{
    public class WebViews
    {
        static Dictionary<string, string> _ViewKeys = null;

        static WebViews()
        {
            var ass = Assembly.GetAssembly(typeof(WebViews));
            var types = GetInterfaceTypes<IsView>(ass);
            _ViewKeys = types.ToDictionary<Type, string, string>(s => s.Name, s => s.FullName);
        }

        static ConcurrentDictionary<string, IsView> Views = new ConcurrentDictionary<string, IsView>();

        static string CreateView(string name)
        {
            var ass = Assembly.GetAssembly(typeof(WebViews));
            var typeName = _ViewKeys[name];
            var instance = ass.CreateInstance(typeName);
            var view = instance as IsView;
            if (view == null) return string.Empty;
            Views.TryAdd(name, view);
            return view.GenerateString();
        }

        static string CreateView<T>(string name, T model)
        {
            var ass = Assembly.GetAssembly(typeof(WebViews));
            var typeName = _ViewKeys[name];
            var instance = ass.CreateInstance(typeName);
            var view = instance as IsView<T>;
            Views.TryAdd(name, view);
            view.Model = model;
            return view.GenerateString();
        }

        public static string Get(string viewName)
        {
            if (_ViewKeys.Keys.Contains(viewName) == false) return string.Empty;
            if (Views.Keys.Contains(viewName) == false) return CreateView(viewName);
            IsView view = Views[viewName];
            return view.GenerateString();
        }

        public static string Get<T>(string viewName, T model)
        {
            if (_ViewKeys.Keys.Contains(viewName) == false) return string.Empty;
            if (Views.Keys.Contains(viewName) == false) return CreateView<T>(viewName, model);
            IsView<T> view = Views[viewName] as IsView<T>;
            view.Model = model;
            return view.GenerateString();
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

    }

}