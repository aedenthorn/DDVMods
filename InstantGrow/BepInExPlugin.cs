using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using Definitions;
using Definitions.Conditions;
using Definitions.Items;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Threading.Tasks;
using Mdl;
using Mdl.Activities;
using Mdl.InputSystem;
using Mdl.Navigation;
using Mdl.Online;
using Meta;
using Meta.Online;
using System;
using System.Reflection;
using UnityEngine;
using static Mdl.InputSystem.RewiredInputProvider;

namespace InstantGrow
{
    [BepInPlugin("aedenthorn.InstantGrow", "InstantGrow", "0.1.0")]
    public class BepInExPlugin : BasePlugin
    {
        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<bool> isDebug;
        public static ConfigEntry<KeyCode> modKey;
        public static ConfigEntry<int> pickupMultiplier;

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
            modKey = Config.Bind<KeyCode>("General", "ModKey", KeyCode.Delete, "Nuke key");

            AddComponent<MyUpdater>();
            Dbgl("mod loaded");

        }
        [HarmonyPatch(typeof(GardeningSlot), nameof(GardeningSlot.GetRemainingTime))]
        static class GardeningSlot_GetRemainingTime_Patch
        {
            static bool Prefix(ref TimeSpan __result)
            {
                if (!modEnabled.Value)
                    return true;
                __result = new TimeSpan(0);
                return false;
            }
        }
        [HarmonyPatch(typeof(GardeningUtil), nameof(GardeningUtil.CanHarvest))]
        static class GardeningUtil_CanHarvest_Patch
        {
            static bool Prefix(ref bool __result)
            {
                if (!modEnabled.Value)
                    return true;
                __result = true;
                return false;
            }
        }
    }
}
