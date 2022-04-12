using CustomMenuMusic.Configuration;
using HarmonyLib;
using System;

namespace CustomMenuMusic.Harmony
{
    [HarmonyPatch(typeof(LevelCollectionViewController), nameof(LevelCollectionViewController.SongPlayerCrossfadeToLevelAsync), new Type[] { typeof(IPreviewBeatmapLevel) })]
    public class SongPlayerCrossfadeToLevelAsyncPatch
    {
        public static bool Prefix()
        {
            return PluginConfig.Instance.EnableSongPreview;
        }
    }
}
