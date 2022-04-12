using CustomMenuMusic.Configuration;
using HarmonyLib;
using System;
using UnityEngine;

namespace CustomMenuMusic.Harmony
{
    [HarmonyPatch(typeof(SongPreviewPlayer), nameof(SongPreviewPlayer.CrossfadeTo),
        new Type[] { typeof(AudioClip), typeof(float), typeof(float), typeof(float), typeof(bool), typeof(Action) })]
    public class SongPreviewPlayerCrossfadeToPatch
    {
        /// <summary>
        /// デフォルト曲が飛んで来たら差し替え。それ以外ならスルー
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="startTime"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static void Prefix(ref AudioClip audioClip, ref float musicVolume, ref float startTime, ref float duration, ref bool isDefault, out bool __state)
        {
            if (isDefault && CustomMenuMusic.MenuMusic) {
                audioClip = CustomMenuMusic.MenuMusic;
                CustomMenuMusic.IsMenuSongPlaying = true;
            }
            else {
                CustomMenuMusic.IsMenuSongPlaying = false;
            }
            var max = audioClip.length - duration;
            if (startTime == -1) {
                startTime = UnityEngine.Random.Range(0f, max < 0 ? 0 : audioClip.length);
            }
            __state = isDefault;
        }

        /// <summary>
        /// ループが取れません！
        /// </summary>
        /// <param name="____activeChannel"></param>
        public static void Postfix(ref int ____activeChannel, ref bool ____transitionAfterDelay, ref SongPreviewPlayer.AudioSourceVolumeController[] ____audioSourceControllers, bool __state)
        {
            CustomMenuMusic.IsPauseOrFadeOut = false;
            if (!__state || !CustomMenuMusic.MenuMusic) {
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

    [HarmonyPatch(typeof(SongPreviewPlayer), nameof(SongPreviewPlayer.CrossfadeToDefault))]
    public class SongPreviewPlayerCrossfadeToDefaultPatch
    {
        public static bool Prefix()
        {
            return !CustomMenuMusic.IsMenuSongPlaying;
        }
    }
}
