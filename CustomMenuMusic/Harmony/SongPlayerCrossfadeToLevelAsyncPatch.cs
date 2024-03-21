using CustomMenuMusic.Configuration;
using HarmonyLib;
using System;
using System.Threading;

namespace CustomMenuMusic.Harmony
{
    [HarmonyPatch(typeof(LevelCollectionViewController), nameof(LevelCollectionViewController.SongPlayerCrossfadeToLevelAsync), new Type[] { typeof(BeatmapLevel), typeof(CancellationToken) })]
    public class SongPlayerCrossfadeToLevelAsyncPatch
    {
        public static bool Prefix()
        {
            return PluginConfig.Instance.EnableSongPreview;
        }
    }
}
