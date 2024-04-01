# WinRTCoreWindow
Hosting a raw CoreWindow on hWnd from C# or C++.
<br/>
## Projects

### WinRTCoreWindow.vcxproj
This is the project containing the C++ version of the code.

### CoreWindowSharp.csproj
This is the project containing the C# version of the code.

> [!NOTE]  
> There are minor differences between the two to conform to each languages' standards, but they both achieve the same end result.
<br/>

# How does it work?
### C++:
1. Load `Windows.UI.dll` and extract a function pointer out of it pointing to a private function (`CreateCoreWindow`).
2. Cast that into the `PrivateCreateCoreWindow` delegate
3. Init winrt (`winrt::init_apartment()`)
4. Create a standard hWnd window (`CreateWindowEx` and `WNDCLASS`)
5. Create a GUID referring to CoreWindow.
6. Call the func pointer delegate, passing in the handle of that hWnd window and the GUID.
7. Initialize the `XamlPresenter` (found in the internal version of `Windows.UI.Xaml.Hosting`)
8. Init winrt components and set the content of the `XamlPresenter`.
9. Init `ICoreDispatcher` with ptr to CoreWindow dispatcher
10. Process dispatcher messages.

### C#:
1. `DllImport` the `Windows.UI.dll` library at ordinal 1500.
2. Create a standard hWnd window (`CreateWindowEx` and `WNDCLASSEX`)
3. Call `CreateCoreWindow` on the main thread and discard its output. (It's still initialized on the thread, so we don't need the output directly.)
4. Set the `ICoreWindow` instance to `CoreWindow.GetForCurrentThread()`.
5. Initialize the `XamlPresenter` (found in the internal version of `Windows.UI.Xaml.Hosting`).
6. Init winrt components and set the content of the `XamlPresenter`.
7. Extract the `ICoreDispatcher` as a `Windows.UI.Core.CoreDispatcher`.
8. Command `CoreDispatcher` to start processing dispatcher messages.
<br/>

# Demo
![image](https://github.com/Pdawg-bytes/WinRTCoreWindow/assets/83825746/cab55c13-d3df-42dc-89ef-a2b375eea067)
