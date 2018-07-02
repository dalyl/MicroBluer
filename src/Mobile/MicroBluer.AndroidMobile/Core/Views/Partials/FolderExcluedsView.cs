namespace MicroBluer.AndroidMobile.Views.Partials
{
    using System;
    using System.Collections.Generic;
    using MicroBluer.AndroidMobile.Models;
    using MicroBluer.AndroidMobile.WebAgreement;

    public  partial class FolderExcluedsView : IPartialView<List<FolderExcludeModel>>
    {
        public static AgreementUri Partial = new AgreementUri(nameof(FolderExcluedsView), nameof(PartialHost), typeof(FolderExcluedsView).Name);

        public void PushRequest(PartialActivity context, string args) => context.StackPush(Partial, args);

        public List<FolderExcludeModel> Model { get; set; }

        public string GenerateString(string args)
        {
            this.Model = ActiveContext.FolderExcludeStore.GetList();
            return GenerateString();
        }
    }
}