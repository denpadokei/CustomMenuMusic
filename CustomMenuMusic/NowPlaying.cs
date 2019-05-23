using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using Logger = CustomMenuMusic.Util.Logger; 

namespace CustomMenuMusic
{
    class NowPlaying : MonoBehaviour
    {
        internal static NowPlaying instance;

        private Canvas _canvas;
        private TMP_Text _nowPlayingText;
        private string songName;

        private static readonly Vector3 Position = new Vector3(1.9f, 2.2f, 2.2f);
        private static readonly Vector3 Rotation = new Vector3(0, 60, 0);
        private static readonly Vector3 Scale = new Vector3(0.01f, 0.01f, 0.01f);
        private static readonly Vector2 CanvasSize = new Vector2(100, 50);
        private static readonly string LabelText = "Now Playing - ";

        internal static void OnLoad()
        {
            if (instance == null)
            {
                instance = new GameObject("NowPlaying").AddComponent<NowPlaying>();
                Logger.Log("Created NowPlaying object.");
            }

        }

        internal void SetCurrentSong(string newSong)
        {
            if (new DirectoryInfo(Path.GetDirectoryName(newSong)).Name.Equals("CustomMenuSongs"))
            {
                songName = Path.GetFileNameWithoutExtension(newSong);
            }
            else
            {
                try
                {
                    dynamic songInfo = JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(Path.GetDirectoryName(newSong), "info.json")));
                    songName = songInfo.songName;
                }
                catch (Exception e)
                {
                    Logger.Log(e.StackTrace, Logger.LogLevel.Error);
                    songName = Path.GetFileNameWithoutExtension(newSong);
                }
            }

            _nowPlayingText.text = $"{LabelText}{songName}";
        }

        private void Awake()
        {
            gameObject.transform.position = Position;
            gameObject.transform.eulerAngles = Rotation;
            gameObject.transform.localScale = Scale;

            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;
            _canvas.enabled = false;
            var rectTransform = _canvas.transform as RectTransform;
            rectTransform.sizeDelta = CanvasSize;

            _nowPlayingText = CreateText(_canvas.transform as RectTransform, LabelText, new Vector2(10, 31));
            rectTransform = _nowPlayingText.transform as RectTransform;
            rectTransform.SetParent(_canvas.transform, false);
            rectTransform.anchoredPosition = new Vector2(10, 31);
            rectTransform.sizeDelta = new Vector2(100, 20);
            _nowPlayingText.text = LabelText;
            _nowPlayingText.fontSize = 14;

            _canvas.enabled = true;
        }

        private TextMeshProUGUI CreateText(RectTransform parent, string text, Vector2 anchoredPosition)
        {
            return CreateText(parent, text, anchoredPosition, new Vector2(60f, 10f));
        }

        private TextMeshProUGUI CreateText(RectTransform parent, string text, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            GameObject gameObj = new GameObject("CustomUIText");
            gameObj.SetActive(false);

            TextMeshProUGUI textMesh = gameObj.AddComponent<TextMeshProUGUI>();
            textMesh.font = Instantiate(Resources.FindObjectsOfTypeAll<TMP_FontAsset>().First(t => t.name == "Teko-Medium SDF No Glow"));
            textMesh.rectTransform.SetParent(parent, false);
            textMesh.text = text;
            textMesh.fontSize = 4;
            textMesh.color = Color.white;

            textMesh.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.sizeDelta = sizeDelta;
            textMesh.rectTransform.anchoredPosition = anchoredPosition;

            gameObj.SetActive(true);
            return textMesh;
        }
    }
}
