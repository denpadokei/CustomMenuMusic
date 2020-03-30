using IPA;
using UnityEngine.SceneManagement;

namespace CustomMenuMusic
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public string Name => "Custom Menu Music";
        public string ID => "CustomMenuMusic";
        public string Version => "1.5.3";

        [Init]
        public void Init(IPA.Logging.Logger log)
        {
            Util.Logger.logger = log;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            if (scene.name == "MenuCore")
                Config.instance.CreateSettingsUI();
            CustomMenuMusic.OnLoad();
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
