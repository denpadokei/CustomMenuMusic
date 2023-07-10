using CustomMenuMusic.Configuration;
using CustomMenuMusic.Interfaces;
using CustomMenuMusic.Util;
using CustomMenuMusic.Views;
using IPA.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace CustomMenuMusic
{
    public class CustomMenuMusic : MonoBehaviour, ICustomMenuMusicable
    {
        private volatile bool _isLoadingAudioClip = false;
        private volatile bool _isChangeing = false;
        private bool _overrideCustomSongsList = false;
        private RandomObjectPicker<string> filePathPicker;
        public static AudioClip MenuMusic { get; set; }
        public static bool IsMenuSongPlaying { get; internal set; } = false;
        public static bool IsPauseOrFadeOut { get; internal set; } = true;
        public int ActiveChannel => this.PreviewPlayer.GetField<int, SongPreviewPlayer>("_activeChannel");
        public AudioSource ActiveAudioSource
        {
            get
            {
                try {
                    return this.AudioSources?.Length > (uint)this.ActiveChannel ? this.AudioSources[this.ActiveChannel].audioSource : null;
                }
                catch (Exception e) {
                    Logger.logger.Error(e);
                    return null;
                }
            }
        }
        public SongPreviewPlayer.AudioSourceVolumeController[] AudioSources => !this.PreviewPlayer
                    ? null
                    : this.PreviewPlayer.GetField<SongPreviewPlayer.AudioSourceVolumeController[], SongPreviewPlayer>("_audioSourceControllers");
        [Inject]
        private readonly INowPlayable nowPlay;
        [Inject]
        private readonly CMMTabViewController tabViewController;
        [Inject]
        private SongPreviewPlayer PreviewPlayer { get; }
        [Inject]
        private ResultsViewController ResultsViewController { get; }

        private WaitWhile waitWhileMenuMusic;
        private WaitWhile waitWhileLoading;

        public string CurrentSongPath { get; private set; }
        private static readonly string CustomSongsPath = Path.Combine(Environment.CurrentDirectory, "Beat Saber_Data", "CustomLevels");
        private static readonly string UserDataPath = Path.Combine(Environment.CurrentDirectory, "UserData", "CustomMenuMusic");
        private static readonly string MenuSongsPath = "MenuSongs";
        private static readonly string ResultSongsPath = "ResultSound";
        #region Unity Message
        private void OnDisable()
        {
            MainThreadInvoker.Instance.Enqueue(this.LoadAudioClip());
        }

        private async void Awake()
        {
            Logger.Log("Awake call");

            this.waitWhileMenuMusic = new WaitWhile(() => MenuMusic == null || !MenuMusic);
            this.waitWhileLoading = new WaitWhile(() => this._isLoadingAudioClip);
            if (!Directory.Exists(UserDataPath)) {
                _ = Directory.CreateDirectory(UserDataPath);
            }
            await this.GetSongsListAsync();
        }

        private void Start()
        {
            MainThreadInvoker.Instance.Enqueue(this.SetResultSong());
            this.Restart();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Period)) {
                this.Next();
            }
        }

        private void LateUpdate()
        {
            if (PluginConfig.Instance.Loop) {
                return;
            }
            if (IsPauseOrFadeOut || this._isChangeing || this._isLoadingAudioClip || this.PreviewPlayer.isActiveAndEnabled != true) {
                return;
            }
            if (this.ActiveAudioSource?.isPlaying != true) {
                this.Next();
            }
        }
        #endregion
        public Task GetSongsListAsync()
        {
            return this.GetSongsListAsync(PluginConfig.Instance.UseCustomMenuSongs);
        }

        public Task GetSongsListAsync(bool useCustomMenuSongs)
        {
            return Task.Run(() =>
                         {
                             if (!Directory.Exists(Path.Combine(UserDataPath, MenuSongsPath))) {
                                 _ = Directory.CreateDirectory(Path.Combine(UserDataPath, MenuSongsPath));
                             }
                             this.filePathPicker = useCustomMenuSongs && this.GetAllCustomMenuSongsAsync().Any()
                                 ? new RandomObjectPicker<string>(this.GetAllCustomMenuSongsAsync().ToArray(), 0f)
                                 : new RandomObjectPicker<string>(this.GetAllCustomSongsAsync().ToArray(), 0f);
                             this._overrideCustomSongsList = !this.filePathPicker.GetField<string[], RandomObjectPicker<string>>("_objects").Any();
                         });
        }

        private IEnumerable<string> GetAllCustomMenuSongsAsync()
        {
            return Directory.EnumerateFiles(Path.Combine(UserDataPath, MenuSongsPath), "*.ogg", SearchOption.AllDirectories);
        }

        private IEnumerable<string> GetAllCustomSongsAsync()
        {
            return this.DirSearch(CustomSongsPath);
        }

        private string GetResultSongPath()
        {
            if (!Directory.Exists(Path.Combine(UserDataPath, ResultSongsPath))) {
                _ = Directory.CreateDirectory(Path.Combine(UserDataPath, ResultSongsPath));
            }
            return Directory.EnumerateFiles(Path.Combine(UserDataPath, ResultSongsPath), "*.wav", SearchOption.AllDirectories).FirstOrDefault();
        }

        private IEnumerator SetResultSong()
        {
            var resultSound = this.GetResultSongPath();
            if (!PluginConfig.Instance.CustomResultSound) {
                yield break;
            }
            if (string.IsNullOrEmpty(resultSound)) {
                yield break;
            }
            Logger.Log("Loading file @ " + resultSound);
            var clipResponse = UnityWebRequestMultimedia.GetAudioClip(resultSound, AudioType.WAV);
            yield return clipResponse.SendWebRequest();
            if (clipResponse.error != null) {
                Logger.Log($"Unity Web Request Failed! Error: {clipResponse.error}", Logger.LogLevel.Error);
                yield break;
            }
            else {
                this.ResultsViewController.SetField("_levelClearedAudioClip", DownloadHandlerAudioClip.GetContent(clipResponse));
            }
        }

        private IEnumerable<string> DirSearch(string sDir)
        {
            return Directory.EnumerateFiles(sDir, "*.egg", SearchOption.AllDirectories);
        }

        private string GetNewSong()
        {
            UnityEngine.Random.InitState(Plugin.Instance.Seed);
            return this.filePathPicker.PickRandomObject();
        }

        private IEnumerator LoadAudioClip()
        {
            if (this._isLoadingAudioClip) {
                yield break;
            }

            this._isLoadingAudioClip = true;
            yield return new WaitUntil(() => this.filePathPicker != null || this._overrideCustomSongsList);

            this.CurrentSongPath = this.GetNewSong();
            this.tabViewController.CurrentSongPath = PluginConfig.Instance.UseCustomMenuSongs ? "" : this.CurrentSongPath;
            Logger.Log("Loading file @ " + this.CurrentSongPath);
            var clipResponse = UnityWebRequestMultimedia.GetAudioClip(this.CurrentSongPath, AudioType.OGGVORBIS);
            yield return clipResponse.SendWebRequest();
            if (clipResponse.error != null) {
                Logger.Log($"Unity Web Request Failed! Error: {clipResponse.error}", Logger.LogLevel.Error);
                this._isLoadingAudioClip = false;
                yield break;
            }
            else {
                if (MenuMusic != null) {
                    Destroy(MenuMusic);
                    MenuMusic = null;
                }
                MenuMusic = DownloadHandlerAudioClip.GetContent(clipResponse);
                MenuMusic.name = Path.GetFileName(this.CurrentSongPath);
            }
            yield return this.waitWhileMenuMusic;
            if (this._overrideCustomSongsList) {
                this.nowPlay.SetCurrentSong(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(MenuMusic.name), false);
            }
            else if (PluginConfig.Instance.ShowNowPlaying) {
                this.nowPlay.SetCurrentSong(this.CurrentSongPath);
            }
            this._isLoadingAudioClip = false;
        }

        private void Restart()
        {
            MainThreadInvoker.Instance.Enqueue(this.LoadAudioClip());
            MainThreadInvoker.Instance.Enqueue(this.StopActiveAudioSource);
            MainThreadInvoker.Instance.Enqueue(this.StartAudioSource());
        }

        public void Next()
        {
            MainThreadInvoker.Instance.Enqueue(this.LoadAudioClip());
            MainThreadInvoker.Instance.Enqueue(this.StartAudioSource());
        }

        private void StopActiveAudioSource()
        {
            this.PreviewPlayer.gameObject.SetActive(false);
        }

        private IEnumerator StartAudioSource()
        {
            if (this._isChangeing || !this.PreviewPlayer) {
                yield break;
            }
            this._isChangeing = true;
            yield return this.waitWhileLoading;
            try {
                if (this.PreviewPlayer) {
                    this.PreviewPlayer.gameObject.SetActive(true);
                    this.PreviewPlayer.CrossfadeTo(MenuMusic, AudioManagerSO.kDefaultMusicVolume, 0f, -1f, true, null);
                }
            }
            catch (Exception ex) {
                Logger.Log($"{ex}");
            }
            finally {
                this._isChangeing = false;
            }
        }
    }
}
