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
    [HarmonyPatch(typeof(SongPreviewPlayer), nameof(SongPreviewPlayer.CrossfadeTo),
        new Type[] { typeof(AudioClip), typeof(float), typeof(float), typeof(bool) })]
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
        public static bool Prefix(ref AudioClip audioClip, ref float startTime, ref float duration, ref bool isDefault, ref AudioClip ____defaultAudioClip, SongPreviewPlayer __instance)
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
        public static void Postfix(SongPreviewPlayer __instance, ref int ____activeChannel, ref bool ____transitionAfterDelay)
        {
            if (CustomMenuMusic.MenuMusic == null) {
                return;
            }
            var controllersObj = __instance.GetType().GetField("_audioSourceControllers", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var controllers = controllersObj as object[];
            var source = controllers[____activeChannel].GetType().GetField("audioSource").GetValue(controllers[____activeChannel]);

            if (!PluginConfig.Instance.Loop && ((AudioSource)source).clip.GetInstanceID() == CustomMenuMusic.MenuMusic.GetInstanceID()) {
                ((AudioSource)source).loop = false;
                ____transitionAfterDelay = false;
            }
        }
    }
}
