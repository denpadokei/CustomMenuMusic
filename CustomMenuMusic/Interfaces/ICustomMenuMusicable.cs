using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomMenuMusic.Interfaces
{
    public interface ICustomMenuMusicable
    {
        string CurrentSongPath { get; }
        Task GetSongsListAsync();
        Task GetSongsListAsync(bool useCustomMenuSongs);
    }
}
