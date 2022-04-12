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
            this.Container.BindInterfacesAndSelfTo<SongListUtility>().AsCached();
            this.Container.BindInterfacesAndSelfTo<CustomMenuMusic>().FromNewComponentOn(new GameObject(nameof(CustomMenuMusic))).AsCached().NonLazy();
            this.Container.BindInterfacesAndSelfTo<NowPlaying>().FromNewComponentOn(new GameObject(nameof(NowPlaying))).AsCached();
            this.Container.BindInterfacesAndSelfTo<CMMTabViewController>().FromNewComponentAsViewController().AsSingle().NonLazy();
            this.Container.BindInterfacesAndSelfTo<ConfigViewController>().FromNewComponentAsViewController().AsSingle().NonLazy();
        }
    }
}
