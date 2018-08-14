using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace MicroBluer.ServerHost.Features
{

    public interface IVoiceReader
    {
        decimal Progress { get; }

        bool State { get; }
        List<string> Voicers { get; }
        void Speak(string voicer, double volume, double rate, string content);
        void Generated(string voicer, double volume, double rate, string content, string path);
    }

    public class MicrosoftVoiceReader : IVoiceReader
    {
        SpeechSynthesizer Synth { get; } = new SpeechSynthesizer();

        public List<string> Voicers { get; } = new List<string>();

        public decimal Progress { get; private set; } = 0;

        public bool State { get; private set; } = false;

        public MicrosoftVoiceReader()
        {
            foreach (InstalledVoice iv in Synth.GetInstalledVoices())
            {
                Voicers.Add(iv.VoiceInfo.Name);
            }


            //选择不同的发音
            //synth.SelectVoice("Microsoft Anna");//美式发音，但只能读英文
            //synth.SelectVoice("Microsoft Lili");//能读中英文
            //语音识别
            //SpeechRecognitionEngine sre = new SpeechRecognitionEngine();
        }

        private object lockobj = 1;
        private void BeginWork(Action job)
        {
            if (State) return;
            lock (lockobj)
            {
                if (State) return;
                State = true;
                job();
                State = false;
            }
        }

        void Check(ref double volume,ref double rate)
        {
            if (volume < 1) volume = 1;
            if (volume > 100) volume = 99;
            if (rate < -10) rate = -9;
            if (rate > 10) rate = 9;
        }


        public void Speak(string voicer, double volume, double rate, string content)
        {
            if (Voicers.Contains(voicer) == false) return;
            Check(ref volume,ref  rate);
            BeginWork(() =>
            {
                Synth.SelectVoice(voicer);
                Synth.Volume = (int)volume;
                Synth.Rate =(int) rate;
                Synth.Speak(content);
            });
        }


        public void Generated(string voicer, double volume, double rate, string content, string file)
        {
            if (Voicers.Contains(voicer) == false) return;
            Check(ref volume, ref rate);
            BeginWork(() =>
            {
                var path = new System.IO.FileInfo(file);
                if (path.Directory.Exists == false) System.IO.Directory.CreateDirectory(path.Directory.FullName);
                Synth.SelectVoice(voicer);
                Synth.Volume = (int)volume;
                Synth.Rate = (int)rate;
                Synth.SetOutputToWaveFile(file);
                Synth.Speak(content);
                Synth.SetOutputToDefaultAudioDevice();
            });
        }

        public void Dispose()
        {
            Synth.Dispose();
        }
    }

    public class DefaultVoiceReader : IVoiceReader
    {
        public decimal Progress => 0;

        public bool State => false;

        public List<string> Voicers { get; } = new List<string>();

        public void Generated(string voicer, double volume, double rate, string content, string path)
        {

        }

        public void Speak(string voicer, double volume, double rate, string content)
        {

        }
    }

    public static class VoiceReader
    {
        public enum Reader
        {
            Default,
            Microsoft
        }

        static IVoiceReader CreateReader(Reader speaker)
        {
            switch (speaker)
            {
                case Reader.Microsoft: return new MicrosoftVoiceReader();
                default: return new DefaultVoiceReader();
            }
        }

        public static IVoiceReader Speak(Reader speaker, string voicer, string content, double volume, double rate)
        {
            var reader = CreateReader(speaker);
            Task.Factory.StartNew(() => reader.Speak(voicer, volume, rate, content));
            return reader;
        }

        public static IVoiceReader Generated(Reader speaker, string voicer, string content, string path, double volume, double rate)
        {
            var reader = CreateReader(speaker);
            Task.Factory.StartNew(() => reader.Generated(voicer, volume, rate, content, path));
            return reader;
        }
    }


}
