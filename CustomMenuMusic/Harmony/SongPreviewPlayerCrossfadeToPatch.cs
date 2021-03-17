using CustomMenuMusic.Configuration;
using HarmonyLib;
using System;
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
        /// <returns></returns>
        public static void Prefix(ref AudioClip audioClip, ref float startTime, ref float duration, ref bool isDefault, out bool __isDefault)
        {
            if (isDefault && CustomMenuMusic.MenuMusic) {
                audioClip = CustomMenuMusic.MenuMusic;
            }
            __isDefault = isDefault;
        }

        /// <summary>
        /// ループが取れません！
        /// </summary>
        /// <param name="____activeChannel"></param>
        public static void Postfix(ref int ____activeChannel, ref bool ____transitionAfterDelay, ref SongPreviewPlayer.AudioSourceVolumeController[] ____audioSourceControllers, bool __isDefault)
        {
            CustomMenuMusic.IsPause = false;
            if (!__isDefault || !CustomMenuMusic.MenuMusic) {
                return;
            }
            var source = ____audioSourceControllers[____activeChannel]?.audioSource;
            if (!source) {
                return;
            }
            if (!PluginConfig.Instance.Loop) {
                source.loop = false;
                ____transitionAfterDelay = false;
            }
        }
    }
}
