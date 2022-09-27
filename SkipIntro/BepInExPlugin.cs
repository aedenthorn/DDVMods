using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using Definitions.Items;
using HarmonyLib;
using Il2CppSystem;
using Mdl;
using Mdl.Avatar;
using Mdl.Navigation;
using Meta;
using System.Reflection;
using UnityEngine;

namespace SkipIntro
{
    [BepInPlugin("aedenthorn.SkipIntro", "SkipIntro", "0.1.0")]
    public class BepInExPlugin : BasePlugin
    {
        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<bool> isDebug;

        public static BepInExPlugin context;
        public static void Dbgl(string str = "", bool pref = true)
        {
            if (isDebug.Value)
                Debug.Log((pref ? typeof(BepInExPlugin).Namespace + " " : "") + str);
        }

        public override void Load()
        {
            context = this;
            modEnabled = Config.Bind("General", "Enabled", true, "Enable this mod");
            isDebug = Config.Bind<bool>("General", "IsDebug", true, "Enable debug logs");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            Dbgl("mod loaded");
        }

        [HarmonyPatch(typeof(Startup), nameof(Startup.DoFadeIn))]
        static class Startup_DoFadeIn_Patch
        {
            static void Prefix(ref bool skipIntroSequence)
            {
                if (!modEnabled.Value)
                    return;
                Dbgl("skipping intro");
                skipIntroSequence = true;
            }
        }
    }
}
