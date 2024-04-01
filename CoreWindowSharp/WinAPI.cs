using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static CoreWindowSharp.Structs;

namespace CoreWindowSharp
{
    public static class WinAPI
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowEx")]
        public static extern IntPtr CreateWindowEx(
               WindowStylesEx dwExStyle,
               UInt16 lpClassName,
               string lpWindowName,
               WindowStyles dwStyle,
               uint x,
               uint y,
               uint nWidth,
               uint nHeight,
               IntPtr hWndParent,
               IntPtr hMenu,
               IntPtr hInstance,
               IntPtr lpParam);

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        public static extern System.UInt16 RegisterClassW([System.Runtime.InteropServices.In] ref WNDCLASSEX lpWndClass);

        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIConName);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("gdi32.dll")]
        public static extern IntPtr GetStockObject(StockObjects fnObject);

        [DllImport("user32.dll")]
        public static extern UInt16 RegisterClassEx([In] ref WNDCLASSEX lpwcx);

        [DllImport("User32.dll")]
        public static extern bool EnableMouseInPointer(bool fEnable);
    }
}
