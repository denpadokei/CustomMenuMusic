using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomMenuMusic.Configuration;
using CustomMenuMusic.Util;
using SongCore;
using System;
using System.IO;
using System.Linq;
using Zenject;

namespace CustomMenuMusic.Views
{
    [HotReload]
    public class CMMTabViewController : BSMLAutomaticViewController, IInitializable
    {
        // For this method of setting the ResourceName, this class must be the first class in the file.
        public string ResourceName => string.Join(".", this.GetType().Namespace, this.GetType().Name);
        public string CurrentSongPath { get; set; } = "";

        private string _songName;

        private SongListUtility songListUtility;
        private CustomMenuMusic _customMenuMusic;
        private SongPreviewPlayer _songPreviewPlayer;

        [Inject]
        public void Constractor(SongListUtility util, CustomMenuMusic customMenuMusic, SongPreviewPlayer player)
        {
            this.songListUtility = util;
            this._customMenuMusic = customMenuMusic;
            this._songPreviewPlayer = player;
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
        private void PlayClick()
        {
            if (PluginConfig.Instance.UseCustomMenuSongs) {
                return;
            }

            if (Loader.CustomLevels.TryGetValue(Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(this.CurrentSongPath)), out var song)) {
                MainThreadInvoker.Instance.Enqueue(this.songListUtility.ScrollToLevel(song.levelID, null));
            }
            else {
                Logger.Log($"Notfind song : {this.CurrentSongPath}");
                Logger.Log($"{Loader.CustomLevels.FirstOrDefault().Key}");
            }
        }
        [UIAction("skip")]
        private void Skip()
        {
            this._songPreviewPlayer.FadeOut(0);
            this._customMenuMusic.Next();
        }

        public void Initialize()
        {
            Logger.Log("Create tab.");
            GameplaySetup.instance?.RemoveTab("Custom Menu Music");
            GameplaySetup.instance?.AddTab("Custom Menu Music", this.ResourceName, this);
        }
        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            this.CurrentSongPath = this._customMenuMusic.CurrentSongPath;
        }
    }
}
