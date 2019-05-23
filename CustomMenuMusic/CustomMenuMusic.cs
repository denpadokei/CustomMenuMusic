using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = CustomMenuMusic.Util.Logger;
using BS_Utils.Utilities;
using UnityEngine.Networking;

namespace CustomMenuMusic
{
    class CustomMenuMusic : MonoBehaviour
    {
        static CustomMenuMusic instance;

        AudioClip _menuMusic;
        SongPreviewPlayer _previewPlayer;
        Config config;

        private string musicPath;
        private const string useMenuSongsOption = "UseCustomMenuSongs";
        private const string showNowPlayingOption = "ShowNowPlaying";
        private const string builtInSongsFolder = "CustomMenuMusic.BuiltInSongs";

        private string[] AllSongFilePaths = new string[0];

        private bool UseMenuSongs
        {
            get { return config.GetBool("CustomMenuMusic", useMenuSongsOption, true, true); }
        }

        private bool ShowNowPlaying
        {
            get { return config.GetBool("CustomMenuMusic", showNowPlayingOption, true, true); }
        }

        internal static void OnLoad()
        {
            if (instance == null)
                instance = new GameObject("CustomMenuMusic").AddComponent<CustomMenuMusic>();
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);

            SceneManager.activeSceneChanged += ActiveSceneChanged;

            if (!Directory.Exists("CustomMenuSongs"))
                Directory.CreateDirectory("CustomMenuSongs");

            config = new Config("CustomMenuMusic");
            Logger.Log($"{useMenuSongsOption}: {UseMenuSongs}", Logger.LogLevel.Debug);
            Logger.Log($"{showNowPlayingOption}: {ShowNowPlaying}", Logger.LogLevel.Debug);

            GetSongsList();
        }

        private void ActiveSceneChanged(Scene arg0, Scene arg1)
        {
            if (arg1.name == "MenuCore")
            {
                if (ShowNowPlaying)
                    NowPlaying.OnLoad();

                StartCoroutine(LoadAudioClip());
            }
        }

        private void GetSongsList()
        {
            if (UseMenuSongs)
                AllSongFilePaths = GetAllCustomMenuSongs();
            else
                AllSongFilePaths = GetAllCustomSongs();

        }

        private string[] GetAllCustomMenuSongs()
        {
            if (!Directory.Exists("CustomMenuSongs"))
                Directory.CreateDirectory("CustomMenuSongs");

            string[] AllSongsFilePaths = Directory.GetFiles("CustomMenuSongs", "*.*").Where(file => file.ToLower().EndsWith("ogg")).ToArray();

            Logger.Log($"Found {AllSongFilePaths.Length} songs in CustomMenuSongs.", (AllSongFilePaths.Length > 0) ? Logger.LogLevel.Notice : Logger.LogLevel.Warning);

            if (AllSongsFilePaths.Length == 0)
            {
                AllSongsFilePaths = GetAllCustomSongs();
            }

            return AllSongsFilePaths;
        }

        private string[] GetAllCustomSongs()
        {
            string[] AllSongsFilePaths = DirSearch("CustomSongs").ToArray();

            Logger.Log($"Found {AllSongFilePaths.Length} songs in CustomSongs.", Logger.LogLevel.Notice);

            return AllSongsFilePaths;
        }

        private List<String> DirSearch(string sDir)
        {
            List<String> files = new List<String>();
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(Directory.GetFiles(d, "*.ogg"));

                    foreach (string f in Directory.GetDirectories(d))
                        files.AddRange(Directory.GetFiles(f, "*.ogg"));
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
            yield return new WaitUntil(() => _previewPlayer = Resources.FindObjectsOfTypeAll<SongPreviewPlayer>().First());
            _previewPlayer.GetField<AudioSource[]>("_audioSources")[_previewPlayer.GetField<int>("_activeChannel")].Stop();

            GetNewSong();
            if (musicPath.EndsWith("despacito.ogg"))
                Logger.Log("This is so sad, Beat Saber play Despacito", Logger.LogLevel.Notice);
            else
                Logger.Log("Loading file @ " + musicPath);
            UnityWebRequest songe = UnityWebRequestMultimedia.GetAudioClip($"{Environment.CurrentDirectory}\\{musicPath}", AudioType.OGGVORBIS);
            yield return songe.SendWebRequest();
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
            }

            yield return new WaitUntil(() => _menuMusic);

            if (_previewPlayer != null && _menuMusic != null)
            {
                try
                {
                    _previewPlayer.SetField("_defaultAudioClip", _menuMusic);
                    _previewPlayer.CrossfadeToDefault();
                    NowPlaying.instance.SetCurrentSong(musicPath);
                }
                catch (Exception e)
                {
                    Logger.Log($"Oops! - {e.StackTrace}", Logger.LogLevel.Error);
                }
            }
        }
    }
}
