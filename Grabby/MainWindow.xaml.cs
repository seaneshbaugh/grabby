using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;

namespace Grabby
{
    public partial class MainWindow : Window
    {
        private KeyInterceptor keyInterceptor;

        private ScreenGrabber screenGrabber;

        public MainWindow()
        {
            InitializeComponent();

            keyInterceptor = new KeyInterceptor();

            screenGrabber = new ScreenGrabber();

            keyInterceptor.SetHook(InterceptPrintScreen);
        }

        private void SelectFolder(object sender, RoutedEventArgs e)
        {
            BrowseForFolder folderBrowser = new BrowseForFolder();

            String path = folderBrowser.SelectFolder("", "C:\\", Process.GetCurrentProcess().MainWindowHandle);

            outputPath.Text = path;
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            keyInterceptor.RemoveHook();
        }

        private IntPtr InterceptPrintScreen(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)KeyInterceptor.WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (vkCode == 0x2C)
                {
                    screenGrabber.CaptureScreenToFile("C:\\Users\\Sean\\Documents\\test.png");
                }
            }

            return IntPtr.Zero;
        }
    }
}