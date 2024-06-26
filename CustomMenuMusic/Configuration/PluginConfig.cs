﻿using IPA.Config.Stores;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace CustomMenuMusic.Configuration
{
    public class PluginConfig
    {
        public static PluginConfig Instance { get; internal set; }
        public event Action<PluginConfig> OnSettingChanged;

        public virtual bool UseCustomMenuSongs { get; set; }
        public virtual bool Loop { get; set; }
        public virtual bool ShowNowPlaying { get; set; } = true;
        public virtual int NowPlayingLocation { get; set; }
        public virtual int NowPlayingColor { get; set; }
        public virtual bool CustomResultSound { get; set; } = false;
        public virtual bool GrobalActiveSound { get; set; } = true;
        public virtual bool EnableSongPreview { get; set; } = true;
        public virtual bool OverrideSongPrevewLength { get; set; } = false;
        public virtual float MaxSongPreviewLength { get; set; } = 10;
        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload()
        {
            // Do stuff after config is read from disk.
        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            // Do stuff when the config is changed.
            this.OnSettingChanged?.Invoke(this);
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            // This instance's members populated from other
            this.UseCustomMenuSongs = other.UseCustomMenuSongs;
            this.Loop = other.Loop;
            this.ShowNowPlaying = other.ShowNowPlaying;
            this.NowPlayingLocation = other.NowPlayingLocation;
            this.NowPlayingColor = other.NowPlayingColor;
            this.CustomResultSound = other.CustomResultSound;
        }
    }
}
