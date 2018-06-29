
namespace MicroBluer.AndroidMobile.Views.Partials
{
    using MicroBluer.AndroidMobile.Models;
    using MicroBluer.AndroidMobile.WebAgreement;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public partial class FolderMapExplerorView : IPartialView<(FolderKind kind, List<string> dirs)>
    {

        public static AgreementUri Partial = new AgreementUri(nameof(FolderMapExplerorView), nameof(PartialHost), typeof(FolderMapExplerorView).Name);

        public (FolderKind kind, List<string> dirs) Model { get; set; }

        public void PushRequest(PartialActivity context, string args) => context.StackPush(Partial, args);

        public string GenerateString(string args)
        {
            this.Model = JsonConvert.DeserializeObject <(FolderKind kind, List<string> dirs)> (args);
            return GenerateString();
        }

    }
}