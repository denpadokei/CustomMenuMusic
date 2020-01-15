using System;
using System.Collections;
using System.Collections.Generic;
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

        private static readonly Vector3 LeftPosition = new Vector3(-2.55f, 2.2f, 1);
        private static readonly Vector3 CenterPosition = new Vector3(-1.05f, 2.45f, 2.6f);
        private static readonly Vector3 RightPosition = new Vector3(1.9f, 2.2f, 2.2f);
        private static readonly Vector3 LeftRotation = new Vector3(0, -60, 0);
        private static readonly Vector3 CenterRotation = new Vector3(0, 0, 0);
        private static readonly Vector3 RightRotation = new Vector3(0, 60, 0);

        private static readonly Vector3 scale = new Vector3(0.01f, 0.01f, 0.01f);
        private static readonly Vector2 CanvasSize = new Vector2(100, 50);
        private static readonly string LabelText = "Now Playing - ";

        internal static void OnLoad()
        {
            if (instance) return;

            instance = new GameObject("NowPlaying").AddComponent<NowPlaying>();
            Logger.Log("Created NowPlaying object.");
        }

        internal void SetCurrentSong(string newSong)
        {
            if (new DirectoryInfo(Path.GetDirectoryName(newSong)).Name.Equals("CustomMenuSongs"))
                songName = Path.GetFileNameWithoutExtension(newSong);
            else
            {
                try
                {
                    string songDirectory = Path.GetDirectoryName(newSong);
                    string infoFileName = (File.Exists(Path.Combine(songDirectory, "info.json"))) ? "info.json" : "info.dat";
                    dynamic songInfo = JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(songDirectory, infoFileName)));
                    songName = songInfo.songName ?? songInfo._songName;
                }
                catch (Exception e)
                {
                    Logger.Log(e.StackTrace, Logger.LogLevel.Error);
                    songName = Path.GetFileNameWithoutExtension(newSong);
                }
            }

            if (songName != null || songName != string.Empty)
                _nowPlayingText.text = $"{LabelText}{songName}";
        }

        internal void SetLocation(Config.Location location)
        {
            Vector3 position;
            Vector3 rotation;

            switch(location)
            {
                case Config.Location.LeftPanel:
                    position = LeftPosition;
                    rotation = LeftRotation;
                    break;
                case Config.Location.CenterPanel:
                    position = CenterPosition;
                    rotation = CenterRotation;
                    break;
                case Config.Location.RightPanel:
                    position = RightPosition;
                    rotation = RightRotation;
                    break;
                default:
                    DestroyImmediate(this.gameObject);
                    return;
            }

            gameObject.transform.position = position;
            gameObject.transform.eulerAngles = rotation;
            gameObject.transform.localScale = scale;
        }

        private void Awake()
        {
            SetLocation(Config.NowPlayingLocation);

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
            SetTextColor();

            _canvas.enabled = true;
        }

        private TextMeshProUGUI CreateText(RectTransform parent, string text, Vector2 anchoredPosition)
        {
            return CreateText(parent, text, anchoredPosition, new Vector2(0, 0));
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
            textMesh.overrideColorTags = true;
            textMesh.color = new Color(0.349f, 0.69f, 0.957f, 1);

            textMesh.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.sizeDelta = sizeDelta;
            textMesh.rectTransform.anchoredPosition = anchoredPosition;

            gameObj.SetActive(true);
            return textMesh;
        }

        internal void SetTextColor()
        {
            SetTextColor(Config.NowPlayingColor);
        }

        internal void SetTextColor(int colorIndex)
        {
            _nowPlayingText.color = colors[colorIndex].Item1;
        }

        internal static List<Tuple<Color, string>> colors = new List<Tuple<Color, string>>()
        {
            { Color.white, "White" },
            { Color.red, "Red" },
            { Color.blue, "Blue" },
            { Color.green, "Green" },
            { Color.gray, "Gray" },
            { Color.cyan, "Cyan" },
            { Color.magenta, "Magenta" },
            { Color.yellow, "Yellow" },
            { Color.black, "Black" },
            { MyColors.KlouderBlue, "Klouder Blue" },
            { MyColors.ElectricBlue, "Electric Blue" },
            { MyColors.MintGreen, "Mint Green" },
            { MyColors.Mauve, "Mauve" },
            { MyColors.Melrose, "Melrose" },
            { MyColors.Pink, "Pink" },
            { MyColors.Carnation, "Carnation" },
            { MyColors.CarnationPink, "Carnation Pink" },
            { MyColors.DabDab, "DabDab" },
            { MyColors.HintOfGreen, "Hint of Green" },
            { MyColors.BestGirl, "Best Girl" }
        };
    }

    struct MyColors
    {
        internal static Color KlouderBlue => new Color(0.349f, 0.69f, 0.957f, 1);
        internal static Color ElectricBlue => new Color(0, .98f, 2.157f);
        internal static Color Pink => new Color(1f, 0.388f, 0.7724f);
        internal static Color MintGreen => new Color(0.031f, 0.431f, 0.929f);
        internal static Color Melrose => new Color(0.686f, 0.647f, 0.992f);
        internal static Color Mauve => new Color(0.784f, 0.596f, 1);
        internal static Color CarnationPink => new Color(1, 0.631f, 0.831f);
        internal static Color Carnation => new Color(0.992f, 0.365f, 0.365f);
        internal static Color DabDab => new Color(0.855f, 0.741f, 0.671f);
        internal static Color HintOfGreen => new Color(0.875f, 1, 0.894f);
        internal static Color BestGirl => new Color(0.929f, 0.933f, 0.659f);
    }
}
