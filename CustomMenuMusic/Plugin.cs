using CustomMenuMusic.Installer;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using System;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace CustomMenuMusic
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public static Plugin Instance { get; private set; }

        public string Name => "Custom Menu Music";
        public string ID => "CustomMenuMusic";
        public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private System.Random seedMaker = new System.Random(Environment.TickCount);
        public int Seed => this.seedMaker.Next();

        public const string HarmonyID = "CustomMenuMusic.denpadokei.com.github";
        internal static HarmonyLib.Harmony Harmony;
        [Init]
        public void Init(IPA.Logging.Logger log, Config conf, Zenjector zenjector)
        {
            Instance = this;
            Logger.logger = log;
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Logger.logger.Debug("Config loaded");
            Harmony = new HarmonyLib.Harmony(HarmonyID);
            zenjector.OnMenu<CMMMenuInstaller>();
        }

        [OnStart]
        public void OnStart()
        {
            
        }

        [OnEnable]
        public void OnEnable()
        {
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [OnDisable]
        public void OnDisable()
        {
            Harmony.UnpatchAll(HarmonyID);
        }

        [OnExit]
        public void OnApplicationQuit()
        {

        }
    }
}
