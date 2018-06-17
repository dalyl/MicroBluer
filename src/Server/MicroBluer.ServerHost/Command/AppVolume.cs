using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MicroBluer.ServerHost.Command
{
    public class AppVolume 
    {
        public void SetValue(decimal value)
        {
            if (value > 1) return;
            SystemMultimediaController.CurrentValue = (int)(value * (SystemMultimediaController.MaxValue - SystemMultimediaController.MinValue));
        }

        public decimal GetValue()
        {
            var value = (decimal)SystemMultimediaController.CurrentValue / (SystemMultimediaController.MaxValue - SystemMultimediaController.MinValue);
            return decimal.Round(value, 2);
        }

        class SystemMultimediaController
        {
            /*
             * 弹出系统音量控制器
             * */
            public static void PopupController()
            {
                ProcessStartInfo Info = new ProcessStartInfo();
                Info.FileName = "Sndvol32";
                Process.Start(Info);
            }

            /*
             * 获得音量范围和获取/设置当前音量
             * */
            public static int MaxValue
            {
                get { return int.Parse(iMaxValue.ToString()); }
            }
            public static int MinValue
            {
                get { return int.Parse(iMinValue.ToString()); }
            }
            public static int CurrentValue
            {
                get
                {
                    GetVolume();
                    return iCurrentValue;
                }
                set
                {
                    SetValue(MaxValue, MinValue, value);
                }
            }


            #region Private Static Data Members
            private const UInt32 iMaxValue = 0xFFFF;
            private const UInt32 iMinValue = 0x0000;
            private static int iCurrentValue = 0;
            #endregion
            #region Private Static Method
            /*
             * 得到当前音量
             **/
            private static void GetVolume()
            {
                UInt32 d, v;
                d = 0;
                long i = waveOutGetVolume(d, out v);
                UInt32 vleft = v & 0xFFFF;
                UInt32 vright = (v & 0xFFFF0000) >> 16;
                UInt32 all = vleft | vright;
                UInt32 value = (all * UInt32.Parse((MaxValue - MinValue).ToString()) / ((UInt32)iMaxValue));
                iCurrentValue = int.Parse(value.ToString());
            }

            /*
             * 修改音量值
             * */
            private static void SetValue(int aMaxValue, int aMinValue, int aValue)
            {
                //先把trackbar的value值映射到0x0000～0xFFFF范围  
                UInt32 Value = (UInt32)((double)0xffff * (double)aValue / (double)(aMaxValue - aMinValue));
                //限制value的取值范围  
                if (Value < 0) Value = 0;
                if (Value > 0xffff) Value = 0xffff;
                UInt32 left = (UInt32)Value;//左声道音量  
                UInt32 right = (UInt32)Value;//右  
                waveOutSetVolume(0, left << 16 | right); //"<<"左移，“|”逻辑或运算  
            }


            #endregion
            /*
             * 在winmm.dll中   
             *第一个参数可以为0，表示首选设备   
             *第二个参数为音量:0xFFFF为最大，0x0000为最小，
             *其中高位（前两位）表示右声道音量，低位（后两位）表示左 声道音量 。
            */
            #region Windows Media API
            [DllImport("winmm.dll")]
            private static extern long waveOutSetVolume(UInt32 deviceID, UInt32 Volume);
            [DllImport("winmm.dll")]
            private static extern long waveOutGetVolume(UInt32 deviceID, out UInt32 Volume);
            #endregion
        }
    }

}
