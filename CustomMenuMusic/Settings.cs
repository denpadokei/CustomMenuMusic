using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomMenuMusic
{
    partial class Config : PersistentSingleton<Config>
    {
        internal static bool initialized;

        [UIAction("#apply")]
        private void Apply() => Config.instance.Save();

        [UIValue("volume-range")]
        public List<object> VolumeRange => Enumerable.Range(0, 21).Select(x => x * 0.05f).Cast<object>().ToList();

        [UIAction("percent-formatter")]
        public string OnFormatPercent(float obj) => $"{obj * 100}%";

        [UIValue("now-playing-location-range")]
        public List<object> NowPlayingLocationRange => Enumerable.Range(0, 3).Select(x => (int)x).Cast<object>().ToList();

        [UIAction("format-now-playing-location")]
        public string OnFormatNowPlayingLocation(int obj) => locationNames[obj];

        [UIValue("preset-colors")]
        public List<object> presetColors = Enumerable.Range(0, NowPlaying.colors.Count).Select(x => (int)x).Cast<object>().ToList();

        [UIAction("format-text-color")]
        public string OnFormatTextColor(int value) => $"<color=#{ColorUtility.ToHtmlStringRGB(NowPlaying.colors[(int)value].Item1)}>{NowPlaying.colors[(int)value].Item2}";

        internal void CreateSettingsUI()
        {
            if (initialized) return;
            initialized = true;

            Util.Logger.Log("Creating Settings UI", Util.Logger.LogLevel.Debug);

            BSMLSettings.instance.AddSettingsMenu("Custom Menu Music", "CustomMenuMusic.Resources.Settings.bsml", PersistentSingleton<Config>.instance);
        }

        static readonly List<String> locationNames = new List<string> { "Left Panel", "Center Panel", "Right Panel" };
    }
}
