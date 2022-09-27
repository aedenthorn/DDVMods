using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using Definitions.Items;
using HarmonyLib;
using Il2CppSystem;
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
                context.Log.LogInfo(str);
        }

        public static bool pressedKey = false;

        public override void Load()
        {
            context = this;
            modEnabled = Config.Bind("General", "Enabled", true, "Enable this mod");
            isDebug = Config.Bind<bool>("General", "IsDebug", true, "Enable debug logs");

            //Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            Dbgl("mod loaded");


            // [Info   : ApplyBuff] buff type: AvatarMovementSpeedMultiplier, end Wednesday, January 1, 3000s, origin Default, persistence Transient, %1150
        }
        [HarmonyPatch(typeof(ProfilePlayer), nameof(ProfilePlayer.AddBuff), new System.Type[] { typeof(BuffOrigin), typeof(BuffType), typeof(BuffPersistence), typeof(int), typeof(DateTime) , typeof(IPlayerEventDispatcher) })]
        static class ProfilePlayer_AddBuff_Patch1
        {
            static bool Prefix(ProfilePlayer __instance, BuffOrigin buffOrigin, BuffType buffType, BuffPersistence persistence, int percentageEffect, DateTime endTime)
            {
                if (!modEnabled.Value)
                    return true;
                Dbgl($"buff type: {buffType}, end {endTime.ToLongDateString()}, origin {buffOrigin}, persistence {persistence}, %{percentageEffect}");
                return true;
            }
        }
        [HarmonyPatch(typeof(ProfilePlayer), nameof(ProfilePlayer.AddBuff), new System.Type[] { typeof(BuffOrigin), typeof(BuffType), typeof(BuffPersistence), typeof(float), typeof(DateTime) , typeof(IPlayerEventDispatcher) })]
        static class ProfilePlayer_AddBuff_Patch2
        {
            static bool Prefix(ProfilePlayer __instance, BuffOrigin buffOrigin, BuffType buffType, BuffPersistence persistence, float percentageEffect, DateTime endTime)
            {
                if (!modEnabled.Value)
                    return true;
                Dbgl($"buff type: {buffType}, end {endTime.ToLongDateString()}, origin {buffOrigin}, persistence {persistence}, %{percentageEffect}");
                return true;
            }
        }
        [HarmonyPatch(typeof(ProfilePlayer), nameof(ProfilePlayer.ApplyBuff), new System.Type[] { typeof(BuffType), typeof(TimeSpan), typeof(bool), typeof(int) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal })]
        static class ProfilePlayer_ApplyBuff_Patch
        {
            static bool Prefix(ProfilePlayer __instance, ref BuffType buffType, ref TimeSpan value, bool isReduction, int extraBuff)
            {
                if (!modEnabled.Value)
                    return true;
                Dbgl($"buff type: {buffType}, span {value.TotalSeconds}s, reduction? {isReduction}, extra {extraBuff}");
                return true;
            }
        }
        [HarmonyPatch(typeof(ProfilePlayer), nameof(ProfilePlayer.ApplyBuff), new System.Type[] { typeof(BuffType), typeof(int), typeof(bool), typeof(int) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal })]
        static class ProfilePlayer_ApplyBuff_Patch2
        {
            static bool Prefix(ProfilePlayer __instance, ref BuffType buffType, ref int value, bool isReduction, int extraBuff)
            {
                if (!modEnabled.Value)
                    return true;
                Dbgl($"buff type: {buffType}, span {value}s, reduction? {isReduction}, extra {extraBuff}");

                return true;
            }
        }
    }
}
