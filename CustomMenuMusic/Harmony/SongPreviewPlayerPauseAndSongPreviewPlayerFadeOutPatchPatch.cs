using HarmonyLib;
using System;

namespace CustomMenuMusic.Harmony
{
    [HarmonyPatch(typeof(SongPreviewPlayer), nameof(SongPreviewPlayer.PauseCurrentChannel))]
    public class SongPreviewPlayerPausePatch
    {
        public static void Postfix() => CustomMenuMusic.IsPauseOrFadeOut = true;
    }

    [HarmonyPatch(typeof(SongPreviewPlayer), nameof(SongPreviewPlayer.FadeOut), new Type[] { typeof(float) })]
    public class SongPreviewPlayerFadeOutPatch
    {
        public static void Postfix() => CustomMenuMusic.IsPauseOrFadeOut = true;
    }
}
