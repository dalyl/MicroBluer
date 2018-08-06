using System;
using System.Speech.Synthesis; //用于语音朗读
using System.Speech.Recognition;//用于识别语音

namespace UnitTest
{


    public class SpeechTest
    {
        public void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("请输入中英文：");
                string s = Console.ReadLine();
                SpeechSynthesizer synth = new SpeechSynthesizer();
                //选择不同的发音
                //synth.SelectVoice("Microsoft Anna");//美式发音，但只能读英文
                synth.SelectVoice("Microsoft Lili");//能读中英文
                synth.Speak(s);
            }
            //语音识别
            //SpeechRecognitionEngine sre = new SpeechRecognitionEngine();
        }
    }
}
