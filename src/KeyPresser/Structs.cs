using System.Runtime.InteropServices;

namespace KeyPresser
{
    public static class Structs
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public long x;
            public long y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            uint length;
            uint flags;
            uint showCmd;
            POINT ptMinPosition;
            POINT ptMaxPosition;
            RECT rcNormalPosition;
            RECT rcDevice;
        }
    }
}
