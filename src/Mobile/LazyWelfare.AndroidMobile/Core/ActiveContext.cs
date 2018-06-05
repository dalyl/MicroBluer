﻿namespace LazyWelfare.AndroidMobile
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Android.App;
    using Android.Widget;
    using LazyWelfare.AndroidMobile.AgreementServices;
    using LazyWelfare.AndroidMobile.Logic;
    using LazyWelfare.AndroidMobile.Models;

    public class ActiveContext
    {
        static ActiveContext Current {  get; set; }

        public static Activity Activity => Current.activity;

        public static UserModel User => Instantiate(UserStore.Get);

        public static HostModel Host => Instantiate(()=> string.IsNullOrEmpty(User.Host) ? null : ActiveContext.HostStore.Get(User.Host));

        public static HostStoreService HostStore => Instantiate<HostStoreService>();

        public static UserStoreService UserStore => Instantiate<UserStoreService>();

        public static FolderMapStoreService FolderMapStore => Instantiate<FolderMapStoreService>();

        public static AgreementService Agreement => Instantiate<AgreementService>();

        internal static HostExpressService HostExpress => Instantiate(() => new HostExpressService(Host));

        public static TryCatch Try { get; } = new TryCatch(ShowMessage);

        public static void RegisterContext(ActiveContext context)
        {
            if (Current == null) Current = context;
        }

        public static void  RepalceContext(ActiveContext context)
        {
            if (context != Current) Current = context;
        }

        public static void ExprieContext() => Current = null;

        public static void ExpireSercvice<T>(string name = "")
        {
            Current.DeleteSercvice<T>(name);
        }

        static void ShowMessage(string message)
        {
            Activity.RunOnUiThread(() => Toast.MakeText(Activity, message.Trim(), ToastLength.Short).Show());
        }

        static T Instantiate<T>(string name = "", Action after = null) where T : class, new()
        {
            return Instantiate<T>(() => new T(), name);
        }

        static T Instantiate<T>(Func<T> fetchInstance, string name = "", Action after = null) where T : class
        {
            var key = string.IsNullOrEmpty(name) ? typeof(T).Name : name;
            if (fetchInstance == null) Try.Throw<T>($"{ key } 实例化方法未提供");
            if (Current.ServiceInstances.Keys.Contains(key))
            {
                var instance = Current.ServiceInstances[key] as T;
                if (instance == null)
                {
                    Current.ServiceInstances[key] = fetchInstance();
                    after?.Invoke();
                }
                return Current.ServiceInstances[key] as T;
            }
            else
            {
                var instance = fetchInstance();
                after?.Invoke();
                Current.ServiceInstances.Add(key, instance);
                return instance;
            }
        }

        Dictionary<string, object> ServiceInstances = new Dictionary<string, object>();

        Activity activity = null;

        public ActiveContext(Activity context)
        {
            activity = context;
        }
      
        private void DeleteSercvice<T>(string name = "")
        {
            var key = string.IsNullOrEmpty(name) ? typeof(T).Name : name;
            if (ServiceInstances.Keys.Contains(key)) ServiceInstances.Remove(key);
        }

    }


}