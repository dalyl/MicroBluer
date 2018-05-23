using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LazyWelfare.AndroidMobile.Views;

namespace LazyWelfare.AndroidMobile
{
    public class WebPartialRequestStack
    {
        public class ActionValue
        {
            public ActionValue() { }
            public ActionValue(string view,string args)
            {
                Partial = view;
                Args = args;
            }
            public string Partial { get; set; }
            public string Args { get; set; }
        }

        Stack<ActionValue> RequestStacks { get; set; } = new Stack<ActionValue>();

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

        public void Push(string partial, string args)
        {
            var value = new ActionValue(partial, args);
            RequestStacks.Push(value);
        }

        public ActionValue Fetch()
        {
            if (IsEmpty) return null;
            RequestStacks.Pop();
            return RequestStacks.Pop();
        }
    }
}