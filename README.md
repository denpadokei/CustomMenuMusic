# CustomMenuMusic

## Custom Menu Music v1.3.0
Changes the music in the menu, now you can use either a randomly selected song from all downloaded maps, or use multiple songs selected at random from the `BeatSaber\CustomMenuSongs` folder.

### Configuration
By default it reads the `BeatSaber\CustomMenuSongs` folder. If this folder does not exist, it is created and a default song is added.

To use songs from the `BeatSaber\CustomSongs` folder, simply go into `Beat Saber\modprefs.ini` and change `UseCustomMenuSongs` to `0`. 

All songs files **HAVE** to be `.ogg` format.


Thanks to **andruzzzhka** for his base code.

#### Changelog :
- **v1.3.0** : *Updated to 0.13.2. If user has no songs in their CustomMenuSongs folder, Despacito is added*
- **v1.1.2** : *Now default setting is CustomMenuSongs and reads CustomSongs if empty.*
- **v1.1.1** : *Fixed issue with map loading.*
- **v1.1.0** : *Changed the folder from UserData to CustomMenuSongs (Thanks to BeigeAnimal for the suggestion)*
- **v1.0.0** : *You can now play multiple menu songs*
