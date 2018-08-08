using System;
using System.Speech.Synthesis; //用于语音朗读
using System.Speech.Recognition;//用于识别语音
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{


    [TestClass]
    public class SpeechTest
    {
        [TestMethod]
        public void Main()
        {
            string s = "hello world";
            SpeechSynthesizer synth = new SpeechSynthesizer();
            //选择不同的发音
            //synth.SelectVoice("Microsoft Anna");//美式发音，但只能读英文
            synth.SelectVoice("Microsoft Lili");//能读中英文
            synth.Speak(s);
            //语音识别
            //SpeechRecognitionEngine sre = new SpeechRecognitionEngine();
        }
    }
}
