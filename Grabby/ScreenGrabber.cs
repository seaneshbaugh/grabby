using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Grabby
{
    public class ScreenGrabber
    {
        public ImageFormat format;

        public ScreenGrabber()
        {
            format = ImageFormat.Png;
        }

        public System.Drawing.Image CaptureWindow(IntPtr handle)
        {
            IntPtr hdcSrc = User32.GetWindowDC(handle);

            User32.RECT windowRect = new User32.RECT();

            User32.GetWindowRect(handle, ref windowRect);

            int width = windowRect.right - windowRect.left;

            int height = windowRect.bottom - windowRect.top;

            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);

            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);

            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);

            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);

            GDI32.SelectObject(hdcDest, hOld);

            GDI32.DeleteDC(hdcDest);

            User32.ReleaseDC(handle, hdcSrc);

            System.Drawing.Image image = System.Drawing.Image.FromHbitmap(hBitmap);

            GDI32.DeleteObject(hBitmap);

            return image;
        }

        public System.Drawing.Image CaptureScreen()
        {
            return CaptureWindow(User32.GetDesktopWindow());
        }

        public void CaptureWindowToFile(IntPtr handle, string filename)
        {
            System.Drawing.Image image = CaptureWindow(handle);

            image.Save(filename, format);
        }

        public void CaptureScreenToFile(string filename)
        {
            System.Drawing.Image image = CaptureScreen();

            image.Save(filename, format);
        }

        private class GDI32
        {
            public const int SRCCOPY = 0x00CC0020;

            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, int dwRop);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);

            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);

            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }

            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
        }
    }
}