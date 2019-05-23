# CustomMenuMusic

## Custom Menu Music v1.5.0
Changes the music in the menu! You can use either a randomly selected song from your CustomSongs folder, or add your own music to the `BeatSaber\CustomMenuSongs` folder.

### Configuration
By default it reads the `BeatSaber\CustomMenuSongs` folder. If this folder does not exist, it will be created for you.

To use songs from the `BeatSaber\CustomSongs` folder, simply go into `Beat Saber\UserData\CustomMenuMusic.ini` and change `UseCustomMenuSongs` to `False`.

To disable the Now Playing feature, set `ShowNowPlaying` to `False`.

All songs files **HAVE** to be `.ogg` format *(I tried to support MP3s, unfortunately it's not possible)*. The Now Playing feature will pull the name from `info.json` if available *(i.e. a CustomSongs song)*, otherwise it will use the filename without the extension.

Thanks to **andruzzzhka** and **Lunikc** for their base code.

#### Changelog :
- **v1.5.0** : *Refactored some code. Added UI to show the currently playing song.*
- **v1.4.0** : *Updated to 1.0.0 and BSIPA. Removed Despacito.*
- **v1.3.0** : *Updated to 0.13.2. If user has no songs in their CustomMenuSongs folder, Despacito is added*
- **v1.1.2** : *Now default setting is CustomMenuSongs and reads CustomSongs if empty.*
- **v1.1.1** : *Fixed issue with map loading.*
- **v1.1.0** : *Changed the folder from UserData to CustomMenuSongs (Thanks to BeigeAnimal for the suggestion)*
- **v1.0.0** : *You can now play multiple menu songs*
