using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp.Ctrls
{
    

    [TemplatePart(Name = TitleTextBlockKey, Type = typeof(TextBlock))]
    public class TitleTextBox : TextBox
    {
        private const string TitleTextBlockKey = "PART_TitleTextBlock";

        private TextBlock _tbkTitle;

        public static readonly DependencyProperty TitleProperty;

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        static TitleTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TitleTextBox), new FrameworkPropertyMetadata(typeof(TitleTextBox)));

            TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TitleTextBox), new UIPropertyMetadata(new PropertyChangedCallback(TitlePropertyChangedCallback)));
        }

        private static void TitlePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TitleTextBox ttb = d as TitleTextBox;

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
