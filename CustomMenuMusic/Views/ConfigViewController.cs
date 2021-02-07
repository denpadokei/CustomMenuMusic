using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomMenuMusic.Configuration;
using CustomMenuMusic.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace CustomMenuMusic.Views
{
    public class ConfigViewController : BSMLAutomaticViewController, IInitializable
    {
        [UIValue("use-custom-menu-songs")]
        public bool UseCustomMenuSongs
        {
            get => PluginConfig.Instance.UseCustomMenuSongs;
            set => PluginConfig.Instance.UseCustomMenuSongs = value;
        }

        [UIValue("loop")]
        public bool Loop
        {
            get => PluginConfig.Instance.Loop;
            set => PluginConfig.Instance.Loop = value;
        }

        [UIValue("volume")]
        public float MenuMusicVolume
        {
            get => PluginConfig.Instance.MenuMusicVolume;
            set => PluginConfig.Instance.MenuMusicVolume = value;
        }

        [UIValue("show-now-playing")]
        public bool ShowNowPlaying
        {
            get => PluginConfig.Instance.ShowNowPlaying;
            set => PluginConfig.Instance.ShowNowPlaying = value;
        }

        [UIValue("now-playing-location")]
        public int NowPlayingLocation
        {
            get => PluginConfig.Instance.NowPlayingLocation;
            set => PluginConfig.Instance.NowPlayingLocation = value;
        }

        [UIValue("now-playing-color")]
        public int NowPlayingColor
        {
            get => PluginConfig.Instance.NowPlayingColor;
            set => PluginConfig.Instance.NowPlayingColor = value;
        }

        [UIValue("use-custom-result-songs")]
        public bool CustomResultSound
        {
            get => PluginConfig.Instance.CustomResultSound;
            set => PluginConfig.Instance.CustomResultSound = value;
        }

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
            Util.Logger.Log("Creating Settings UI", Util.Logger.LogLevel.Debug);
            BSMLSettings.instance.AddSettingsMenu("Custom Menu Music", "CustomMenuMusic.Resources.Settings.bsml", this);
        }

        public void Initialize()
        {
            this.CreateSettingsUI();
        }

        static readonly List<String> locationNames = new List<string> { "Left Panel", "Center Panel", "Right Panel" };

        public enum Location
        {
            LeftPanel,
            CenterPanel,
            RightPanel
        }
    }
}
