using CustomMenuMusic.Util;
using CustomMenuMusic.Views;
using SiraUtil;
using Zenject;

namespace CustomMenuMusic.Installer
{
    public class CMMMenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<SongListUtility>().AsCached();
            this.Container.BindInterfacesAndSelfTo<CustomMenuMusic>().FromNewComponentOnNewGameObject(nameof(CustomMenuMusic)).AsCached().NonLazy();
            this.Container.BindInterfacesAndSelfTo<NowPlaying>().FromNewComponentOnNewGameObject(nameof(NowPlaying)).AsCached();
            this.Container.BindInterfacesAndSelfTo<CMMTabViewController>().FromNewComponentAsViewController().AsCached().NonLazy();
            this.Container.BindInterfacesAndSelfTo<ConfigViewController>().FromNewComponentAsViewController().AsCached().NonLazy();
        }
    }
}
