using System;
using System.Windows.Markup;

namespace MicroBluer.Windows.Notification.Converters
{
    public abstract class BaseConverter : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
