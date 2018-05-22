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
using LazyWelfare.AndroidMobile.Views.Partials;

namespace LazyWelfare.AndroidMobile
{
    public class WebPartialRequestStack
    {
        public class ActionValue
        {
            public ActionValue() { }
            public ActionValue(PartialView view,string args)
            {
                Partial = view;
                Args = args;
            }
            public PartialView Partial { get; set; }
            public string Args { get; set; }
        }

        Stack<ActionValue> RequestStacks { get; set; } = new Stack<ActionValue>();

        public bool IsEmpty
        {
            get
            {
              return  RequestStacks.Count == 0;
            }
        }

        public void Clear()
        {
            RequestStacks.Clear();
        }

        public void Push(PartialView partial, string args)
        {
            var value = new ActionValue(partial, args);
            RequestStacks.Push(value);
        }

        public ActionValue Fetch()
        {
            return RequestStacks.Pop();
        }
    }
}