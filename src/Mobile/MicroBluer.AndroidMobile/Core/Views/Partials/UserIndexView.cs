﻿namespace MicroBluer.AndroidMobile.Views.Partials
{
    using MicroBluer.AndroidMobile.Models;
    using MicroBluer.AndroidMobile.WebAgreement;

    public partial class UserIndexView : IPartialView<UserModel>
    {

        public static AgreementUri Partial = new AgreementUri(nameof(UserIndexView), nameof(PartialHost), typeof(UserIndexView).Name);

        public void PushRequest(PartialActivity context, string args) => context.StackClearPush(Partial, args);

        public string GenerateString(string args)
        {
            this.Model = ActiveContext.User;
            return GenerateString();
        }

    }
}