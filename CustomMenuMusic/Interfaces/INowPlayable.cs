using CustomMenuMusic.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomMenuMusic.Interfaces
{
    public interface INowPlayable
    {
        void SetCurrentSong(string newSong, bool isPath = true);
        void SetTextColor();
        void SetTextColor(int colorIndex);
        void SetLocation(ConfigViewController.Location location);
    }
}
