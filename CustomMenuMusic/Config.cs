using BeatSaberMarkupLanguage.Attributes;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace CustomMenuMusic
{
    partial class Config : PersistentSingleton<Config>
    {
        BS_Utils.Utilities.Config config = new BS_Utils.Utilities.Config(configName);

        static readonly string configName = "CustomMenuMusic";
        static readonly string sectionCore = "Core";
        static readonly string sectionNowPlaying = "NowPlaying";


        readonly string useCustomMenuSongs = "UseCustomMenuSongs";
        readonly string loop = "Loop";
        readonly string menuMusicVolume = "MenuMusicVolume";
        readonly string showNowPlaying = "ShowNowPlaying";
        readonly string nowPlayingLocation = "NowPlayingLocation";
        readonly string nowPlayingColor = "NowPlayingColor";

        Config()
        {
            Load();
        }

        internal void Load()
        {
            Util.Logger.Log("Loading config!");

            UseCustomMenuSongs = config.GetBool(sectionCore, useCustomMenuSongs, false, true);
            Loop = config.GetBool(sectionCore, loop, false, true);
            MenuMusicVolume = config.GetFloat(sectionCore, menuMusicVolume, 0.5f, true);
            ShowNowPlaying = config.GetBool(sectionNowPlaying, showNowPlaying, true, true);
            NowPlayingLocation = config.GetInt(sectionNowPlaying, nowPlayingLocation, 0, true);
            NowPlayingColor = config.GetInt(sectionNowPlaying, nowPlayingColor, 0, true);
        }

        internal void Save()
        {
            Util.Logger.Log("Saving config!");
            config.SetBool(sectionCore, useCustomMenuSongs, UseCustomMenuSongs);
            config.SetBool(sectionCore, loop, Loop);
            config.SetFloat(sectionCore, menuMusicVolume, MenuMusicVolume);
            config.SetBool(sectionNowPlaying, showNowPlaying, ShowNowPlaying);
            config.SetInt(sectionNowPlaying, nowPlayingLocation, NowPlayingLocation);
            config.SetInt(sectionNowPlaying, nowPlayingColor, NowPlayingColor);

            CustomMenuMusic.instance.GetSongsList();
            StartCoroutine(SetVolume());
            NowPlaying.instance?.SetCurrentSong(ShowNowPlaying ? CustomMenuMusic.instance?.CurrentSongPath : String.Empty);
            NowPlaying.instance?.SetTextColor(NowPlayingColor);
            NowPlaying.instance?.SetLocation((Location)NowPlayingLocation);
        }

        IEnumerator SetVolume()
        {
            SongPreviewPlayer _previewPlayer = null;
            yield return new WaitUntil(() => _previewPlayer = Resources.FindObjectsOfTypeAll<SongPreviewPlayer>().FirstOrDefault());
            _previewPlayer.volume = MenuMusicVolume;
        }


        [UIValue("use-custom-menu-songs")]
        public bool UseCustomMenuSongs;

        [UIValue("loop")]
        public bool Loop;

        [UIValue("volume")]
        public float MenuMusicVolume;

        [UIValue("show-now-playing")]
        public bool ShowNowPlaying;

        [UIValue("now-playing-location")]
        public int NowPlayingLocation;

        [UIValue("now-playing-color")]
        public int NowPlayingColor;

        internal enum Location
        {
            LeftPanel,
            CenterPanel,
            RightPanel
        }
    }
}
