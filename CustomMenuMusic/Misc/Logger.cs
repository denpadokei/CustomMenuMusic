namespace CustomMenuMusic.Misc
{
    static class Logger
    {
        public static void Log(object data)
        {
            UnityEngine.Debug.Log($"[Custom Menu Music] {data}");
        }

        public static void Debug(object data)
        {
#if DEBUG
            UnityEngine.Debug.Log($"[Custom Menu Music // DEBUG] {data}");
#endif
        }
    }
}
