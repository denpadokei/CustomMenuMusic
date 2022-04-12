using CustomMenuMusic.Configuration;
using HarmonyLib;

namespace CustomMenuMusic.Harmony
{
    [HarmonyPatch(typeof(SongPreviewPlayerPauseOnInputFocusLost), nameof(SongPreviewPlayerPauseOnInputFocusLost.HandleInputFocusCaptured))]
    public class SongPreviewPlayerPauseOnInputFocusLostHandleInputFocusCapturedPatch
    {
        public static bool Prefix()
        {
            return !PluginConfig.Instance.GrobalActiveSound;
        }
    }
}
