using CustomMenuMusic.Util;
using CustomMenuMusic.Views;
using UnityEngine;
using Zenject;

namespace CustomMenuMusic.Installer
{
    public class CMMMenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            _ = this.Container.BindInterfacesAndSelfTo<SongListUtility>().AsCached();
            _ = this.Container.BindInterfacesAndSelfTo<CustomMenuMusic>().FromNewComponentOn(new GameObject(nameof(CustomMenuMusic))).AsCached().NonLazy();
            _ = this.Container.BindInterfacesAndSelfTo<NowPlaying>().FromNewComponentOn(new GameObject(nameof(NowPlaying))).AsCached();
            _ = this.Container.BindInterfacesAndSelfTo<CMMTabViewController>().FromNewComponentAsViewController().AsSingle().NonLazy();
            _ = this.Container.BindInterfacesAndSelfTo<ConfigViewController>().FromNewComponentAsViewController().AsSingle().NonLazy();
        }
    }
}
