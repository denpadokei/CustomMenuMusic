using CustomMenuMusic.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomMenuMusic.Harmony
{
    [HarmonyPatch(typeof(SongPreviewPlayerPauseOnInputFocusLost), nameof(SongPreviewPlayerPauseOnInputFocusLost.HandleInputFocusCaptured))]
    public class SongPreviewPlayerPauseOnInputFocusLostHandleInputFocusCapturedPatch
    {
        public static bool Postfix()
        {
            return PluginConfig.Instance.GrobalActiveSound;
        }
    }
}
