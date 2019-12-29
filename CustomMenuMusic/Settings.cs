using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using BS_Utils.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomMenuMusic
{
    partial class Config : PersistentSingleton<Config>
    {
        internal static bool initialized;

        [UIAction("#apply")]
        private void Apply()
        {
            Util.Logger.Log("Saving Config!", Util.Logger.LogLevel.Notice);
            Config.instance.Save();
        }

        //private static IEnumerator PresentTest()
        //{
        //    yield return new WaitForSeconds(1);
        //    Settings testViewController = BeatSaberMarkupLanguage.BeatSaberUI.CreateViewController<Settings>();
        //    Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First().InvokeMethod("PresentViewController", new object[] { testViewController, null, false });
        //}

        [UIValue("volume-range")]
        public List<object> VolumeRange => Enumerable.Range(0, 20).Select(x => x * 0.05f).Cast<object>().ToList();

        [UIValue("now-playing-location-range")]
        public List<object> NowPlayingLocationRange => Enumerable.Range(0, 3).Select(x => (int)x).Cast<object>().ToList();

        [UIAction("format-now-playing-location")]
        public string OnFormatNowPlayingLocation(int obj)
        {
            return locationNames[obj];
        }

        [UIValue("preset-colors")]
        public List<object> presetColors = Enumerable.Range(0, NowPlaying.colors.Count).Select(x => (int)x).Cast<object>().ToList();

        [UIAction("format-text-color")]
        public string OnFormatTextColor(int value)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(NowPlaying.colors[(int)value].Item1)}>{NowPlaying.colors[(int)value].Item2}";
        }

        internal void CreateSettingsUI()
        {
            if (initialized) return;
            initialized = true;

            //SharedCoroutineStarter.instance.StartCoroutine(PresentTest());
            Util.Logger.Log("Creating Settings UI", Util.Logger.LogLevel.Notice);


            BSMLSettings.instance.AddSettingsMenu("Custom Menu Music", "CustomMenuMusic.Resources.Settings.bsml", PersistentSingleton<Config>.instance);

            //    var subMenuCMM = SettingsUI.CreateSubMenu("Menu Music");

        

            //    var nowPlayingMenu = subMenuCMM.AddSubMenu("Now Playing", "Tweak Now Playing Options Here", true);

            //    var showNowPlayingOption = nowPlayingMenu.AddBool("Show Now Playing");
            //    showNowPlayingOption.GetValue += delegate { return Config.ShowNowPlaying; };
            //    showNowPlayingOption.SetValue += delegate (bool value) { Config.ShowNowPlaying = value; };
            //    showNowPlayingOption.EnabledText = "Yes";
            //    showNowPlayingOption.DisabledText = "No";

            //    var NowPlayingLocationOption = nowPlayingMenu.AddList("Location", Enumerable.Range(0, 3).Select(x => (float) x).ToArray());
            //    NowPlayingLocationOption.GetValue += delegate { return (float) Config.NowPlayingLocation; };
            //    NowPlayingLocationOption.SetValue += delegate (float value) { Config.NowPlayingLocation = (Config.Location) value; };
            //    NowPlayingLocationOption.FormatValue += delegate (float value) { return locationNames[(int) value]; };

            //    float[] presetColors = new float[NowPlaying.colors.Count];
            //    for (int i = 0; i < NowPlaying.colors.Count; i++) presetColors[i] = i;
            //    var NowPlayingColorOption = nowPlayingMenu.AddList("Text Color", presetColors);
            //    NowPlayingColorOption.GetValue += delegate { return Config.NowPlayingColor; };
            //    NowPlayingColorOption.SetValue += delegate (float value) { Config.NowPlayingColor = (int) value; };
            //    NowPlayingColorOption.FormatValue += delegate (float value) { return $"<color=#{ColorUtility.ToHtmlStringRGB(NowPlaying.colors[(int) value].Item1)}>{NowPlaying.colors[(int) value].Item2}"; };
        }

        static readonly List<String> locationNames = new List<string> { "Left Panel", "Center Panel", "Right Panel" };
    }
}
