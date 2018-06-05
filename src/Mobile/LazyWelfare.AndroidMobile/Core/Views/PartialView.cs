namespace LazyWelfare.AndroidMobile.Views
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using LazyWelfare.AndroidMobile.Logic;
    using LazyWelfare.AndroidMobile.Models;
    using LazyWelfare.AndroidMobile.Views.Partials;

    public  class PartialView
    {

        public const string None = "";
        public const string HomeViewPartial = nameof(HomeView);
        public const string HostsViewPartial = nameof(HostsView);
        public const string HostDetailViewPartial = nameof(HostDetailView);
        public const string HostFacultyViewPartial = nameof(HostFacultyView);
        public const string FoldersViewPartial = nameof(FoldersView);


        static Dictionary<string, string> _ViewKeys = null;

        static PartialView()
        {
            var ass = Assembly.GetAssembly(typeof(PartialView));
            var types = GetInterfaceTypes<IPartialView>(ass);
            _ViewKeys = types.ToDictionary<Type, string, string>(s => s.Name, s => s.FullName);
        }

        static ConcurrentDictionary<string, IPartialView> Views = new ConcurrentDictionary<string, IPartialView>();

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

        static string CreateView(string name)
        {
            var ass = Assembly.GetAssembly(typeof(PartialView));
            var typeName = _ViewKeys[name];
            var instance = ass.CreateInstance(typeName);
            if (!(instance is IPartialView view)) return string.Empty;
            Views.TryAdd(name, view);
            return view.GenerateString();
        }

        public static string Get(string viewName)
        {
            if (_ViewKeys.Keys.Contains(viewName) == false) return string.Empty;
            if (Views.Keys.Contains(viewName) == false) return CreateView(viewName);
            IPartialView view = Views[viewName];
            return view.GenerateString();
        }


        static string CreateView<T>(string name, string args)
        {
            var ass = Assembly.GetAssembly(typeof(PartialView));
            var typeName = _ViewKeys[name];
            var instance = ass.CreateInstance(typeName);
            if (!(instance is IPartialView<T> view)) return string.Empty;
            Views.TryAdd(name, view);
            view.Model = view.GetModel(args);
            if (view.Model == null) return view.GenerateStringWithoutModel();
            return view.GenerateString();
        }

        static string CreateView<T>(string name, T model)
        {
            var ass = Assembly.GetAssembly(typeof(PartialView));
            var typeName = _ViewKeys[name];
            var instance = ass.CreateInstance(typeName);
            if (!(instance is IPartialView<T> view)) return string.Empty;
            Views.TryAdd(name, view);
            view.Model = model;
            if (view.Model == null) return view.GenerateStringWithoutModel();
            return view.GenerateString();
        }

        public static string Get<T>(string viewName,string args)
        {
            if (_ViewKeys.Keys.Contains(viewName) == false) return string.Empty;
            if (Views.Keys.Contains(viewName) == false) return CreateView<T>(viewName, args);
            IPartialView<T> view = Views[viewName] as IPartialView<T>;
            view.Model = view.GetModel(args);
            if (view.Model == null) return view.GenerateStringWithoutModel();
            return view.GenerateString();
        }

        static string Get<T>(string viewName, T model)
        {
            if (_ViewKeys.Keys.Contains(viewName) == false) return string.Empty;
            if (Views.Keys.Contains(viewName) == false) return CreateView<T>(viewName, model);
            IPartialView<T> view = Views[viewName] as IPartialView<T>;
            view.Model = model;
            if (view.Model == null) return view.GenerateStringWithoutModel();
            return view.GenerateString();
        }

        public static string SwitchWebView(PartialActivity context, string url, string args)
        {
            if (_ViewKeys.Keys.Contains(url) == false) return string.Empty;

            switch (url)
            {
                case PartialView.HomeViewPartial:
                    {
                        context.RequestStack.Clear();
                        context.RequestStack.Push(PartialView.HomeViewPartial, "");
                        return Get<HostModel>(typeof(HomeView).Name, args);
                    }
                case PartialView.HostsViewPartial:
                    {
                        context.RequestStack.Push(PartialView.HostsViewPartial, "");
                        return Get<List<HostModel>>(typeof(HostsView).Name, args);
                    }
                case PartialView.HostDetailViewPartial:
                    {
                        context.RequestStack.Push(PartialView.HostsViewPartial, args);
                        return Get<HostModel>(typeof(HostDetailView).Name, args);
                    }
                case PartialView.HostFacultyViewPartial:
                    {
                        var page = Get<HostModel>(typeof(HostFacultyView).Name, args);
                        var append = ActiveContext.HostExpress.GetPageContent("command-panel");
                        return page.Replace(HostFacultyView.Placeholder_Append, append);
                    }
                case PartialView.FoldersViewPartial:
                    {
                        context.RequestStack.Push(PartialView.FoldersViewPartial, args);
                        return Get<List<FolderMapModel>>(typeof(FoldersView).Name, args);
                    }
            }
            return string.Empty;
        }
    }

}