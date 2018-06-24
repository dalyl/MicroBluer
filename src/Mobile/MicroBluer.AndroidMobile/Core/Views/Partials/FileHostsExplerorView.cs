namespace MicroBluer.AndroidMobile.Views.Partials
{
    using System.IO;
    using MicroBluer.AndroidMobile.WebAgreement;

    public partial  class FileHostsExplerorView : IPartialView<string>
    {
        public static AgreementUri Partial = new AgreementUri(nameof(FileHostsExplerorView), nameof(PartialHost), typeof(FileHostsExplerorView).Name);

        public void PushRequest(PartialActivity context, string args) => context.StackPush(Partial.Name, args);

        public string GenerateString(string args)
        {
            var path = @"/system/etc/hosts";
            if (File.Exists(path)) this.Model = File.ReadAllText(path);
            return GenerateString();
        }
    }
}