using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomMenuMusic
{
    static class Config
    {
        static BS_Utils.Utilities.Config config;

        static readonly string configName = "CustomMenuMusic";
        static readonly string sectionCore = "Core";
        static readonly string sectionNowPlaying = "NowPlaying";

        internal static void Init()
        {
            config = new BS_Utils.Utilities.Config(configName);
        }

        static readonly string useCustomMenuSongs = "UseCustomMenuSongs";
        internal static bool UseCustomMenuSongs
        {
            get
            {
                return config.GetBool(sectionCore, useCustomMenuSongs, false, true);
            }
            set
            {
                config.SetBool(sectionCore, useCustomMenuSongs, value);
                CustomMenuMusic.instance.GetSongsList(value);
            }
        }

        static readonly string loop = "Loop";
        internal static bool Loop
        {
            get
            {
                return config.GetBool(sectionCore, loop, false, true);
            }
            set
            {
                config.SetBool(sectionCore, loop, value);
            }
        }

        static readonly string menuMusicVolume = "MenuMusicVolume";
        internal static float MenuMusicVolume
        {
            get
            {
                return config.GetFloat(sectionCore, menuMusicVolume, 0.5f, true);
            }
            set
            {
                config.SetFloat(sectionCore, menuMusicVolume, value);
            }
        }

        static readonly string showNowPlaying = "ShowNowPlaying";
        internal static bool ShowNowPlaying
        {
            get
            {
                return config.GetBool(sectionNowPlaying, showNowPlaying, true, true);
            }
            set
            {
                config.SetBool(sectionNowPlaying, showNowPlaying, value);
                //NowPlaying.instance.enabled = value;
            }
        }

        static readonly string nowPlayingLocation = "NowPlayingLocation";
        internal static Location NowPlayingLocation
        {
            get
            {
                return (Location) config.GetInt(sectionNowPlaying, nowPlayingLocation, 0, true);
            }
            set
            {
                config.SetInt(sectionNowPlaying, nowPlayingLocation, (int) value);
            }
        }

        static readonly string nowPlayingColor = "NowPlayingColor";
        internal static int NowPlayingColor
        {
            get
            {
                return config.GetInt(sectionNowPlaying, nowPlayingColor, 0, true);
            }
            set
            {
                config.SetInt(sectionNowPlaying, nowPlayingColor, value);
                NowPlaying.instance.SetTextColor(value);
            }
        }

        internal enum Location
        {
            LeftPanel,
            CenterPanel,
            RightPanel
        }
    }
}
