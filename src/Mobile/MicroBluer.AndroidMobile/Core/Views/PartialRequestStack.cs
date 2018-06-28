namespace MicroBluer.AndroidMobile
{
    using MicroBluer.AndroidMobile.WebAgreement;
    using System.Collections.Generic;

    public class PartialRequestStack
    {

        Stack<AgreementUri> RequestStacks { get; set; } = new Stack<AgreementUri>();

        public bool IsEmpty
        {
            get
            {
              return  RequestStacks.Count < 2;
            }
        }

        public void Clear()
        {
            RequestStacks.Clear();
        }

        public void Push(AgreementUri uri, string args)
        {
            RequestStacks.Push(new AgreementUri (uri, args));
        }
        public void Push(AgreementUri uri)
        {
            RequestStacks.Push(uri);
        }

        public AgreementUri Fetch()
        {
            if (IsEmpty) return AgreementUri.Empty;
            RequestStacks.Pop();
            return RequestStacks.Pop();
        }
    }
}