using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.ViewControllers;
using BS_Utils.Utilities;
using CustomMenuMusic.Configuration;
using CustomMenuMusic.Util;
using SongCore;
using Zenject;

namespace CustomMenuMusic.Views
{
    [HotReload]
    public class CMMTabViewController : BSMLAutomaticViewController, IInitializable
    {
        // For this method of setting the ResourceName, this class must be the first class in the file.
        public string ResourceName => string.Join(".", GetType().Namespace, GetType().Name);
        public string CurrentSongPath { get; set; } = "";

        private string _songName;

        private SongListUtility songListUtility;

        [Inject]
        public void Constractor(SongListUtility util)
        {
            this.songListUtility = util;
        }

        [UIValue("song-name")]
        public string SongName
        {
            get => this._songName ?? "";
            set
            {
                this._songName = value;
                this.NotifyPropertyChanged();
            }
        }
        [UIAction("play-now-play")]
        void PlayClick()
        {
            if (PluginConfig.Instance.UseCustomMenuSongs) {
                return;
            }

            if (Loader.CustomLevels.TryGetValue(Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(this.CurrentSongPath)), out var song)) {
                HMMainThreadDispatcher.instance.Enqueue(this.songListUtility.ScrollToLevel(song.levelID, null, true));
            }
            else {
                Util.Logger.Log($"Notfind song : {this.CurrentSongPath}");
                Util.Logger.Log($"{Loader.CustomLevels.FirstOrDefault().Key}");
            }
        }

        public void Initialize()
        {
            GameplaySetup.instance.AddTab("Custom Menu Music", this.ResourceName, this);
        }
    }
}
