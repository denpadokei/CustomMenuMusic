﻿using CustomMenuMusic.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CustomMenuMusic.Harmony
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// BeatmapLevel(
    /// bool , string , string , string , string ,
    /// string[] , string[] , float , float , float ,
    /// float , float , float , PlayerSensitivityFlag , IPreviewMediaData , IReadOnlyDictionary<ValueTuple<BeatmapCharacteristicSO, BeatmapDifficulty>, BeatmapBasicData> )
    /// </remarks>
    [HarmonyPatch(typeof(BeatmapLevel), MethodType.Constructor,
        new Type[] {
            typeof(bool), typeof(string), typeof(string), typeof(string), typeof(string),
            typeof(string[]), typeof(string[]), typeof(float), typeof(float), typeof(float),
            typeof(float), typeof(float), typeof(float), typeof(PlayerSensitivityFlag), typeof(IPreviewMediaData), typeof(IReadOnlyDictionary<ValueTuple<BeatmapCharacteristicSO, BeatmapDifficulty>, BeatmapBasicData>) })]
    public class CustomPreviewBeatmapLevelPreviewDurationPatch
    {
        public static void Prefix(float previewStartTime, float songDuration, ref float previewDuration)
        {
            if (!PluginConfig.Instance.OverrideSongPrevewLength) {
                return;
            }
            if (songDuration < (previewDuration + previewStartTime)) {
                previewDuration = 0;
                return;
            }
            if (0 <= PluginConfig.Instance.MaxSongPreviewLength && PluginConfig.Instance.MaxSongPreviewLength < previewDuration) {
                previewDuration = PluginConfig.Instance.MaxSongPreviewLength;
            }
        }
    }
}
