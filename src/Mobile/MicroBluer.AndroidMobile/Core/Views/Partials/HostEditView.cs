namespace MicroBluer.AndroidMobile.Views.Partials
{
    using System;
    using MicroBluer.AndroidMobile.Models;
    using MicroBluer.AndroidMobile.WebAgreement;
    public partial class HostEditView : IPartialView<HostModel>
    {
        public static AgreementUri Partial = new AgreementUri(nameof(HostEditView), nameof(PartialHost), typeof(HostEditView).Name);

        public void PushRequest(PartialActivity context, string args) => context.StackPush(Partial, args);

        public string GenerateString(string args)
        {
            this.Model = string.IsNullOrEmpty(args) ? new HostModel { Domain = Guid.NewGuid() } : ActiveContext.HostStore.Get(args);
            return GenerateString();
        }
    }

}