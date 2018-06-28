
namespace MicroBluer.AndroidMobile.Views.Partials
{
    using MicroBluer.AndroidMobile.WebAgreement;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public partial class FolderMapExplerorView : IPartialView<List<string>>
    {

        public static AgreementUri Partial = new AgreementUri(nameof(FolderMapExplerorView), nameof(PartialHost), typeof(FolderMapExplerorView).Name);

        public void PushRequest(PartialActivity context, string args) => context.StackPush(Partial, args);

        public string GenerateString(string args)
        {
            this.Model = JsonConvert.DeserializeObject<List<string>>(args);
            if (this.Model == null) this.Model = new List<string>();
            return GenerateString();
        }

    }
}