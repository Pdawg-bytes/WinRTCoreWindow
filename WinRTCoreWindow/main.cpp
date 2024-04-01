#include "pch.h"
#include <Windows.h>
#include "wrl.h"
#include <Windows.UI.Core.CoreWindowFactory.h>
#include <winrt/Windows.UI.Xaml.Controls.h>
#include <winrt/Windows.UI.Xaml.Controls.Primitives.h>
#include <winrt/Windows.UI.Xaml.Hosting.h>
#include <winrt/Windows.Foundation.h>
#include <iostream>

using namespace Microsoft::WRL;
using namespace ABI::Windows::UI::Core;
using namespace winrt::Windows::UI::Xaml::Controls::Primitives;
using namespace winrt::Windows::UI::Xaml::Controls;
using namespace winrt::Windows::UI::Xaml::Hosting;
using namespace winrt::Windows::UI::Xaml;
using namespace winrt::Windows::Foundation;
using namespace winrt;


enum WINDOW_TYPE
{
    IMMERSIVE_BODY,
    IMMERSIVE_DOCK,
    IMMERSIVE_HOSTED,
    IMMERSIVE_TEST,
    IMMERSIVE_BODY_ACTIVE,
    IMMERSIVE_DOCK_ACTIVE,
    NOT_IMMERSIVE
};

LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

typedef HRESULT(WINAPI* PrivateCreateCoreWindow)(WINDOW_TYPE, const wchar_t* title, int x, int y, int width, int height, int attributes, HWND owner, const IID& guid, void** ppv);

HMODULE uiCore = LoadLibrary(L"Windows.UI.dll");
auto privateCreateCoreWindow = (PrivateCreateCoreWindow)GetProcAddress(uiCore, MAKEINTRESOURCEA(1500));

ComPtr<ICoreWindow> coreWindow = nullptr;

ICoreDispatcher* dispatcher = nullptr;

HWND g_window;


int main()
{
    winrt::init_apartment();
    const wchar_t CLASS_NAME[] = L"WinRTWindow";

    WNDCLASS wc = { };

    HWND window;

    wc.lpfnWndProc = WindowProc;
    wc.hInstance = GetModuleHandle(NULL);
    wc.lpszClassName = CLASS_NAME;

    RegisterClass(&wc);

    window = CreateWindowEx(
        0,
        CLASS_NAME,
        L"CppXamlPresenter",
        WS_OVERLAPPEDWINDOW,

        CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT,

        NULL,
        NULL,
        GetModuleHandle(NULL),
        NULL
    );
    g_window = window;

    if (coreWindow == nullptr)
    {
        GUID iid;
        IIDFromString(L"{79B9D5F2-879E-4B89-B798-79E47598030C}", &iid);
        privateCreateCoreWindow(NOT_IMMERSIVE, L"XamlPresenter", 0, 0, 1, 1, 0, window, iid, (void**)coreWindow.GetAddressOf());
    }

    ShowWindow(window, 1);

    XamlPresenter xamlPresenter = XamlPresenter::CreateFromHwnd((int)window);
    xamlPresenter.InitializePresenter();
    auto rootGrid = Grid();

    auto button = Button();
    button.Content(winrt::box_value(L"Hello"));

    IButtonBase buttonBase = button;
    buttonBase.Click([](auto&& sender, RoutedEventArgs const& e)
    {
        std::cout << "C++/WinRT Button clicked!" << std::endl;
        std::cout << "Yes, this is on purpose: " << GetProcAddress(uiCore, MAKEINTRESOURCEA(1500)) << std::endl;
    });

    rootGrid.Children().Append(button);

    auto progressRing = ProgressRing();
    progressRing.IsActive(true);
    rootGrid.Children().Append(progressRing);

    auto textBlock = TextBlock();
    textBlock.Text(L"Hello from C++/WinRT CoreWindow!");
    textBlock.HorizontalAlignment(HorizontalAlignment::Center);
    textBlock.VerticalAlignment(VerticalAlignment::Center);
    rootGrid.Children().Append(textBlock);

    xamlPresenter.Content(rootGrid);

    EnableMouseInPointer(true);


    if (dispatcher == nullptr)
    {
        coreWindow->get_Dispatcher(&dispatcher);

        dispatcher->ProcessEvents(CoreProcessEventsOption::CoreProcessEventsOption_ProcessUntilQuit);
    }

    return 0;
}

LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    return DefWindowProc(hwnd, uMsg, wParam, lParam);
}