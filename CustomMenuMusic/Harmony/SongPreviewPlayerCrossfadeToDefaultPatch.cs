using CustomMenuMusic.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomMenuMusic.Harmony
{
    [HarmonyPatch(typeof(SongPreviewPlayer), nameof(SongPreviewPlayer.CrossfadeToDefault))]
    public class SongPreviewPlayerCrossfadeToDefaultPatch
    {
        ///// <summary>
        ///// デフォルトの処理全スキップの暴挙
        ///// </summary>
        ///// <returns></returns>
        public static bool Prefix(SongPreviewPlayer __instance, ref int ____activeChannel, ref bool ____transitionAfterDelay)
        {
            if (CustomMenuMusic.MenuMusic == null) {
                return true;
            }
            var controllersObj = __instance?.GetType().GetField("_audioSourceControllers", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            if (controllersObj == null) {
                return true;
            }
            var controllers = controllersObj as object[];
            if (controllers == null) {
                return true;
            }
            if (controllers.Length <= (uint)____activeChannel) {
                return false;
            }
            var souceController = controllers[____activeChannel].GetType().GetField("audioSource").GetValue(controllers[____activeChannel]);
            if ((souceController as AudioSource).clip.GetInstanceID() == CustomMenuMusic.MenuMusic.GetInstanceID()) {
                return false;
            }
            if (CustomMenuMusic.MenuMusic) {
                __instance.CrossfadeTo(CustomMenuMusic.MenuMusic, UnityEngine.Random.Range(0.1f, CustomMenuMusic.MenuMusic.length / 2), PluginConfig.Instance.Loop ? -1 : CustomMenuMusic.MenuMusic.length, true);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(SongPreviewPlayer), nameof(SongPreviewPlayer.CrossFadeToDefault))]
    public class SongPreviewPlayerCrossFadeToDefaultPatch
    {
        public static bool Prefix(SongPreviewPlayer __instance)
        {
            __instance.CrossfadeToDefault();
            return false;
        }
    }

    [HarmonyPatch(typeof(SongPreviewPlayer), nameof(SongPreviewPlayer.CrossfadeToNewDefault), new Type[] { typeof(AudioClip) })]
    public class SongPreviewPlayerCrossfadeToNewDefaultPatch
    {
        public static bool Prefix(ref AudioClip audioClip)
        {
            return false;
        }
    }
}
