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
using UnityEngine.UI;
using Zenject;
using static CustomMenuMusic.Views.ConfigViewController;

namespace CustomMenuMusic
{
    public class NowPlaying : MonoBehaviour, INowPlayable
    {
        private GameObject rootObject;
        private Canvas _canvas;
        private CurvedCanvasSettings _curvedCanvasSettings;
        private CurvedTextMeshPro _nowPlayingText;
        private string songName;
        private static readonly Vector3 LeftPosition = new Vector3(-2.8f, 3f, 1);
        private static readonly Vector3 CenterPosition = new Vector3(0, 3.1f, 4.2f);
        private static readonly Vector3 RightPosition = new Vector3(2.8f, 3f, 1);
        private static readonly Vector3 LeftRotation = new Vector3(0, -60, 0);
        private static readonly Vector3 CenterRotation = new Vector3(0, 0, 0);
        private static readonly Vector3 RightRotation = new Vector3(0, 60, 0);

        private static readonly Vector3 scale = new Vector3(0.01f, 0.01f, 0.01f);
        private static readonly Vector2 CanvasSize = new Vector2(100, 50);
        private static readonly string LabelText = "Now Playing - ";

        [Inject]
        private readonly CMMTabViewController tabViewController;

        public void SetCurrentSong(string newSong, bool isPath = true)
        {
            if (newSong == null || newSong == string.Empty) {
                this._nowPlayingText.text = string.Empty;
                if (PluginConfig.Instance.ShowNowPlaying) {
                    Logger.Log("newSong was invalid", Logger.LogLevel.Warning);
                }

                return;
            }

            if (isPath) {
                if (new DirectoryInfo(Path.GetDirectoryName(newSong)).Name.Equals("MenuSongs")) {
                    this.songName = Path.GetFileNameWithoutExtension(newSong);
                }
                else {
                    try {
                        var songDirectory = Path.GetDirectoryName(newSong);
                        var infoFileName = File.Exists(Path.Combine(songDirectory, "info.json")) ? "info.json" : "info.dat";
                        dynamic songInfo = JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(songDirectory, infoFileName)));
                        this.songName = songInfo.songName ?? songInfo._songName;

                    }
                    catch (Exception e) {
                        Logger.Log(e.StackTrace, Logger.LogLevel.Error);
                        this.songName = Path.GetFileNameWithoutExtension(newSong);
                    }
                }
            }
            else {
                this.songName = newSong;
            }

            this._nowPlayingText.text = this.songName != null || this.songName != string.Empty ? $"{LabelText}{this.songName}" : string.Empty;

            this.tabViewController.SongName = this.songName;
        }

        public void SetLocation(ConfigViewController.Location location)
        {
            Vector3 position;
            Vector3 rotation;

            switch (location) {
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
                    Destroy(this.rootObject);
                    return;
            }

            this.rootObject.transform.position = position;
            this.rootObject.transform.eulerAngles = rotation;
            this.rootObject.transform.localScale = scale;
        }

        private void Awake()
        {
            Logger.Log("Awake call");
            this.rootObject = new GameObject("Nowplay Canvas", typeof(Canvas), typeof(CurvedCanvasSettings), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));

            var sizeFitter = this.rootObject.GetComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            this._canvas = this.rootObject.GetComponent<Canvas>();
            this._canvas.sortingOrder = 3;
            this._curvedCanvasSettings = this.rootObject.GetComponent<CurvedCanvasSettings>();
            this._curvedCanvasSettings.SetRadius(0);
            this._canvas.renderMode = RenderMode.WorldSpace;
            var rectTransform = this._canvas.transform as RectTransform;
            rectTransform.sizeDelta = CanvasSize;
            this.SetLocation((Location)PluginConfig.Instance.NowPlayingLocation);
            this._nowPlayingText = this.CreateText(this._canvas.transform as RectTransform, string.Empty, new Vector2(10, 31));
            rectTransform = this._nowPlayingText.transform as RectTransform;
            rectTransform.anchoredPosition = Vector2.one / 2;
            this._nowPlayingText.text = string.Empty;
            this._nowPlayingText.fontSize = 14;
            this.SetTextColor();
        }

        private void OnDestroy()
        {
            Destroy(this.rootObject);
        }

        private CurvedTextMeshPro CreateText(RectTransform parent, string text, Vector2 anchoredPosition)
        {
            return this.CreateText(parent, text, anchoredPosition, new Vector2(0, 0));
        }

        private CurvedTextMeshPro CreateText(RectTransform parent, string text, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            var gameObj = new GameObject("CustomUIText");
            gameObj.SetActive(false);

            var textMesh = gameObj.AddComponent<CurvedTextMeshPro>();
            var font = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstOrDefault(t => t.name == "Teko-Medium SDF");
            if (font != null) {
                textMesh.font = font;
            }
            textMesh.rectTransform.SetParent(parent, false);
            textMesh.text = text;
            textMesh.fontSize = 4;
            textMesh.overrideColorTags = true;
            textMesh.color = Color.white;

            textMesh.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.sizeDelta = sizeDelta;
            textMesh.rectTransform.anchoredPosition = anchoredPosition;

            gameObj.SetActive(true);
            return textMesh;
        }

        public void SetTextColor()
        {
            this.SetTextColor(PluginConfig.Instance.NowPlayingColor);
        }

        public void SetTextColor(int colorIndex)
        {
            this._nowPlayingText.color = colors[colorIndex].Item1;
        }

        internal static readonly List<(Color color, string colorName)> colors = new List<(Color color, string colorName)>
        {
            (Color.white, "White"),
            (Color.red, "Red"),
            (Color.blue, "Blue"),
            (Color.green, "Green"),
            (Color.gray, "Gray"),
            (Color.cyan, "Cyan"),
            (Color.magenta, "Magenta"),
            (Color.yellow, "Yellow"),
            (Color.black, "Black"),
            (MyColors.KlouderBlue, "Klouder Blue"),
            (MyColors.ElectricBlue, "Electric Blue"),
            (MyColors.MintGreen, "Mint Green"),
            (MyColors.Mauve, "Mauve"),
            (MyColors.Melrose, "Melrose"),
            (MyColors.Pink, "Pink"),
            (MyColors.Carnation, "Carnation"),
            (MyColors.CarnationPink, "Carnation Pink"),
            (MyColors.DabDab, "DabDab"),
            (MyColors.HintOfGreen, "Hint of Green"),
            (MyColors.BestGirl, "Best Girl")
        };
    }

    internal struct MyColors
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
