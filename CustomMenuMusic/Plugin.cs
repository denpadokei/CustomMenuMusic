using IPA;
using UnityEngine.SceneManagement;

namespace CustomMenuMusic
{
    public class Plugin : IBeatSaberPlugin
    {
        public string Name => "Custom Menu Music";
        public string ID => "CustomMenuMusic";
        public string Version => "1.5.3";

        public void Init(IPA.Logging.Logger log)
        {
            Util.Logger.logger = log;

            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void OnApplicationStart() { }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene) { }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            if (scene.name == "MenuCore")
                Config.instance.CreateSettingsUI();
            CustomMenuMusic.OnLoad();
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void OnUpdate() { }

        public void OnFixedUpdate() { }

        public void OnSceneUnloaded(Scene scene)
        {
            if (scene.name == "MenuCore")
                Config.initialized = false;
        }
    }
}
