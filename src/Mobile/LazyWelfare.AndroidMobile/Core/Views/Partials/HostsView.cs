

namespace LazyWelfare.AndroidMobile.Views.Partials
{
    using System.Collections.Generic;
    using LazyWelfare.AndroidMobile.Models;
    using LazyWelfare.AndroidMobile.WebAgreement;

    public partial class HostsView :  IPartialView<List<HostModel>>
    {
        public static AgreementUri Partial = new AgreementUri(nameof(HostsView), nameof(PartialHost), typeof(HostsView).Name);

        public void PushRequest(PartialActivity context, string args) => context.StackPush(Partial.Name, args);
    
        public string GenerateString(string args)
        {
            this.Model = ActiveContext.HostStore.GetList();
            return GenerateString();
        }
    }
}