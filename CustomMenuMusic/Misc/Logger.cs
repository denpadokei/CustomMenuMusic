using System;

namespace CustomMenuMusic.Misc
{
    static class Logger
    {
        public static void Log(object data)
        {
            Console.WriteLine($"[Custom Menu Music] {data}");
        }

        public static void Debug(object data)
        {
#if DEBUG
            Console.WriteLine($"[Custom Menu Music // DEBUG] {data}");
#endif
        }
    }
}
