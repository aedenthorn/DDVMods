using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Mdl.Avatar;
using Mdl.Navigation;
using Meta;
using System.Reflection;
using UnityEngine;

namespace ApplyBuff
{
    [BepInPlugin("aedenthorn.ApplyBuff", "ApplyBuff", "0.1.0")]
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

        public static bool pressedKey = false;

        public override void Load()
        {
            context = this;
            modEnabled = Config.Bind("General", "Enabled", true, "Enable this mod");
            isDebug = Config.Bind<bool>("General", "IsDebug", true, "Enable debug logs");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }
        [HarmonyPatch(typeof(PlayerNavigation), nameof(PlayerNavigation.Update))]
        static class PlayerNavigation_Update_Patch
        {
            static void Postfix(PlayerNavigation __instance)
            {
                if (!modEnabled.Value)
                    return;
                if (AedenthornUtils.CheckKeyDown("f5"))
                {
                    Dbgl("Pressed hotkey");
                    GameObject.FindObjectOfType<PlayerAvatar>();
                }
            }
        }
    }
}
