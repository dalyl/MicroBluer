namespace MicroBluer.AndroidMobile.AgreementServices
{
    using System;
    using Android.Content;
    using Newtonsoft.Json;
    using MicroBluer.AndroidCtrls.ImageAction;
    using MicroBluer.AndroidCtrls.ImageAction.Renderer;
    using MicroBluer.AndroidUtils;

    public class VolumeService: IAgreementService
    {
        const string GetCommand = "volume/get";
        const string SetCommand = "volume/set/{0}";

        public ImageActionDialog View { get; private set; }

        public decimal Current { get; private set; }

        public VolumeService(Context context)
        {
            View = new ImageActionDialog(context)
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
            if (View == null) return ActiveContext.Try.Throw<bool>("控件视图没有正确实例化");
            View.Show();
            return true;
        }
    }
}