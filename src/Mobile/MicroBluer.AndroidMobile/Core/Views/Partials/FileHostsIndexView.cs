using System;
namespace MicroBluer.AndroidMobile.Views.Partials
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Runtime;
    using Android.Views;
    using Android.Widget;
    using MicroBluer.AndroidMobile.WebAgreement;

    public partial class FileHostsIndexView: IPartialView
    {
        public static AgreementUri Partial = new AgreementUri(nameof(FileHostsIndexView), nameof(PartialHost), typeof(FileHostsIndexView).Name);

        public void PushRequest(PartialActivity context, string args) => context.StackClearPush(Partial.Name, args);

        public string GenerateString(string args)
        {
            return GenerateString();
        }

    }
}