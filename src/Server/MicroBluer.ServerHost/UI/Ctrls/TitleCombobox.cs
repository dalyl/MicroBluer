using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MicroBluer.ServerHost.UI.Ctrls
{
    [TemplatePart(Name = TitleTextBlockKey, Type = typeof(ComboBox))]
    public  class TitleCombobox: ComboBox
    {
        private const string TitleTextBlockKey = "PART_TitleTextBlock";

        private TextBlock _tbkTitle;

        public static readonly DependencyProperty TitleProperty;

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        static TitleCombobox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TitleCombobox), new FrameworkPropertyMetadata(typeof(TitleCombobox)));

            TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TitleCombobox), new UIPropertyMetadata(new PropertyChangedCallback(TitlePropertyChangedCallback)));
        }

        private static void TitlePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TitleCombobox ttb = d as TitleCombobox;

            if (ttb._tbkTitle != null)
            {
                ttb._tbkTitle.Text = (string)e.NewValue;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _tbkTitle = Template.FindName(TitleTextBlockKey, this) as TextBlock;
            _tbkTitle.Text = Title;
        }
    }
}
