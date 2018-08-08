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
        public SpeechSynthesizer Synth { get; } = new SpeechSynthesizer();
        public SpeechPage()
        {
            InitializeComponent();
            LoadAnnouncerData();
        }

        public void LoadAnnouncerData()
        {
            List<string> voicer = new List<string>();
            foreach (InstalledVoice iv in Synth.GetInstalledVoices())
            {
                voicer.Add(iv.VoiceInfo.Name);
            }

            //选择不同的发音
            //synth.SelectVoice("Microsoft Anna");//美式发音，但只能读英文
            //synth.SelectVoice("Microsoft Lili");//能读中英文
            //语音识别
            //SpeechRecognitionEngine sre = new SpeechRecognitionEngine();

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
            var content = InputContent.Text;
            Synth.Speak(content);
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (Checked == false) return;
            var content = InputContent.Text;

            Synth.Volume = VolumeRangSeleclor.TabIndex;
            Synth.Rate = RateRangSeleclor.TabIndex;

            Synth.SetOutputToWaveFile("D:\\Record.wav");
            Synth.Speak(content);
            Synth.SetOutputToDefaultAudioDevice();
            MessageBox.Show("保存录音文件成功，保存路径：D:\\Record.wav！");
            Synth.Dispose();
        }

        private void AnnouncerSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                object obj = (object)e.AddedItems;
                string str = Convert.ToString(((System.Data.DataRowView)(((object[])(obj))[0])).Row.ItemArray[1]);
                Synth.SelectVoice(str);
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
