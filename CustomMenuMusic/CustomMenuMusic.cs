using BS_Utils.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Logger = CustomMenuMusic.Util.Logger;

namespace CustomMenuMusic
{
    class CustomMenuMusic : MonoBehaviour
    {
        internal static CustomMenuMusic instance;

        private AudioClip _menuMusic;
        private SongPreviewPlayer _previewPlayer;

        private AudioSource _currentAudioSource;
        private int _currentAudioSourceIndex;
        private bool _sceneDidTransition = false;
        private bool _isLoadingAudioClip = false;
        private bool _overrideCustomSongsList = false;

        private string musicPath;
        private const string builtInSongsFolder = "CustomMenuMusic.BuiltInSongs";

        private readonly string CustomSongsPath = "Beat Saber_Data\\CustomLevels";
        private readonly string CustomMenuSongsPath = "CustomMenuSongs";

        private string[] AllSongFilePaths = new string[0];

        internal static void OnLoad()
        {
            if (instance == null)
                instance = new GameObject("CustomMenuMusic").AddComponent<CustomMenuMusic>();
        }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            BSEvents.menuSceneActive += BSEvents_menuSceneActive;

            if (!Directory.Exists(CustomMenuSongsPath))
                Directory.CreateDirectory(CustomMenuSongsPath);

            GetSongsList();

            Logger.Log($"{LayerMasks.noteDebrisLayer}");
        }


        private void Update()
        {
            if (SceneManager.GetActiveScene().name == "GameCore") return;

            if (Input.GetKeyDown(KeyCode.Period))
                StartCoroutine(LoadAudioClip());

            if (Config.instance.Loop || !_previewPlayer || _isLoadingAudioClip) return;

            try
            {
                if ((bool)!_previewPlayer.GetField<AudioSource[]>("_audioSources")[_previewPlayer.GetField<int>("_activeChannel")]?.isPlaying)
                    StartCoroutine(LoadAudioClip(true));
            }
            catch (Exception e)
            {
                //Logger.Log($"Failed to get AudioSource - {_previewPlayer.GetField<int>("_activeChannel")}", Logger.LogLevel.Warning);
            }
        }

        private void BSEvents_menuSceneActive()
        {
            if (Config.instance.ShowNowPlaying)
                NowPlaying.OnLoad();
            _sceneDidTransition = true;

            StartCoroutine(LoadAudioClip());
        }

        internal void GetSongsList() => GetSongsList(Config.instance.UseCustomMenuSongs);

        internal void GetSongsList(bool useCustomMenuSongs)
        {
            if (useCustomMenuSongs)
                AllSongFilePaths = GetAllCustomMenuSongs();
            else
                AllSongFilePaths = GetAllCustomSongs();

            if (AllSongFilePaths.Count() == 0 || (DateTime.UtcNow.Month == 4 && DateTime.UtcNow.Day == 1))
                _overrideCustomSongsList = true;
        }

        private string[] GetAllCustomMenuSongs()
        {
            if (!Directory.Exists(CustomMenuSongsPath))
                Directory.CreateDirectory(CustomMenuSongsPath);

            var FilePaths = Directory.GetFiles(CustomMenuSongsPath, "*.*").Where(file => file.ToLower().EndsWith("ogg")).ToArray();

            Logger.Log($"Found {AllSongFilePaths.Length} Custom Menu Songs.", (AllSongFilePaths.Length > 0) ? Logger.LogLevel.Notice : Logger.LogLevel.Warning);

            if (FilePaths.Length == 0)
            {
                FilePaths = GetAllCustomSongs();
            }

            return FilePaths;
        }

        private string[] GetAllCustomSongs()
        {
            var FilePaths = DirSearch(CustomSongsPath).ToArray();

            Logger.Log($"Found {FilePaths.Length} Custom Songs.", Logger.LogLevel.Notice);

            return FilePaths;
        }

        private List<String> DirSearch(string sDir)
        {
            var files = new List<String>();
            try
            {
                foreach (var d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(Directory.GetFiles(d, "*.egg"));

                    foreach (var f in Directory.GetDirectories(d))
                        files.AddRange(Directory.GetFiles(f, "*.egg"));
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.StackTrace, Logger.LogLevel.Error);
            }
            return files;
        }

        private string GetNewSong()
        {
            UnityEngine.Random.InitState(Environment.TickCount);
            var index = UnityEngine.Random.Range(0, AllSongFilePaths.Length);
            return AllSongFilePaths[index];
        }


        IEnumerator LoadAudioClip(bool startAtBeginning = false, int attempts = 0)
        {
            if (attempts > 2) yield break;

            _isLoadingAudioClip = true;
            yield return new WaitUntil(() => _previewPlayer = Resources.FindObjectsOfTypeAll<SongPreviewPlayer>().First());
            if (_sceneDidTransition)
            {
                _previewPlayer.GetField<AudioSource[]>("_audioSources")[_previewPlayer.GetField<int>("_activeChannel")].Stop();
                _sceneDidTransition = false;
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
                    if (AllSongFilePaths.Count() > 0)
                        StartCoroutine(LoadAudioClip(startAtBeginning, ++attempts));
                }
            }
            else
            {
                musicPath = GetNewSong();
                Logger.Log("Loading file @ " + musicPath);
                var songe = UnityWebRequestMultimedia.GetAudioClip($"{Environment.CurrentDirectory}\\{musicPath}", AudioType.OGGVORBIS);
                yield return songe.SendWebRequest();

                if (songe.error != null)
                    Logger.Log($"Unity Web Request Failed! Error: {songe.error}", Logger.LogLevel.Error);
                else
                {
                    try
                    {
                        _menuMusic = DownloadHandlerAudioClip.GetContent(songe);

                        if (_menuMusic != null)
                            _menuMusic.name = Path.GetFileName(musicPath);
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
                    _previewPlayer.SetField("_ambientVolumeScale", Config.instance.MenuMusicVolume);
                    if (startAtBeginning)
                        _previewPlayer.CrossfadeTo(_previewPlayer.GetField<AudioClip>("_defaultAudioClip"), 0, -1, _previewPlayer.GetField<float>("_ambientVolumeScale"));
                    else
                        _previewPlayer.CrossfadeToDefault();

                    _currentAudioSourceIndex = _previewPlayer.GetField<int>("_activeChannel");
                    _currentAudioSource = _previewPlayer.GetField<AudioSource[]>("_audioSources")[_currentAudioSourceIndex];
                    _currentAudioSource.loop = Config.instance.Loop;

                    if (_overrideCustomSongsList)
                        NowPlaying.instance?.SetCurrentSong(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_menuMusic.name), false);
                    else if (Config.instance.ShowNowPlaying && (bool)NowPlaying.instance)
                        NowPlaying.instance?.SetCurrentSong(musicPath);
                }
                catch (Exception e)
                {
                    Logger.Log($"Oops! - {e.Message} : {e.StackTrace}", Logger.LogLevel.Error);
                    StartCoroutine(LoadAudioClip(startAtBeginning, ++attempts));
                }
            }
            _isLoadingAudioClip = false;
            musicPath = null;
        }
    }
}
