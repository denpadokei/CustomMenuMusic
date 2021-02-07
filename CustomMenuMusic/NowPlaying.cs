using CustomMenuMusic.Configuration;
using CustomMenuMusic.Interfaces;
using CustomMenuMusic.Views;
using HMUI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using static CustomMenuMusic.Views.ConfigViewController;
using Logger = CustomMenuMusic.Util.Logger;

namespace CustomMenuMusic
{
    public class NowPlaying : MonoBehaviour, INowPlayable
    {
        private Canvas _canvas;
        private CurvedCanvasSettings _curvedCanvasSettings;
        private CurvedTextMeshPro _nowPlayingText;
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

        [Inject]
        CMMTabViewController tabViewController;

        public void SetCurrentSong(string newSong, bool isPath = true)
        {
            if (newSong == null || newSong == string.Empty)
            {
                _nowPlayingText.text = String.Empty;
                if (PluginConfig.Instance.ShowNowPlaying) Logger.Log("newSong was invalid", Logger.LogLevel.Warning);
                return;
            }

            if (isPath)
            {
                if (new DirectoryInfo(Path.GetDirectoryName(newSong)).Name.Equals("CustomMenuSongs"))
                    songName = Path.GetFileNameWithoutExtension(newSong);
                else
                {
                    try
                    {
                        var songDirectory = Path.GetDirectoryName(newSong);
                        var infoFileName = (File.Exists(Path.Combine(songDirectory, "info.json"))) ? "info.json" : "info.dat";
                        dynamic songInfo = JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(songDirectory, infoFileName)));
                        songName = songInfo.songName ?? songInfo._songName;
                        
                    }
                    catch (Exception e)
                    {
                        Logger.Log(e.StackTrace, Logger.LogLevel.Error);
                        songName = Path.GetFileNameWithoutExtension(newSong);
                    }
                }
            }
            else
                songName = newSong;

            if (songName != null || songName != string.Empty)
                _nowPlayingText.text = $"{LabelText}{songName}";
            else
                _nowPlayingText.text = String.Empty;
            tabViewController.SongName = this.songName;
        }

        public void SetLocation(ConfigViewController.Location location)
        {
            Vector3 position;
            Vector3 rotation;

            switch (location)
            {
                case ConfigViewController.Location.LeftPanel:
                    position = LeftPosition;
                    rotation = LeftRotation;
                    break;
                case ConfigViewController.Location.CenterPanel:
                    position = CenterPosition;
                    rotation = CenterRotation;
                    break;
                case ConfigViewController.Location.RightPanel:
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
            Logger.Log("Awake call");
            SetLocation((Location)PluginConfig.Instance.NowPlayingLocation);

            _canvas = gameObject.AddComponent<Canvas>();
            _curvedCanvasSettings = gameObject.AddComponent<CurvedCanvasSettings>();
            _canvas.renderMode = RenderMode.WorldSpace;
            _canvas.enabled = false;
            var rectTransform = _canvas.transform as RectTransform;
            rectTransform.sizeDelta = CanvasSize;

            _nowPlayingText = CreateText(_canvas.transform as RectTransform, String.Empty, new Vector2(10, 31));
            rectTransform = _nowPlayingText.transform as RectTransform;
            rectTransform.SetParent(_canvas.transform, false);
            rectTransform.anchoredPosition = new Vector2(10, 31);
            rectTransform.sizeDelta = new Vector2(100, 20);
            _nowPlayingText.text = String.Empty;
            _nowPlayingText.fontSize = 14;
            SetTextColor();

            _canvas.enabled = true;
        }
        private CurvedTextMeshPro CreateText(RectTransform parent, string text, Vector2 anchoredPosition) => CreateText(parent, text, anchoredPosition, new Vector2(0, 0));

        private CurvedTextMeshPro CreateText(RectTransform parent, string text, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            var gameObj = new GameObject("CustomUIText");
            gameObj.SetActive(false);

            var textMesh = gameObj.AddComponent<CurvedTextMeshPro>();
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

        public void SetTextColor() => SetTextColor(PluginConfig.Instance.NowPlayingColor);

        public void SetTextColor(int colorIndex) => _nowPlayingText.color = colors[colorIndex].Item1;

        internal static readonly List<Tuple<Color, string>> colors = new List<Tuple<Color, string>>()
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
