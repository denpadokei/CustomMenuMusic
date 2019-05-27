using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CustomUI;
using CustomUI.Settings;
using UnityEngine;

namespace CustomMenuMusic
{
    class SettingsMenu
    {
        internal static bool initialized;

        internal static void CreateSettingsUI()
        {
            if (initialized) return;
            initialized = true;

            Util.Logger.Log("Creating Settings UI", Util.Logger.LogLevel.Notice);

            var subMenuCMM = SettingsUI.CreateSubMenu("Menu Music");

            var coreMenu = subMenuCMM.AddSubMenu("Core Settings", "Adjust Core Custom Menu Music Settings Here", true);

            var sourceOption = coreMenu.AddBool("Use Custom Menu Songs");
            sourceOption.GetValue += delegate { return Config.UseCustomMenuSongs; };
            sourceOption.SetValue += delegate (bool value) { Config.UseCustomMenuSongs = value; };
            sourceOption.EnabledText = "Yes";
            sourceOption.DisabledText = "No";

            var LoopOption = coreMenu.AddBool("Loop");
            LoopOption.GetValue += delegate { return Config.Loop; };
            LoopOption.SetValue += delegate (bool value) { Config.Loop = value; };
            LoopOption.EnabledText = "Yes";
            LoopOption.DisabledText = "No";

            var volumeOption = coreMenu.AddList("Volume", Enumerable.Range(0, 20).Select(x => x * 0.05f).ToArray());
            volumeOption.GetValue += delegate { return Config.MenuMusicVolume; };
            volumeOption.SetValue += delegate (float value) { Config.MenuMusicVolume = value; };
            volumeOption.FormatValue += delegate (float value) { return $"{Mathf.Floor(value * 100) * 2}%"; };

            var nowPlayingMenu = subMenuCMM.AddSubMenu("Now Playing", "Tweak Now Playing Options Here", true);

            var showNowPlayingOption = nowPlayingMenu.AddBool("Show Now Playing");
            showNowPlayingOption.GetValue += delegate { return Config.ShowNowPlaying; };
            showNowPlayingOption.SetValue += delegate (bool value) { Config.ShowNowPlaying = value; };
            showNowPlayingOption.EnabledText = "Yes";
            showNowPlayingOption.DisabledText = "No";

            var NowPlayingLocationOption = nowPlayingMenu.AddList("Location", Enumerable.Range(0, 3).Select(x => (float) x).ToArray());
            NowPlayingLocationOption.GetValue += delegate { return (float) Config.NowPlayingLocation; };
            NowPlayingLocationOption.SetValue += delegate (float value) { Config.NowPlayingLocation = (Config.Location) value; };
            NowPlayingLocationOption.FormatValue += delegate (float value) { return locationNames[(int) value]; };

            float[] presetColors = new float[NowPlaying.colors.Count];
            for (int i = 0; i < NowPlaying.colors.Count; i++) presetColors[i] = i;
            var NowPlayingColorOption = nowPlayingMenu.AddList("Text Color", presetColors);
            NowPlayingColorOption.GetValue += delegate { return Config.NowPlayingColor; };
            NowPlayingColorOption.SetValue += delegate (float value) { Config.NowPlayingColor = (int) value; };
            NowPlayingColorOption.FormatValue += delegate (float value) { return $"<color=#{ColorUtility.ToHtmlStringRGB(NowPlaying.colors[(int) value].Item1)}>{NowPlaying.colors[(int) value].Item2}"; };
        }

        static readonly List<String> locationNames = new List<string> { "Left Panel", "Center Panel", "Right Panel" };
    }
}
