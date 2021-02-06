using CustomMenuMusic.Installer;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace CustomMenuMusic
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public string Name => "Custom Menu Music";
        public string ID => "CustomMenuMusic";
        public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        [Init]
        public void Init(IPA.Logging.Logger log, Config conf, Zenjector zenjector)
        {
            Util.Logger.logger = log;
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Util.Logger.logger.Debug("Config loaded");
            zenjector.OnMenu<CMMMenuInstaller>();
        }

        [OnStart]
        public void OnStart()
        {

        }

        [OnEnable]
        public void OnEnable()
        {

        }

        [OnExit]
        public void OnApplicationQuit()
        {

        }
    }
}
