using System;
using System.Runtime.InteropServices; /*KeyBoard*/

namespace DodgeGame.Classes
{

    /*This class has virtual press key methods to make the code cleaner*/

    public class SimulatePCControl
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void keybd_event(uint bVk, uint bScan, uint dwFlags, uint dwExtraInfo);

        public const int VK_SPACE = 0x20;   /*Space*/
        public const int VK_RETURN = 0x0D;  /*Enter*/
        public const int VK_KEY_P = 0x50;   /*P*/
        public const int VK_KEY_R = 0x52;   /*R*/

        public static void SpaceArrow()
        {
            keybd_event(VK_SPACE, 0, 0, 0);
        }
        public static void PArrow()
        {   /*Pause*/
            keybd_event(VK_KEY_P, 0, 0, 0);
        }
        public static void RArrow()
        {   /*Restart*/
            keybd_event(VK_KEY_R, 0, 0, 0);
        }
    }
}
