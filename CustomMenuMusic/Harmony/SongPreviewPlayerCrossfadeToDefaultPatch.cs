using CustomMenuMusic.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomMenuMusic.Harmony
{
    [HarmonyPatch(typeof(SongPreviewPlayer), "CrossfadeToDefault")]
    public class SongPreviewPlayerCrossfadeToDefaultPatch
    {
        ///// <summary>
        ///// デフォルトの処理全スキップの暴挙
        ///// </summary>
        ///// <returns></returns>
        public static bool Prefix(SongPreviewPlayer __instance, ref int ____activeChannel, ref AudioSource[] ____audioSources, ref bool ____transitionAfterDelay)
        {
            if (!PluginConfig.Instance.Loop) {
                ____transitionAfterDelay = false;
            }
            if (____audioSources.Length <= (uint)____activeChannel) {
                return false;
            }
            if (____audioSources[____activeChannel].clip.GetInstanceID() == CustomMenuMusic.MenuMusic.GetInstanceID()) {
                return false;
            }
            if (CustomMenuMusic.MenuMusic) {
                __instance.CrossfadeTo(CustomMenuMusic.MenuMusic, UnityEngine.Random.Range(0.1f, CustomMenuMusic.MenuMusic.length / 2), CustomMenuMusic.MenuMusic.length, PluginConfig.Instance.MenuMusicVolume);
            }
            else {
                __instance.FadeOut();
            }
            return false;
        }
    }
}
