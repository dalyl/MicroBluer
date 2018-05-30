using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CSCore.CoreAudioAPI;
using LazyWelfare.ServerCore.CommandInterface;

namespace LazyWelfare.ServerHost.Command
{

    public class WindowsVolume : IVolumeController
    {
        public void SetValue(decimal value)
        {
            if (value > 1) return;
            SetVolume(value);
        }

        public decimal GetValue()
        {
            decimal value = GetVolume();
            return decimal.Round(value, 2);
        }

        public void Execute(string command, object[] args)
        {
            if (args == null || args.Length == 0) return;
            var value = Convert.ToDecimal(args[0]);
            SetValue(value);
        }

        public object ExecuteResult(string command, object[] args)
        {
            return GetValue();
        }

        #region CoreAudioAPI


        private static void SetVolume(decimal value) {
            using (var volume = GetDefaultAudioEndpointVolume())
            {
                volume.MasterVolumeLevelScalar = (float)value;
            }
          
        }

        private static decimal GetVolume()
        {
            using (var volume = GetDefaultAudioEndpointVolume())
            {
                var db= volume.MasterVolumeLevelScalar;
                return new decimal(db);
            }

        }

        static double dbToPercent(double value)
        {
            return Math.Pow(10.0f, (value / 20.0f)) * 100.0f;
          //  return powf(10.0f, (value / 20.0f)) * 100.0f;
        }

        private static AudioEndpointVolume GetDefaultAudioEndpointVolume()
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                using (var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia))
                {
                    return AudioEndpointVolume.FromDevice(device);
                }
            }
        }

        #endregion

        #region SendMessageW

        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0x0a0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x090000;
        private const int WM_APPCOMMAND = 0x319;

        [DllImport("user32.dll")]
        static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        void SetVol(int count)
        {
            var Handle= Process.GetCurrentProcess().Handle;
            for (int i = 0; i < count; i++)
            {
                SendMessageW(Handle, WM_APPCOMMAND, Handle, (IntPtr)APPCOMMAND_VOLUME_UP);
            }
        }

        #endregion

        #region keybd_event


        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, UInt32 dwFlags, UInt32 dwExtraInfo);

        [DllImport("user32.dll")]
        static extern Byte MapVirtualKey(UInt32 uCode, UInt32 uMapType);

        private const byte VK_VOLUME_MUTE = 0xAD;
        private const byte VK_VOLUME_DOWN = 0xAE;
        private const byte VK_VOLUME_UP = 0xAF;
        private const UInt32 KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const UInt32 KEYEVENTF_KEYUP = 0x0002;

        /// <summary>
        /// 改变系统音量大小，增加
        /// </summary>
        public void VolumeUp()
        {
            keybd_event(VK_VOLUME_UP, MapVirtualKey(VK_VOLUME_UP, 0), KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_VOLUME_UP, MapVirtualKey(VK_VOLUME_UP, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }

        /// <summary>
        /// 改变系统音量大小，减小
        /// </summary>
        public void VolumeDown()
        {
            keybd_event(VK_VOLUME_DOWN, MapVirtualKey(VK_VOLUME_DOWN, 0), KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_VOLUME_DOWN, MapVirtualKey(VK_VOLUME_DOWN, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }

        /// <summary>
        /// 改变系统音量大小，静音
        /// </summary>
        public void Mute()
        {
            keybd_event(VK_VOLUME_MUTE, MapVirtualKey(VK_VOLUME_MUTE, 0), KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_VOLUME_MUTE, MapVirtualKey(VK_VOLUME_MUTE, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }

        #endregion

    }


}
