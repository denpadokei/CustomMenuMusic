using HarmonyLib;

namespace CustomMenuMusic.Harmony
{
    [HarmonyPatch(typeof(SongPreviewPlayer), nameof(SongPreviewPlayer.UnPauseCurrentChannel))]
    public class SongPreviewPlayerUnPausePatch
    {
        public static void Postfix()
        {
            CustomMenuMusic.IsPauseOrFadeOut = true;
            CustomMenuMusic.IsMenuSongPlaying = false;
        }
    }
}
