using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using Definitions.Items;
using HarmonyLib;
using Meta;
using System.Reflection;
using UnityEngine;

namespace ExpMultiplier
{
    [BepInPlugin("aedenthorn.FriendshipMultiplier", "FriendshipMultiplier", "0.1.0")]
    public class BepInExPlugin : BasePlugin
    {
        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<bool> isDebug;
        public static ConfigEntry<float> multiplier;

        public static BepInExPlugin context;
        public static void Dbgl(string str = "")
        {
            if (isDebug.Value)
                context.Log.LogInfo(str);
        }

        public static bool pressedKey = false;

        public override void Load()
        {
            context = this;
            modEnabled = Config.Bind("General", "Enabled", true, "Enable this mod");
            isDebug = Config.Bind<bool>("General", "IsDebug", true, "Enable debug logs");
            multiplier = Config.Bind<float>("Options", "Multiplier", 10f, "Multiply friendship gains by this amount");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            Dbgl("mod loaded");
        }
        //[HarmonyPatch(typeof(Character), nameof(Character.AddFriendship))]
        static class Character_AddFriendship_Patch
        {
            static void Prefix(ref int amount)
            {
                if (!modEnabled.Value)
                    return;
                Dbgl($"Adding {amount} friendship");
                amount = Mathf.RoundToInt(amount * multiplier.Value);
            }
        }
    }
}
