using IPA;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace CustomMenuMusic
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public string Name => "Custom Menu Music";
        public string ID => "CustomMenuMusic";
        public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        [Init]
        public void Init(IPA.Logging.Logger log)
        {
            Util.Logger.logger = log;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        [OnStart]
        public void OnStart() => CustomMenuMusic.OnLoad();

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            if (scene.name == "MenuCore")
                Config.instance.CreateSettingsUI();
        }

        [OnExit]
        public void OnApplicationQuit() => SceneManager.sceneLoaded -= OnSceneLoaded;

        public void OnSceneUnloaded(Scene scene)
        {
            if (scene.name == "MenuCore")
                Config.initialized = false;
        }
    }
}
