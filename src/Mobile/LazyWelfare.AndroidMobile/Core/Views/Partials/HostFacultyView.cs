namespace LazyWelfare.AndroidMobile.Views.Partials
{
    using System;
    using LazyWelfare.AndroidMobile.Models;
    using LazyWelfare.AndroidMobile.WebAgreement;

    public partial class HostFacultyView : IPartialView<HostModel>
    {
        public const string Placeholder_Append = "#############Append#############";

        public static AgreementUri Partial = new AgreementUri(nameof(HostFacultyView), nameof(PartialHost), typeof(HostFacultyView).Name);

        public void PushRequest(PartialActivity context, string args) { }

        public string GenerateString(string args)
        {
            this.Model = string.IsNullOrEmpty(args) ? new HostModel { Domain = Guid.NewGuid() } : ActiveContext.HostStore.Get(args);
            if (Model == null) return ActiveContext.Try.Show(string.Empty, "获取服务主机信息失败");
            var content = GenerateString();
            var append = ActiveContext.HostExpress.GetPageContent("command-panel");
            return content.Replace(Placeholder_Append, append);
        }
    }
}