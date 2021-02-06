using BS_Utils.Utilities;
using CustomMenuMusic.Configuration;
using CustomMenuMusic.Interfaces;
using CustomMenuMusic.Util;
using CustomMenuMusic.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
        private AudioClip _menuMusic;
        private AudioSource _currentAudioSource;
        private int _currentAudioSourceIndex;
        private bool _sceneDidTransition = false;
        private bool _isLoadingAudioClip = false;
        private bool _overrideCustomSongsList = false;
        private string _currentSceneName = "MenuCore";

        [Inject]
        INowPlayable nowPlay;
        [Inject]
        CMMTabViewController tabViewController;
        [Inject]
        SongPreviewPlayer _previewPlayer;
        [Inject]
        ResultsViewController resultsViewController;

        public string CurrentSongPath { get; private set; }
        private const string builtInSongsFolder = "CustomMenuMusic.BuiltInSongs";

        private readonly string CustomSongsPath = "Beat Saber_Data\\CustomLevels";
        private readonly string CustomMenuSongsPath = "CustomMenuSongs";

        private IEnumerable<string> AllSongFilePaths = Array.Empty<string>();
        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            this._currentSceneName = arg1.name;
        }

        private async void Awake()
        {
            Logger.Log("Awake call");
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

            if (!Directory.Exists(CustomMenuSongsPath))
                Directory.CreateDirectory(CustomMenuSongsPath);
            await GetSongsListAsync();
            this.StartCoroutine(this.LoadAudioClip());
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        }

        private void Update()
        {
            if (this._currentSceneName == "GameCore") return;

            if (Input.GetKeyDown(KeyCode.Period))
                StartCoroutine(LoadAudioClip());

            if (PluginConfig.Instance.Loop || !_previewPlayer || _isLoadingAudioClip) return;

            try
            {
                if ((bool)!_previewPlayer?.GetField<AudioSource[]>("_audioSources")?[_previewPlayer.GetField<int>("_activeChannel")]?.isPlaying)
                    StartCoroutine(LoadAudioClip(true));
            }
            catch { }
        }

        public Task GetSongsListAsync() => GetSongsListAsync(PluginConfig.Instance.UseCustomMenuSongs);

        public Task GetSongsListAsync(bool useCustomMenuSongs)
        {
            return Task.Run(async () =>
            {
                AllSongFilePaths = Array.Empty<string>();

                if (useCustomMenuSongs)
                    AllSongFilePaths = await GetAllCustomMenuSongsAsync();
                else
                    AllSongFilePaths = await GetAllCustomSongsAsync();

                _overrideCustomSongsList = !AllSongFilePaths.Any();
            });
        }

        private async Task<IEnumerable<string>> GetAllCustomMenuSongsAsync()
        {
            if (!Directory.Exists(CustomMenuSongsPath))
                Directory.CreateDirectory(CustomMenuSongsPath);
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var filePaths = await Task.Run(() => Directory.EnumerateFiles(this.CustomMenuSongsPath, "*.ogg", SearchOption.AllDirectories));
            stopWatch.Stop();
            Logger.Log($"Found {AllSongFilePaths.Count()} Custom Menu Songs in {stopWatch.ElapsedMilliseconds} milliseconds.",
                (AllSongFilePaths.Any()) ? Logger.LogLevel.Debug : Logger.LogLevel.Warning);

            return filePaths;
        }

        private async Task<IEnumerable<string>> GetAllCustomSongsAsync()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var FilePaths = await DirSearchAsync(CustomSongsPath);
            stopWatch.Stop();
            Logger.Log($"Found {FilePaths.Count} Custom Songs in {stopWatch.ElapsedMilliseconds} milliseconds.", Logger.LogLevel.Debug);

            return FilePaths;
        }

        private async Task<List<String>> DirSearchAsync(string sDir)
        {
            var files = new List<String>();
            try
            {
                await Task.Run(() =>
                {
                    files.AddRange(Directory.EnumerateFiles(sDir, "*.egg", SearchOption.AllDirectories));
                });
            }
            catch (Exception e)
            {
                Logger.Log($"{ e.Message} - {e.StackTrace}", Logger.LogLevel.Error);
            }
            return files;
        }

        private string GetNewSong()
        {
            UnityEngine.Random.InitState(Environment.TickCount);
            var index = UnityEngine.Random.Range(0, AllSongFilePaths.Count() - 1);
            return AllSongFilePaths.ElementAt(index);
        }

        IEnumerator LoadAudioClip(bool startAtBeginning = false, int attempts = 0)
        {
            Logger.Log("LoadAudioClip start.");
            if (attempts > 2 || _isLoadingAudioClip) yield break;

            _isLoadingAudioClip = true;

            yield return new WaitUntil(() => AllSongFilePaths.Any() || _overrideCustomSongsList);

            yield return new WaitUntil(() => _previewPlayer = Resources.FindObjectsOfTypeAll<SongPreviewPlayer>().FirstOrDefault());
            try {
                if (_sceneDidTransition && (uint)_previewPlayer.GetField<int>("_activeChannel") < _previewPlayer.GetField<AudioSource[]>("_audioSources").Length) {
                    _previewPlayer.GetField<AudioSource[]>("_audioSources")[_previewPlayer.GetField<int>("_activeChannel")].Stop();
                    _sceneDidTransition = false;
                }
            }
            catch (Exception e) {
                Logger.Log($"{e}", Logger.LogLevel.Error);
            }
            if (_overrideCustomSongsList)
            {
                try
                {
                    var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CustomMenuMusic.Resources.default.asset");
                    var assetBundle = AssetBundle.LoadFromStream(stream);

                    _menuMusic = assetBundle.LoadAllAssets<AudioClip>().FirstOrDefault();
                    assetBundle.Unload(false);
                    Logger.Log("This is so sad, Beat Saber play Despacito", Logger.LogLevel.Notice);
                }
                catch (Exception e)
                {
                    Logger.Log("Error loading AssetBundle", Logger.LogLevel.Error);
                    Logger.Log($"{e.Message}\n{e.StackTrace}", Logger.LogLevel.Error);
                    _overrideCustomSongsList = false;
                    if (AllSongFilePaths.Any())
                        StartCoroutine(LoadAudioClip(startAtBeginning, ++attempts));
                }
            }
            else
            {
                try {
                    CurrentSongPath = GetNewSong();
                    if (!PluginConfig.Instance.UseCustomMenuSongs) {
                        this.tabViewController.CurrentSongPath = this.CurrentSongPath;
                    }
                    Logger.Log("Loading file @ " + CurrentSongPath);
                }
                catch (Exception e) {
                    Logger.Log($"{e}", Logger.LogLevel.Error);
                }
                var songe = UnityWebRequestMultimedia.GetAudioClip($"{Environment.CurrentDirectory}\\{CurrentSongPath}", AudioType.OGGVORBIS);
                yield return songe.SendWebRequest();

                if (songe.error != null)
                    Logger.Log($"Unity Web Request Failed! Error: {songe.error}", Logger.LogLevel.Error);
                else
                {
                    try
                    {
                        _menuMusic = DownloadHandlerAudioClip.GetContent(songe);

                        if (_menuMusic != null)
                            _menuMusic.name = Path.GetFileName(CurrentSongPath);
                        else
                            Logger.Log("No audio found!", Logger.LogLevel.Warning);
                    }
                    catch (Exception e)
                    {
                        Logger.Log("Can't load audio! Exception: " + e, Logger.LogLevel.Error);
                        StartCoroutine(LoadAudioClip(startAtBeginning, ++attempts));
                        yield break;
                    }
                }
            }

            yield return new WaitUntil(() => _menuMusic);

            if (_previewPlayer != null && _menuMusic != null)
            {
                try
                {
                    _previewPlayer.SetField("_defaultAudioClip", _menuMusic);
                    _previewPlayer.SetField("_ambientVolumeScale", PluginConfig.Instance.MenuMusicVolume);
                    if (startAtBeginning)
                        _previewPlayer.CrossfadeTo(_previewPlayer.GetField<AudioClip>("_defaultAudioClip"), 0, -1, _previewPlayer.GetField<float>("_ambientVolumeScale"));
                    else
                        _previewPlayer.CrossfadeToDefault();

                    _currentAudioSourceIndex = _previewPlayer.GetField<int>("_activeChannel");
                    _currentAudioSource = _previewPlayer.GetField<AudioSource[]>("_audioSources")[_currentAudioSourceIndex];
                    _currentAudioSource.loop = PluginConfig.Instance.Loop;

                    if (_overrideCustomSongsList)
                        this.nowPlay.SetCurrentSong(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_menuMusic.name), false);
                    else if (PluginConfig.Instance.ShowNowPlaying)
                        this.nowPlay.SetCurrentSong(CurrentSongPath);
                }
                catch (Exception e)
                {
                    Logger.Log($"Oops! - {e.Message} : {e.StackTrace}", Logger.LogLevel.Error);
                    StartCoroutine(LoadAudioClip(startAtBeginning, ++attempts));
                }
            }
            _isLoadingAudioClip = false;
            Logger.Log("LoadAudioClip end");
        }
    }
}
