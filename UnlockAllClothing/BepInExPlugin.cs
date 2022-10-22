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
using Mdl.InputSystem;
using Mdl.Navigation;
using Mdl.Online;
using Meta;
using Meta.Online;
using System;
using System.Reflection;
using UnityEngine;
using static Mdl.InputSystem.RewiredInputProvider;

namespace UnlockAllClothing
{
    [BepInPlugin("aedenthorn.UnlockAllClothing", "UnlockAllClothing", "0.1.0")]
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
            modKey = Config.Bind<KeyCode>("General", "ModKey", KeyCode.Home, "Press this key to unlock");
                        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);

            AddComponent<MyUpdater>();
            Dbgl("mod loaded");

        }
    }
}
