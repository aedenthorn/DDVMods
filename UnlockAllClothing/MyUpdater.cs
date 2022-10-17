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

namespace UnlockAllClothing
{
    public class MyUpdater : MonoBehaviour
    {
        Client client;
        public void Update()
        {
            if(client is null)
            {
                client = FindObjectOfType<Client>();
            }
            if (client is null || client.Profile is null || !BepInExPlugin.modEnabled.Value || !Input.GetKeyDown(BepInExPlugin.modKey.Value))
                return;

            var all = new List<IItemData>(ItemDatabase.Instance.GetAllByType(ItemType.Clothing));
            BepInExPlugin.Dbgl($"Acquiring all {all.Count} clothing");
            foreach (var item in all)
            {
                client.Profile.Player.Wardrobe.AddItem(item.Item, 1, client.Profile.HangoutState.dispatcher);
                //BepInExPlugin.Dbgl($"adding {item.Name}");
            }
        }
    }
}