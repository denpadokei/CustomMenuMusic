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
    [HarmonyPatch(typeof(SongPreviewPlayer), "CrossfadeTo",
        new Type[] { typeof(AudioClip), typeof(float), typeof(float), typeof(float) })]
    public class SongPreviewPlayerCrossfadeToPatch
    {
        /// <summary>
        /// デフォルト曲が飛んで来たら差し替え。それ以外ならスルー
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="startTime"></param>
        /// <param name="duration"></param>
        /// <param name="volumeScale"></param>
        /// <param name="____defaultAudioClip"></param>
        /// <param name="__instance"></param>
        /// <returns></returns>
        public static bool Prefix(ref AudioClip audioClip, ref float startTime, ref float duration, ref float volumeScale, ref AudioClip ____defaultAudioClip, SongPreviewPlayer __instance)
        {
            if (audioClip.GetInstanceID() == ____defaultAudioClip.GetInstanceID()) {
                if (CustomMenuMusic.MenuMusic) {
                    audioClip = CustomMenuMusic.MenuMusic;
                }
                else {
                    return true;
                }
            }
            if (!PluginConfig.Instance.Loop && duration < 0f) {
                duration = audioClip.length;
            }
            return true;
        }

        /// <summary>
        /// ループが取れません！
        /// </summary>
        /// <param name="____activeChannel"></param>
        /// <param name="____audioSources"></param>
        public static void Postfix(ref int ____activeChannel, ref AudioSource[] ____audioSources, ref bool ____transitionAfterDelay)
        {
            if (!PluginConfig.Instance.Loop && ____audioSources[____activeChannel].clip.GetInstanceID() == CustomMenuMusic.MenuMusic.GetInstanceID()) {
                ____audioSources[____activeChannel].loop = false;
                ____transitionAfterDelay = false;
            }
        }
    }
}
