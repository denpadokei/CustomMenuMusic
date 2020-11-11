using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using BS_Utils.Utilities;
using CustomMenuMusic.Util;
using SongCore;

namespace CustomMenuMusic.Views
{
    [HotReload]
    public class TabViewController : PersistentSingleton<TabViewController>, INotifyPropertyChanged
    {
        // For this method of setting the ResourceName, this class must be the first class in the file.
        public string ResourceName => string.Join(".", GetType().Namespace, GetType().Name);

        
        private LevelCollectionViewController _levelCollectionViewController;
        
        private SelectLevelCategoryViewController _selectLevelCategoryViewController;
        
        private GameplaySetupViewController _gameplaySetupViewController;
        
        private LevelFilteringNavigationController _levelFilteringNavigationController;
        
        private AnnotatedBeatmapLevelCollectionsViewController _annotatedBeatmapLevelCollectionsViewController;

        public string CurrentSongPath { get; set; } = "";

        private string _songName;
        [UIValue("song-name")]
        public string SongName
        {
            get => this._songName ?? "";
            set
            {
                this._songName = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SongName)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [UIAction("play-now-play")]
        void PlayClick()
        {
            if (Config.instance.UseCustomMenuSongs) {
                return;
            }

            if (Loader.CustomLevels.TryGetValue(Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(this.CurrentSongPath)), out var song)) {
                HMMainThreadDispatcher.instance.Enqueue(SongListUtility.ScrollToLevel(song.levelID, null, true));
            }
            else {
                Util.Logger.Log($"Notfind song : {this.CurrentSongPath}");
                Util.Logger.Log($"{Loader.CustomLevels.FirstOrDefault().Key}");
            }
        }
    }
}
