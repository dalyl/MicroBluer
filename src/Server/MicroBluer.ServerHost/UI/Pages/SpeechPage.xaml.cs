using MicroBluer.ServerHost.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MicroBluer.ServerHost.UI.Pages
{
    /// <summary>
    /// SpeechPage.xaml 的交互逻辑
    /// </summary>
    public partial class SpeechPage : PageBase
    {
        public SpeechPage()
        {
            InitializeComponent();
            VolumeRangSeleclor.Minimum = 0;
            VolumeRangSeleclor.Maximum = 100;
            VolumeRangSeleclor.Value = 50;

            RateRangSeleclor.Minimum = -10;
            RateRangSeleclor.Maximum = 10;
            RateRangSeleclor.Value = 0;

            LoadAnnouncerData();
        }

        public void LoadAnnouncerData()
        {
            List<string> voicer = new List<string>();
            var Synth  = new SpeechSynthesizer();
            foreach (InstalledVoice iv in Synth.GetInstalledVoices())
            {
                voicer.Add(iv.VoiceInfo.Name);
            }

            AnnouncerSelector.ItemsSource = voicer;
        }

        bool Checked
        {
            get
            {
                if (AnnouncerSelector.SelectedItem == null) return false;
                var voicer = AnnouncerSelector.SelectedValue.ToString();
                if (string.IsNullOrEmpty(voicer)) return false;
                var content = InputContent.Text;
                if (string.IsNullOrEmpty(content)) return false;
                return true;
            }
        }


        private void Button_Read_Click(object sender, RoutedEventArgs e)
        {
            if (Checked == false) return;
            var voicer = AnnouncerSelector.SelectedValue.ToString();
            var content = InputContent.Text;
            var volume = VolumeRangSeleclor.Value;
            var rate = RateRangSeleclor.Value;
            VoiceReader.Speak(VoiceReader.Reader.Microsoft, voicer, content, volume, rate);
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (Checked == false) return;
            var voicer = AnnouncerSelector.SelectedValue.ToString();
            var content = InputContent.Text.Trim();
            var volume = VolumeRangSeleclor.TabIndex;
            var rate = RateRangSeleclor.TabIndex;
            var title = (content.Length > 100 ? content.Substring(0, 100) : content).Replace(" ", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}\\Record\\{title}.wav";
            VoiceReader.Generated(VoiceReader.Reader.Microsoft, voicer, content, path, volume, rate);
            MessageBox.Show($"保存录音文件成功，保存路径：{path}！");
        }

       
    }
}
