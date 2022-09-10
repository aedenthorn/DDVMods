using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using Definitions;
using HarmonyLib;
using Mdl.Avatar;
using System.Reflection;
using UnityEngine;

namespace SprintMod
{
    [BepInPlugin("aedenthorn.SprintMod", "SprintMod", "0.1.0")]
    public class BepInExPlugin : BasePlugin
    {
        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<bool> isDebug;
        public static ConfigEntry<int> nexusID;

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
        }
        [HarmonyPatch(typeof(ManaData), nameof(ManaData.GetManaCost))]
        static class ManaData_GetManaCost_Patch
        {
            static bool Prefix(ref int __result)
            {
                if (!modEnabled.Value)
                    return true;
                __result = 0;
                return false;
            }
        }
        [HarmonyPatch(typeof(PlayerAvatar), nameof(PlayerAvatar.Up))]
        static class ManaData_GetManaCost_Patch
        {
            static bool Prefix(ref int __result)
            {
                if (!modEnabled.Value)
                    return true;
                __result = 0;
                return false;
            }
        }
    }
}
