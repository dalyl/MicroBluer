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
using MicroBluer.Interface;

namespace MicroBluer.AndroidMobile.AgreementServices
{
    public sealed class AgreementService
    {
        static Dictionary<string, Func<Context, IAgreementService>> Servces = new Dictionary<string, Func<Context, IAgreementService>>();

        static AgreementService()
        {
            Servces.Add(nameof(IVolumeController), context => new VolumeService(context));
            Servces.Add(nameof(IComputerCloseFire), context => new CloseFireService(context));
        }

        public bool Contains(string service) => Servces.Keys.Contains(service);

        public bool Execute(string name, Context context)
        {
            if (Contains(name) == false) return ActiveContext.Try.Throw<bool>($"{name} 服务未不属于协议服务或未被正确注册");
            var fetchService = Servces[name];
            var service = fetchService(context);
            service.Execute();
            return true;
        }
    }
     
   
    public interface IComputerCloseFire
    {

    }

  
}