using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
