using System;
using System.Threading;
using AutoIt;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.Utility
{
    public class AutoItTool
    {
        public double WidthRatio=1;
        public double HeightRatio=1;

        //public AutoItTool()
        //{
        //    //ClUtility.GetScreenResolution(ref HeightRatio, ref WidthRatio);
        //    ////For Temporary purpose setting value 1.00
        //    //WidthRatio = 1.00;
        //    //HeightRatio = 1.00;
        //}

        public void WinActivate(string name, double delayBefore = 0, double delayAfter = 0)
        {
            var win = AutoItX.WinActive(name);
            if (win == 0)
            {
                Thread.Sleep(TimeSpan.FromSeconds(delayBefore));
                AutoItX.WinActivate(name);
                Thread.Sleep(TimeSpan.FromSeconds(delayAfter));
            }
        }

        public void WinActivate(IntPtr intPtr, double delayBefore = 0, double delayAfter = 0)
        {
            var win = AutoItX.WinActive(intPtr);
            if (win == 0)
            {
                Thread.Sleep(TimeSpan.FromSeconds(delayBefore));
                AutoItX.WinActivate(intPtr);
                Thread.Sleep(TimeSpan.FromSeconds(delayAfter));
            }
        }

        public void KillWindow(IntPtr intPtr)
           => AutoItX.WinKill(intPtr);

        public IntPtr GetIntPtr(string name, string text = "") => AutoItX.WinGetHandle(name, text);

        public void MoveWindow(string name, int x = 0, int y = 0, double delayBefore = 0, double delayAfter = 0)
        {
            Thread.Sleep(TimeSpan.FromSeconds(delayBefore));
            AutoItX.WinMove(name, "", x, y);
            Thread.Sleep(TimeSpan.FromSeconds(delayAfter));
        }
        public void MoveWindow(IntPtr processIntPtr, int x = 0, int y = 0, double delayBefore = 0, double delayAfter = 0)
        {
            Thread.Sleep(TimeSpan.FromSeconds(delayBefore));
            AutoItX.WinMove(processIntPtr, x, y);
            Thread.Sleep(TimeSpan.FromSeconds(delayAfter));
        }

        public void MinimizeAll() => AutoItX.WinMinimizeAll();

        public int GetExactHeight(int height)
           => Convert.ToInt32(height * HeightRatio);

        public int GetExactWidth(int width)
           => Convert.ToInt32(width * WidthRatio);

        public string GetLastCopied() => AutoItX.ClipGet();
        public void CopyToClip(string text) => AutoItX.ClipPut(text);
        public void MoveMouse(int x = 0, int y = 0, int movingSpeed = -1, double delayBefore = 0, double delayAfter = 0)
        {
            Thread.Sleep(TimeSpan.FromSeconds(delayBefore));
            AutoItX.MouseMove(GetExactWidth(x), GetExactHeight(y), movingSpeed);
            Thread.Sleep(TimeSpan.FromSeconds(delayAfter));
        }

        public void MouseClick(MouseKeys key = MouseKeys.Left, int count = 1, double delayBetween = 0.5, double delayBefore = 0, double delayAfter = 0)
        {
            Thread.Sleep(TimeSpan.FromSeconds(delayBefore));
            var iteration = 0;
            while (iteration < count)
            {
                Thread.Sleep(TimeSpan.FromSeconds(delayBetween));
                AutoItX.MouseClick($"{key.ToString().ToUpper()}");
                iteration++;
            }
            Thread.Sleep(TimeSpan.FromSeconds(delayAfter));
        }

        public void PressAnyKey(KeyboardKeys key, int count = 1, double delayBetween = 0.5, double delayBefore = 0, double delayAfter = 0)
        {
            Thread.Sleep(TimeSpan.FromSeconds(delayBefore));
            var iteration = 0;
            while (iteration < count)
            {
                Thread.Sleep(TimeSpan.FromSeconds(delayBetween));
                AutoItX.Send("{" + key.ToString().ToUpper() + "}");
                iteration++;
            }
            Thread.Sleep(TimeSpan.FromSeconds(delayAfter));
        }

        public void TypeString(string parameter, double delayBefore = 0, double delayAfter = 0)
        {
            Thread.Sleep(TimeSpan.FromSeconds(delayBefore));
            AutoItX.Send(parameter);
            Thread.Sleep(TimeSpan.FromSeconds(delayAfter));
        }

        public void PressCtrlPlus(KeyboardKeys key, double delayBefore = 0, double delayAfter = 0)
        {
            Thread.Sleep(TimeSpan.FromSeconds(delayBefore));
            AutoItX.MouseClick();
            AutoItX.Send($"^{key.ToString().ToLower()}");
            Thread.Sleep(TimeSpan.FromSeconds(delayAfter));
        }

        public void CopyBySelectAll()
        {
            PressCtrlPlus(KeyboardKeys.A);
            PressCtrlPlus(KeyboardKeys.C, delayAfter: 0.5);
        }

        public void CloseBrowserActiveTab()
        {
            Thread.Sleep(1000);
            AutoItX.MouseClick();
            AutoItX.Send("^w");
        }
    }
}
