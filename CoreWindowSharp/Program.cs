using System.Runtime.InteropServices;
using static CoreWindowSharp.WinAPI;
using static CoreWindowSharp.Structs;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace CoreWindowSharp
{
    public class Program
    {
        private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        // Hey Microsoft, please don't move this function to a new ordinal
        [DllImport("Windows.UI.dll", SetLastError = true, ExactSpelling = true, EntryPoint = "#1500")]
        private static extern int PrivateCreateCoreWindow(WINDOW_TYPE WindowType, string title, int x, int y, int width, int height, int attributes, IntPtr owner, Guid guid, out IntPtr ppv);

        // Instead of ComPtr<T>, we can just spawn on main thread and retrieve using builtin method.
        private static ICoreWindow coreWindow;
        // ABI::Windows::UI::Core::ICore* not accessible. Win.UI.Core.ICoreWindow is fine with CoreDispatcher
        private static CoreDispatcher dispatcher;
        private static XamlPresenter xamlPresenter;


        public static void Main(string[] args)
        {
            nint hInstance = System.Diagnostics.Process.GetCurrentProcess().Handle;
            const string CLASS_NAME = "WinRTWindow";

            WNDCLASSEX wc = new WNDCLASSEX();
            wc.cbSize = Marshal.SizeOf(typeof(WNDCLASSEX));
            wc.style = (int)(ClassStyles.HorizontalRedraw | ClassStyles.VerticalRedraw);
            wc.lpfnWndProc = Marshal.GetFunctionPointerForDelegate((WndProc)((hWnd, message, wParam, lParam) =>
            {
                if (message == (uint)WM.CLOSE) Environment.Exit(0);
                return DefWindowProc(hWnd, message, wParam, lParam);
            }));
            wc.lpszClassName = CLASS_NAME;
            wc.lpszMenuName = null;
            wc.cbClsExtra = 0;
            wc.cbWndExtra = 0;
            wc.hInstance = hInstance;
            wc.hIcon = LoadIcon(IntPtr.Zero, new IntPtr(32512));
            wc.hCursor = LoadCursor(IntPtr.Zero, 32512);
            wc.hbrBackground = GetStockObject(StockObjects.WHITE_BRUSH);

            UInt16 registerClassResult = RegisterClassEx(ref wc);

            IntPtr hwnd = CreateWindowEx(
                        0,
                        registerClassResult,
                        "CsXamlPresenter",
                        WindowStyles.WS_OVERLAPPEDWINDOW,
                        0x80000000,
                        0x80000000,
                        0x80000000,
                        0x80000000,
                        IntPtr.Zero,
                        IntPtr.Zero,
                        hInstance,
                        IntPtr.Zero);

            if (coreWindow == null)
            {
                // Init coreWindow on the thread and retrieve it using builtin method instead
                _ = PrivateCreateCoreWindow(WINDOW_TYPE.NOT_IMMERSIVE, "XamlPresenter", 0, 0, 1, 1, 0, hwnd, typeof(ICoreWindow).GUID, out _);
                coreWindow = CoreWindow.GetForCurrentThread();
            }

            ShowWindow(hwnd, 1);

            xamlPresenter = XamlPresenter.CreateFromHwnd((int)hwnd);
            xamlPresenter.InitializePresenter();
            Grid rootGrid = new();

            Button button = new();
            button.Content = "Hello";
            rootGrid.Children.Add(button);

            ProgressRing progressRing = new();
            progressRing.IsActive = true;
            rootGrid.Children.Add(progressRing);

            TextBlock textBlock = new();
            textBlock.Text = "Hello from Cs/WinRT CoreWindow!";
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center; 
            rootGrid.Children.Add(textBlock);

            xamlPresenter.Content = rootGrid;

            EnableMouseInPointer(true);

            if (dispatcher == null)
            {
                CoreDispatcher dispatcher = coreWindow.Dispatcher;
                dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessUntilQuit);
            }

            return;
        }
    }
}