using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Grabby
{
    public class KeyInterceptor
    {
        public const int WH_KEYBOARD_LL = 13;
        public const int WM_KEYDOWN = 0x0100;

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private LowLevelKeyboardProc hookCallback;
        private IntPtr hookID;

        public KeyInterceptor()
        {
            hookID = IntPtr.Zero;
        }

        public IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            RemoveHook();

            hookCallback = proc;

            using (Process currentProcess = Process.GetCurrentProcess())
            using (ProcessModule currentModule = currentProcess.MainModule)
            {
                hookID = SetWindowsHookEx(WH_KEYBOARD_LL, ProcessHookCallback, GetModuleHandle(currentModule.ModuleName), 0);

                return hookID;
            }
        }

        public bool RemoveHook()
        {
            if (hookID != IntPtr.Zero)
            {
                return UnhookWindowsHookEx(hookID);
            }

            return false;
        }

        private IntPtr ProcessHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            hookCallback(nCode, wParam, lParam);

            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}