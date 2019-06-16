using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = CustomMenuMusic.Util.Logger;
using UnityEngine.Networking;
using BS_Utils.Utilities;

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

            SceneManager.activeSceneChanged += ActiveSceneChanged;

            if (!Directory.Exists(CustomMenuSongsPath))
                Directory.CreateDirectory(CustomMenuSongsPath);

            GetSongsList();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
                StartCoroutine(LoadAudioClip());

            if (Config.Loop || !_previewPlayer || _isLoadingAudioClip) return;

            try
            {
                if ((bool) !_previewPlayer.GetField<AudioSource[]>("_audioSources")[_previewPlayer.GetField<int>("_activeChannel")]?.isPlaying)
                    StartCoroutine(LoadAudioClip());
            }
            catch (Exception e)
            {
                Logger.Log($"Failed to get AudioSource - {_previewPlayer.GetField<int>("_activeChannel")}", Logger.LogLevel.Warning);
            }
        }

        private void ActiveSceneChanged(Scene arg0, Scene arg1)
        {
            if (arg1.name == "MenuCore")
            {
                if (Config.ShowNowPlaying)
                    NowPlaying.OnLoad();
                _sceneDidTransition = true;

                StartCoroutine(LoadAudioClip());
            }
        }

        internal void GetSongsList()
        {
            GetSongsList(Config.UseCustomMenuSongs);
        }

        internal void GetSongsList(bool useCustomMenuSongs)
        {
            if (useCustomMenuSongs)
                AllSongFilePaths = GetAllCustomMenuSongs();
            else
                AllSongFilePaths = GetAllCustomSongs();
        }

        private string[] GetAllCustomMenuSongs()
        {
            if (!Directory.Exists(CustomMenuSongsPath))
                Directory.CreateDirectory(CustomMenuSongsPath);

            string[] FilePaths = Directory.GetFiles(CustomMenuSongsPath, "*.*").Where(file => file.ToLower().EndsWith("ogg")).ToArray();

            Logger.Log($"Found {AllSongFilePaths.Length} Custom Menu Songs.", (AllSongFilePaths.Length > 0) ? Logger.LogLevel.Notice : Logger.LogLevel.Warning);

            if (FilePaths.Length == 0)
            {
                FilePaths = GetAllCustomSongs();
            }

            return FilePaths;
        }

        private string[] GetAllCustomSongs()
        {
            string[] FilePaths = DirSearch(CustomSongsPath).ToArray();

            Logger.Log($"Found {FilePaths.Length} Custom Songs.", Logger.LogLevel.Notice);

            return FilePaths;
        }

        private List<String> DirSearch(string sDir)
        {
            List<String> files = new List<String>();
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(Directory.GetFiles(d, "*.egg"));

                    foreach (string f in Directory.GetDirectories(d))
                        files.AddRange(Directory.GetFiles(f, "*.egg"));
                }
            }
            catch (System.Exception e)
            {
                Logger.Log(e.StackTrace, Logger.LogLevel.Error);
            }
            return files;
        }

        private void GetNewSong()
        {
            UnityEngine.Random.InitState(Environment.TickCount);
            int index = UnityEngine.Random.Range(0, AllSongFilePaths.Length);
            musicPath = AllSongFilePaths[index];
        }

        IEnumerator LoadAudioClip()
        {
            _isLoadingAudioClip = true;
            yield return new WaitUntil(() => _previewPlayer = Resources.FindObjectsOfTypeAll<SongPreviewPlayer>().First());
            if (_sceneDidTransition)
            {
                _previewPlayer.GetField<AudioSource[]>("_audioSources")[_previewPlayer.GetField<int>("_activeChannel")].Stop();
                _sceneDidTransition = false;
            }

            GetNewSong();
            if (musicPath.EndsWith("despacito.ogg"))
                Logger.Log("This is so sad, Beat Saber play Despacito", Logger.LogLevel.Notice);
            else
                Logger.Log("Loading file @ " + musicPath);
            UnityWebRequest songe = UnityWebRequestMultimedia.GetAudioClip($"{Environment.CurrentDirectory}\\{musicPath}", AudioType.OGGVORBIS);
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
                    StartCoroutine(LoadAudioClip());
                    yield break;
                }
            }

            yield return new WaitUntil(() => _menuMusic);

            if (_previewPlayer != null && _menuMusic != null)
            {
                try
                {
                    _previewPlayer.SetField("_defaultAudioClip", _menuMusic);
                    _previewPlayer.SetField("_ambientVolumeScale", Config.MenuMusicVolume);
                    _previewPlayer.CrossfadeToDefault();

                    _currentAudioSourceIndex = _previewPlayer.GetField<int>("_activeChannel");
                    _currentAudioSource = _previewPlayer.GetField<AudioSource[]>("_audioSources")[_currentAudioSourceIndex];
                    _currentAudioSource.loop = Config.Loop;

                    if (Config.ShowNowPlaying && (bool) NowPlaying.instance)
                        NowPlaying.instance?.SetCurrentSong(musicPath);
                }
                catch (Exception e)
                {
                    Logger.Log($"Oops! - {e.StackTrace}", Logger.LogLevel.Error);
                    StartCoroutine(LoadAudioClip());
                }
            }
            _isLoadingAudioClip = false;
        }
    }
}
