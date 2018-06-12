﻿namespace LazyWelfare.AndroidMobile.Views.Partials
{

    using LazyWelfare.AndroidMobile.Models;
    using LazyWelfare.AndroidMobile.WebAgreement;

    public partial class HostIndexView : IPartialView<HostModel>
    {

        public static AgreementUri Partial = new AgreementUri(nameof(HostIndexView), nameof(PartialHost), typeof(HostIndexView).Name);

        public void PushRequest(PartialActivity context, string args) => context.StackClearPush(Partial.Name, args);

        public string GenerateString(string args)
        {
            this.Model = ActiveContext.Host;
            return GenerateString();
        }

    }
}