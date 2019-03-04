using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using CustomMenuMusic.Misc;
using Logger = CustomMenuMusic.Misc.Logger;

namespace CustomMenuMusic
{
    class CustomMenuMusic : MonoBehaviour
    {
        public static CustomMenuMusic instance;

        AudioClip _menuMusic;
        SongPreviewPlayer _previewPlayer = new SongPreviewPlayer();

        string musicPath;
        string optionName = "UseCustomMenuSongs";

        string[] AllSongFilePaths = new string[0];

        public static void OnLoad()
        {
            if (instance == null)
                instance = new GameObject("CustomMenuMusic").AddComponent<CustomMenuMusic>();

        }

        public void Awake()
        {
            DontDestroyOnLoad(this);

            SceneManager.sceneLoaded += SceneLoaded;

            if (!Directory.Exists("CustomMenuSongs"))
                Directory.CreateDirectory("CustomMenuSongs");

            GetSongsList();
        }

        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == "Menu")
            {
                if (!_previewPlayer == Resources.FindObjectsOfTypeAll<SongPreviewPlayer>().First())
                {
                    _previewPlayer = Resources.FindObjectsOfTypeAll<SongPreviewPlayer>().First();

                    StartCoroutine(LoadAudioClip());
                }
            }
        }

        private void GetSongsList()
        {
            if (CheckOptions())
            {
                AllSongFilePaths = GetAllCustomMenuSongs();
            }
            else
            {
                AllSongFilePaths = GetAllCustomSongs();
            }

            Logger.Log("Found " + AllSongFilePaths.Length + " Custom Menu Songs");
        }

        private bool CheckOptions()
        {
            return IllusionPlugin.ModPrefs.GetBool("CustomMenuMusic", optionName, true, true); ;
        }

        private string[] GetAllCustomMenuSongs()
        {
            if (!Directory.Exists("CustomMenuSongs"))
            {
                Directory.CreateDirectory("CustomMenuSongs");
            }

            string[] filePaths = Directory.GetFiles("CustomMenuSongs", "*.ogg");

            Logger.Log("CustomMenuSongs files found " + filePaths.Length);

            if (filePaths.Length == 0)
            {
                filePaths = GetAllCustomSongs();
            }

            return filePaths;
        }

        private string[] GetAllCustomSongs()
        {
            string[] filePaths = DirSearch("CustomSongs").ToArray();

            return filePaths;
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
                    {
                        files.AddRange(Directory.GetFiles(f, "*.ogg"));
                    }
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine("[CustomMenuMusic] " + excpt);
            }

            return files;
        }

        private void GetNewSong()
        {
            UnityEngine.Random.InitState(Environment.TickCount);
            var a = UnityEngine.Random.Range(0, AllSongFilePaths.Length);
            musicPath = AllSongFilePaths[a];
        }

        IEnumerator LoadAudioClip()
        {
            GetNewSong();

            Logger.Log("Loading file @ " + musicPath);
            WWW data = new WWW(Environment.CurrentDirectory + "\\" + musicPath);
            yield return data;
            try
            {
                _menuMusic = data.GetAudioClipCompressed(false, AudioType.OGGVORBIS) as AudioClip;

                if (_menuMusic != null)
                    _menuMusic.name = Path.GetFileName(musicPath);
                else
                    Logger.Log("No audio found!");
            }
            catch (Exception e)
            {
                Logger.Log("Can't load audio! Exception: " + e);
            }

            if (_previewPlayer != null && _menuMusic != null)
            {
                Logger.Log("Applying custom menu music...");

                _previewPlayer.SetField("_defaultAudioClip", _menuMusic);
                _previewPlayer.CrossfadeToDefault();
            }
        }
    }
}
