using BS_Utils.Utilities;
using CustomMenuMusic.Configuration;
using CustomMenuMusic.Interfaces;
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
using UnityEngine.SceneManagement;
using Zenject;
using Logger = CustomMenuMusic.Util.Logger;

namespace CustomMenuMusic
{
    public class CustomMenuMusic : MonoBehaviour, ICustomMenuMusicable
    {
        private volatile bool _sceneDidTransition = false;
        private volatile bool _isLoadingAudioClip = false;
        private volatile bool _isChangeing = false;
        private bool _overrideCustomSongsList = false;
        private string _currentSceneName = "MenuCore";
        private RandomObjectPicker<string> filePathPicker;
        public static AudioClip MenuMusic { get; set; }
        public int ActiveChannel => PreviewPlayer.GetField<int, SongPreviewPlayer>("_activeChannel");
        public AudioSource ActiveAudioSource
        {
            get
            {
                if (this.AudioSources.Length > (uint)this.ActiveChannel) {
                    return this.AudioSources[this.ActiveChannel];
                }
                else {
                    return null;
                }
            }
        }
        public AudioSource[] AudioSources => this.PreviewPlayer.GetField<AudioSource[], SongPreviewPlayer>("_audioSources");
        [Inject]
        INowPlayable nowPlay;
        [Inject]
        CMMTabViewController tabViewController;
        [Inject]
        SongPreviewPlayer PreviewPlayer { get; }
        [Inject]
        ResultsViewController ResultsViewController { get; }

        private WaitWhile waitWhileMenuMusic;
        private WaitWhile waitWhileLoading;

        public string CurrentSongPath { get; private set; }
        private const string builtInSongsFolder = "CustomMenuMusic.BuiltInSongs";

        private static readonly string CustomSongsPath = Path.Combine(Environment.CurrentDirectory, "Beat Saber_Data", "CustomLevels");
        private static readonly string UserDataPath = Path.Combine(Environment.CurrentDirectory, "UserData", "CustomMenuMusic");
        private static readonly string MenuSongsPath = "MenuSongs";
        private static readonly string ResultSongsPath = "ResultSound";
        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            this._currentSceneName = arg1.name;
        }
        #region Unity Message
        private async void Awake()
        {
            Logger.Log("Awake call");

            this.waitWhileMenuMusic = new WaitWhile(() => MenuMusic == null || !MenuMusic);
            this.waitWhileLoading = new WaitWhile(() => this._isLoadingAudioClip);
            this._sceneDidTransition = true;
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
            BSEvents.menuSceneActive += this.BSEvents_menuSceneActive;
            HMMainThreadDispatcher.instance.Enqueue(this.SetVolume());
            PluginConfig.Instance.OnSettingChanged += this.Instance_OnSettingChanged;
            if (!Directory.Exists(UserDataPath))
                Directory.CreateDirectory(UserDataPath);
            HMMainThreadDispatcher.instance.Enqueue(this.SetResultSong());
            await GetSongsListAsync();
            this.Restart();
        }

        private void Instance_OnSettingChanged(PluginConfig obj)
        {
            try {
                this.PreviewPlayer.SetField("_ambientVolumeScale", obj.MenuMusicVolume);
            }
            catch (Exception e) {
                Logger.Log($"{e}");
            }
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
            BSEvents.menuSceneActive -= this.BSEvents_menuSceneActive;
            PluginConfig.Instance.OnSettingChanged -= this.Instance_OnSettingChanged;
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
            if (this._isChangeing || this._isLoadingAudioClip || this.PreviewPlayer.isActiveAndEnabled != true) {
                return;
            }
            if (this.ActiveAudioSource?.isPlaying != true) {
                this.Next();
            }
        }
        #endregion
        /// <summary>
        /// よくわかんないけど残してる処理
        /// </summary>
        private void BSEvents_menuSceneActive()
        {
            this._sceneDidTransition = true;
        }

        private IEnumerator SetVolume()
        {
            yield return new WaitWhile(() => this.PreviewPlayer == null || !this.PreviewPlayer);
            this.PreviewPlayer.SetField("_ambientVolumeScale", PluginConfig.Instance.MenuMusicVolume);
        }

        public Task GetSongsListAsync() => GetSongsListAsync(PluginConfig.Instance.UseCustomMenuSongs);

        public Task GetSongsListAsync(bool useCustomMenuSongs)
        {
            return Task.Run(() =>
            {
                if (!Directory.Exists(Path.Combine(UserDataPath, MenuSongsPath))) {
                    Directory.CreateDirectory(Path.Combine(UserDataPath, MenuSongsPath));
                }
                if (useCustomMenuSongs && GetAllCustomMenuSongsAsync().Any()) {
                    this.filePathPicker = new RandomObjectPicker<string>(GetAllCustomMenuSongsAsync().ToArray(), 0f);
                }
                else {
                    this.filePathPicker = new RandomObjectPicker<string>(GetAllCustomSongsAsync().ToArray(), 0f);
                }
                _overrideCustomSongsList = !filePathPicker.GetField<string[], RandomObjectPicker<string>>("_objects").Any();
            });
        }

        private IEnumerable<string> GetAllCustomMenuSongsAsync()
        {
            return Directory.EnumerateFiles(Path.Combine(UserDataPath, MenuSongsPath), "*.ogg", SearchOption.AllDirectories);
        }

        private IEnumerable<string> GetAllCustomSongsAsync()
        {
            return DirSearch(CustomSongsPath);
        }

        private string GetResultSongPath()
        {
            if (!Directory.Exists(Path.Combine(UserDataPath, ResultSongsPath))) {
                Directory.CreateDirectory(Path.Combine(UserDataPath, ResultSongsPath));
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
            if (_isLoadingAudioClip) yield break;
            _isLoadingAudioClip = true;
            yield return new WaitUntil(() => this.filePathPicker != null || _overrideCustomSongsList);

            CurrentSongPath = GetNewSong();
            this.tabViewController.CurrentSongPath = PluginConfig.Instance.UseCustomMenuSongs ? "" : this.CurrentSongPath;
            Logger.Log("Loading file @ " + CurrentSongPath);
            var clipResponse = UnityWebRequestMultimedia.GetAudioClip(CurrentSongPath, AudioType.OGGVORBIS);
            yield return clipResponse.SendWebRequest();
            if (clipResponse.error != null) {
                Logger.Log($"Unity Web Request Failed! Error: {clipResponse.error}", Logger.LogLevel.Error);
                _isLoadingAudioClip = false;
                yield break;
            }
            else {
                if (MenuMusic != null) {
                    Destroy(MenuMusic);
                    MenuMusic = null;
                }
                MenuMusic = DownloadHandlerAudioClip.GetContent(clipResponse);
                MenuMusic.name = Path.GetFileName(CurrentSongPath);
            }
            yield return this.waitWhileMenuMusic;
            if (_overrideCustomSongsList) {
                this.nowPlay.SetCurrentSong(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(MenuMusic.name), false);
            }
            else if (PluginConfig.Instance.ShowNowPlaying) {
                this.nowPlay.SetCurrentSong(CurrentSongPath);
            }
            _isLoadingAudioClip = false;
        }

        private void Restart()
        {
            HMMainThreadDispatcher.instance.Enqueue(this.LoadAudioClip());
            HMMainThreadDispatcher.instance.Enqueue(this.StopActiveAudioSource);
            HMMainThreadDispatcher.instance.Enqueue(this.StartAudioSource());
        }

        private void Next()
        {
            HMMainThreadDispatcher.instance.Enqueue(this.LoadAudioClip());
            HMMainThreadDispatcher.instance.Enqueue(this.StartAudioSource(true));
        }

        private void StopActiveAudioSource()
        {
            this.PreviewPlayer.gameObject.SetActive(false);
        }
        private IEnumerator StartAudioSource(bool startAtBeginning = false)
        {
            if (this._isChangeing || !this.PreviewPlayer) {
                yield break;
            }
            this._isChangeing = true;
            yield return waitWhileLoading;
            try {
                this.PreviewPlayer.gameObject.SetActive(true);
                if (PluginConfig.Instance.Loop) {
                    this.PreviewPlayer.CrossfadeTo(MenuMusic, 0f, -1, PluginConfig.Instance.MenuMusicVolume);
                }
                else {
                    if (startAtBeginning) {
                        this.PreviewPlayer.CrossfadeTo(MenuMusic, 0f, MenuMusic.length, PluginConfig.Instance.MenuMusicVolume);
                    }
                    else {
                        this.PreviewPlayer.CrossfadeTo(MenuMusic, UnityEngine.Random.Range(0.1f, MenuMusic.length / 2), MenuMusic.length, PluginConfig.Instance.MenuMusicVolume);
                    }
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
