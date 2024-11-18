using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomMenuMusic.Configuration;
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
        [UIValue("global-active-sound")]
        public bool GrobalActiveSound
        {
            get => PluginConfig.Instance.GrobalActiveSound;
            set => PluginConfig.Instance.GrobalActiveSound = value;
        }
        [UIValue("enable-song-preview")]
        public bool EnableSongPreview
        {
            get => PluginConfig.Instance.EnableSongPreview;
            set => PluginConfig.Instance.EnableSongPreview = value;
        }
        [UIValue("override-length")]
        public bool OverrideSongPrevewLength
        {
            get => PluginConfig.Instance.OverrideSongPrevewLength;
            set => PluginConfig.Instance.OverrideSongPrevewLength = value;
        }
        [UIValue("max-song-preview-length")]
        public float MaxSongPreviewLength
        {
            get => PluginConfig.Instance.MaxSongPreviewLength;
            set => PluginConfig.Instance.MaxSongPreviewLength = value;
        }
        [UIValue("volume-range")]
        public List<object> VolumeRange => Enumerable.Range(0, 21).Select(x => x * 0.05f).Cast<object>().ToList();

        [UIAction("percent-formatter")]
        public string OnFormatPercent(float obj)
        {
            return $"{obj * 100}%";
        }

        [UIValue("now-playing-location-range")]
        public List<object> NowPlayingLocationRange => Enumerable.Range(0, 3).Select(x => x).Cast<object>().ToList();

        [UIAction("format-now-playing-location")]
        public string OnFormatNowPlayingLocation(int obj)
        {
            return locationNames[obj];
        }

        [UIValue("preset-colors")]
        public List<object> presetColors = Enumerable.Range(0, NowPlaying.colors.Count).Select(x => x).Cast<object>().ToList();

        [UIAction("format-text-color")]
        public string OnFormatTextColor(int value)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(NowPlaying.colors[value].Item1)}>{NowPlaying.colors[value].Item2}";
        }

        internal void CreateSettingsUI()
        {
            Logger.Log("Creating Settings UI", Logger.LogLevel.Debug);
            BSMLSettings.Instance.AddSettingsMenu("Custom Menu Music", "CustomMenuMusic.Views.ConfigViewController.bsml", this);
        }

        public void Initialize()
        {
            this.CreateSettingsUI();
        }

        private static readonly List<string> locationNames = new List<string> { "Left Panel", "Center Panel", "Right Panel" };

        public enum Location
        {
            LeftPanel,
            CenterPanel,
            RightPanel
        }
    }
}
