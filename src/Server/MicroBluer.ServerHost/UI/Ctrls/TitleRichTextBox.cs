using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MicroBluer.ServerHost.UI.Ctrls
{
    [TemplatePart(Name = TitleTextBlockKey, Type = typeof(RichTextBox))]
    public  class TitleRichTextBox: RichTextBox
    {
        private const string TitleTextBlockKey = "PART_TitleTextBlock";

        private TextBlock _tbkTitle;

        public static readonly DependencyProperty TitleProperty;

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        static TitleRichTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TitleRichTextBox), new FrameworkPropertyMetadata(typeof(TitleRichTextBox)));

            TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TitleRichTextBox), new UIPropertyMetadata(new PropertyChangedCallback(TitlePropertyChangedCallback)));
        }

        private static void TitlePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TitleRichTextBox ttb = d as TitleRichTextBox;

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
