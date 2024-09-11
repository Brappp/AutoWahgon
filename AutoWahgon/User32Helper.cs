using System;
using System.Runtime.InteropServices;

public static class User32Helper
{
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int VK_CONTROL = 0x11; // Virtual key code for Ctrl
    private const int VK_NUMPAD6 = 0x66; // Virtual key code for Numpad6
    private const int VK_NUMPAD0 = 0x60; // Virtual key code for Numpad0

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

    // Retrieve FFXIV window handle
    public static IntPtr GetFfxivWindowHandle()
    {
        return FindWindow(null, "FINAL FANTASY XIV");
    }

    // Send key event to FFXIV
    public static void SendKeyToFfxiv(int vkCode)
    {
        IntPtr ffxivHwnd = GetFfxivWindowHandle();
        if (ffxivHwnd == IntPtr.Zero)
        {
            Console.WriteLine("FFXIV window not found.");
            return;
        }

        // Send WM_KEYDOWN
        PostMessage(ffxivHwnd, WM_KEYDOWN, vkCode, 0);

        // Send WM_KEYUP
        PostMessage(ffxivHwnd, WM_KEYUP, vkCode, 0);
    }

    public static void SendCtrlToFfxiv()
    {
        SendKeyToFfxiv(VK_CONTROL);
    }

    public static void SendNumpad6ToFfxiv()
    {
        SendKeyToFfxiv(VK_NUMPAD6);
    }

    public static void SendNumpad0ToFfxiv()
    {
        SendKeyToFfxiv(VK_NUMPAD0);
    }
}
