using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using Definitions;
using Definitions.Items;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Mdl;
using Mdl.InputSystem;
using Mdl.Navigation;
using Meta;
using System;
using System.Reflection;
using UnityEngine;
using static Mdl.InputSystem.RewiredInputProvider;

namespace PickupMultiplier
{
    [BepInPlugin("aedenthorn.PickupMultiplier", "PickupMultiplier", "0.2.0")]
    public class BepInExPlugin : BasePlugin
    {
        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<bool> isDebug;
        public static ConfigEntry<bool> reverseToggle;
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
            reverseToggle = Config.Bind<bool>("Options", "ReverseToggle", false, "If true, holding down the mod key turns off the behaviour instead.");
            modKey = Config.Bind<KeyCode>("Options", "ModKey", KeyCode.LeftShift, "Hold down this key to multiply");
            pickupMultiplier = Config.Bind<int>("Options", "PickupMultiplier", 2, "Multiply picked up items by this amount");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            Dbgl("mod loaded");
        }
        [HarmonyPatch(typeof(PickUpAction), nameof(PickUpAction.DisplayPickUpGizmo))]
        static class PickUpAction_DisplayPickUpGizmo_Patch
        {
            static void Prefix(Item item, ref int amount)
            {
                if (!modEnabled.Value || reverseToggle.Value == Input.GetKey(modKey.Value))
                    return;
                Dbgl($"multiplying item {item.GetLocalizedDisplayName(LocalizationManager.LocalizatorInstance)} ({amount}) by {pickupMultiplier.Value} (for display)");
                amount *= pickupMultiplier.Value;
            }
        }
        [HarmonyPatch(typeof(ContainerInventory), nameof(ContainerInventory.AddItem), new Type[] { typeof(Item), typeof( int), typeof(ProfileEventDispatcher), typeof(ItemState), typeof(AddDetail) })]
        static class ContainerInventory_AddItem_Patch_1
        {
            static void Prefix(Item item, ref int amount)
            {
                if (!modEnabled.Value || reverseToggle.Value == Input.GetKey(modKey.Value))
                    return;
                Dbgl($"multiplying item {item.GetLocalizedDisplayName(LocalizationManager.LocalizatorInstance)} ({amount}) by {pickupMultiplier.Value} (#1)");
                amount *= pickupMultiplier.Value;
            }
        }
        [HarmonyPatch(typeof(ContainerInventory), nameof(ContainerInventory.AddItem), new Type[] { typeof(Item), typeof(int), typeof(IPlayerEventDispatcher), typeof(ItemState), typeof(AddDetail), typeof(List<int>) })]
        static class ContainerInventory_AddItem_Patch_2
        {
            static void Prefix(Item item, ref int amount)
            {
                if (!modEnabled.Value || reverseToggle.Value == Input.GetKey(modKey.Value))
                    return;
                Dbgl($"multiplying item {item.GetLocalizedDisplayName(LocalizationManager.LocalizatorInstance)} ({amount}) by {pickupMultiplier.Value} (#2)");
                amount *= pickupMultiplier.Value;
            }
        }
    }
}
