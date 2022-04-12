using CustomMenuMusic.Configuration;
using HarmonyLib;

namespace CustomMenuMusic.Harmony
{
    [HarmonyPatch(typeof(CustomPreviewBeatmapLevel), nameof(CustomBeatmapLevel.previewDuration), MethodType.Getter)]
    public class CustomPreviewBeatmapLevelPreviewDurationPatch
    {
        public static void Postfix(CustomBeatmapLevel __instance, ref float __result)
        {
            if (!PluginConfig.Instance.OverrideSongPrevewLength) {
                return;
            }
            if (__instance.songDuration < (__result + __instance.previewStartTime)) {
                __result = 0;
                return;
            }
            if (0 <= PluginConfig.Instance.MaxSongPreviewLength && PluginConfig.Instance.MaxSongPreviewLength < __result) {
                __result = PluginConfig.Instance.MaxSongPreviewLength;
            }
        }
    }
}
