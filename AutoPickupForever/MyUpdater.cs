using Definitions.Items;
using Il2CppSystem;
using Mdl.Activities;
using Mdl.Avatar;
using Mdl.InputSystem;
using Mdl.Navigation;
using Meta;
using UnityEngine;

namespace AutoPickupForever
{
    public class MyUpdater : MonoBehaviour
    {

        public void SUpdate()
        {
            if (!BepInExPlugin.modEnabled.Value)
                return;

            var a = FindObjectsOfType<AutomaticPickup>();
            if (a is null || a .Length == 0)
                return;
            try
            {
                foreach(var c in a)
                {
                    if (c._automaticPickUpData is null)
                        continue;
                    if (AedenthornUtils.CheckKeyDown("x"))
                    {
                        BepInExPlugin.Dbgl($"Pressed hotkey {c._automaticPickUpData.automaticPickupDurationInSeconds}");
                    }
                    c._automaticPickUpData.automaticPickupDurationInSeconds = int.MaxValue;
                }

            }
            catch { }
        }
    }
}