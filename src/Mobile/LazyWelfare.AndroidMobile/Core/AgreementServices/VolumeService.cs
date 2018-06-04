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
using LazyWelfare.AndroidAreaView.Core;
using LazyWelfare.AndroidAreaView.Core.Renderer;
using LazyWelfare.AndroidMobile.Logic;
using LazyWelfare.Interface;
using Newtonsoft.Json;

namespace LazyWelfare.AndroidMobile.AgreementServices
{
    public class VolumeService: IAgreementService
    {
        const string GetCommand = "volume/get";
        const string SetCommand = "volume/set/{0}";

        public MapCtrlDialog View { get; private set; }

        public decimal Current { get; private set; }

        public VolumeService(Context context)
        {
            View = new MapCtrlDialog(context)
            {
                Render = new VolumeControllerRenderer(context)
                {
                    OnClickTop = () => SetValue(0.01m),
                    OnClickBottom = () => SetValue(-0.01m),
                    FetchText = () => $"音量：{(int)(Current * 100)}%",
                }
            };
            Current = GetValue();
        }

        decimal GetValue()
        {
            var data = ActiveContext.HostExpress.GetCommandResult(GetCommand);
            if (string.IsNullOrEmpty(data)) return ActiveContext.Try.Throw<decimal>("服务接口数据为空");
            var result = JsonConvert.DeserializeObject<TaskResult<decimal>>(data);
            if(result==null) return ActiveContext.Try.Throw<decimal>("服务接口数据格式转换异常");
            return Convert.ToDecimal(result.Content);
        }

        void SetValue(decimal value)
        {
            Current += value;
            var result = ActiveContext.HostExpress.SendCommand(string.Format(SetCommand, Current));
            if (result == false) Current -= value;
        }

        public bool Execute()
        {
            if (View != null) return ActiveContext.Try.Throw<bool>("控件视图没有正确实例化");
            View.Show();
            return true;
        }
    }
}