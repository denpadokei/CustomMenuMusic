using CustomMenuMusic.Views;

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
