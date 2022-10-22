using Definitions.Items;
using Il2CppSystem;
using Mdl.Avatar;
using Mdl.Navigation;
using Meta;
using UnityEngine;
using Mdl.InputSystem;
using Il2CppSystem.Collections.Generic;
using Gameloft.Common;
using Mdl.Online;
using Meta.Online;
using Definitions.Rewards;
using Definitions.Util;
using Mdl.Activities;

namespace InstantGrow
{
    public class MyUpdater : MonoBehaviour
    {
        PlayerNavigation nav;
        public void Update()
        {
            if (!BepInExPlugin.modEnabled.Value)
                return;

            if(nav is null)
            {
                nav = FindObjectOfType<PlayerNavigation>();
            }

            if(nav is null)
            {
                return;
            }
            if (!Input.GetKeyDown(BepInExPlugin.modKey.Value))
                return;

            BepInExPlugin.Dbgl("Pressed delete key");


            foreach (var g in FindObjectsOfType<Garden>())
            {
                for(int i = 0; i < g.gardeningSlots.Count; i++)
                {
                    Destroy(g.gardeningSlots[i].gameObject);
                    g.gardeningSlots[i] = null;
                }
            }
        }
    }
}