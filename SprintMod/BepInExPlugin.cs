using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Mdl.Avatar;
using Mdl.Navigation;
using System.Reflection;
using UnityEngine;

namespace SprintMod
{
    [BepInPlugin("aedenthorn.SprintMod", "SprintMod", "0.3.0")]
    public class BepInExPlugin : BasePlugin
    {
        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<bool> isDebug;
        public static ConfigEntry<int> nexusID;
        public static ConfigEntry<float> multiplier;
        public static ConfigEntry<KeyCode> modKey;

        public static BepInExPlugin context;
        public static bool done;
        public static void Dbgl(object str)
        {
            if (isDebug.Value && str != null)
                context.Log.LogInfo(str);
        }

        public override void Load()
        {
            context = this;
            modEnabled = Config.Bind("General", "Enabled", true, "Enable this mod");
            isDebug = Config.Bind<bool>("General", "IsDebug", true, "Enable debug logs");
            multiplier = Config.Bind<float>("General", "Multiplier", 2f, "Multiplier");
            modKey = Config.Bind<KeyCode>("General", "ModKey", KeyCode.LeftAlt, "Hold down this key to multiply");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);

            AddComponent<MyUpdater>();
            Dbgl("mod loaded");
        }
    }
}
