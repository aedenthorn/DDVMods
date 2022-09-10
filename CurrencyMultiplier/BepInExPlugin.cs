using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Meta;
using System.Reflection;
using UnityEngine;

namespace CurrencyMultiplier
{
    [BepInPlugin("aedenthorn.CurrencyMultiplier", "CurrencyMultiplier", "0.1.0")]
    public class BepInExPlugin : BasePlugin
    {
        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<bool> isDebug;
        public static ConfigEntry<float> multiplier;

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
            multiplier = Config.Bind<float>("General", "Multiplier", 10f, "Multiply currency by this amount");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }
        [HarmonyPatch(typeof(ProfilePlayer), nameof(ProfilePlayer.AddCurrency))]
        static class ProfilePlayer_AddCurrency_Patch
        {
            static void Prefix(ref int amount)
            {
                if (!modEnabled.Value)
                    return;
                amount = Mathf.RoundToInt(amount * multiplier.Value);
            }
        }
    }
}
