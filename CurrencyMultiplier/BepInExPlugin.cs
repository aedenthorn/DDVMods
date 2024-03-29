﻿using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using Definitions.Items;
using HarmonyLib;
using Meta;
using System.Reflection;
using UnityEngine;

namespace CurrencyMultiplier
{
    [BepInPlugin("aedenthorn.CurrencyMultiplier", "CurrencyMultiplier", "0.2.1")]
    public class BepInExPlugin : BasePlugin
    {
        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<bool> isDebug;
        public static ConfigEntry<bool> preventLoss;
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
            multiplier = Config.Bind<float>("General", "Multiplier", 10f, "Multiply currency by this amount");
            preventLoss = Config.Bind<bool>("General", "PreventLoss", false, "Prevent loss of currency");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            Dbgl("mod loaded");
        }
        [HarmonyPatch(typeof(ProfilePlayer), nameof(ProfilePlayer.AddCurrency))]
        static class ProfilePlayer_AddCurrency_Patch
        {
            static void Prefix(Item currency, ref int amount)
            {
                if (!modEnabled.Value)
                    return;
                Dbgl($"Adding {amount} {currency.ItemID}, {currency.Index}");
                if (amount < 0)
                {
                    if (preventLoss.Value)
                    {
                        Dbgl("resetting loss to 0");
                        amount = 0;
                    }
                    return;
                }
                if (currency.ItemID == 80100000)
                {
                    Dbgl("Not modifying moonstone amounts");
                    return;
                }
                amount = Mathf.RoundToInt(amount * multiplier.Value);
            }
        }
    }
}
