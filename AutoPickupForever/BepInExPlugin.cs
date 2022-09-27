using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Mdl.Activities;
using Mdl.Grid;
using Mdl.Navigation;
using Meta.Online;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AutoPickupForever
{
    [BepInPlugin("aedenthorn.AutoPickupForever", "AutoPickupForever", "0.1.0")]
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

        public override void Load()
        {
            context = this;
            modEnabled = Config.Bind("General", "Enabled", true, "Enable this mod");
            isDebug = Config.Bind<bool>("General", "IsDebug", true, "Enable debug logs");

            //Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);

            //AddComponent<MyUpdater>();

            Dbgl("mod loaded");
        }

        [HarmonyPatch(typeof(Client), nameof(Client.Update))]
        static class Client_Update_Patch
        {
            static void Postfix(Client __instance)
            {
                if (!modEnabled.Value)
                    return;
                var a = Object.FindObjectsOfType<AutomaticPickup>();
                if (a is null || a.Length == 0)
                    return;
                try
                {
                    foreach (var c in a)
                    {
                        var g = c.gameObject.GetComponent<PlayerTaskCollider>();
                        if (g is null || g.TaskDefinition.Action is null)
                            continue;
                        var action = g.TaskDefinition.Action.GetComponent<PlayerAction>();
                        Dbgl($"action: {action.ToString()}");
                        var mi = AccessTools.Method("Mdl.Navigation.PickUpAction:PickUpItemWithGizmo");
                        if(mi != null)
                        {
                            Dbgl($"picking up object {c.name}");
                            mi.Invoke(action, new object[] { c.gameObject.GetComponent<GridObjectScript>() });
                        }
                    }
                }
                catch { }
            }
        }
    }
}
