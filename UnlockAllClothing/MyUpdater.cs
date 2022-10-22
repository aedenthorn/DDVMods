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

namespace UnlockAllClothing
{
    public class MyUpdater : MonoBehaviour
    {
        Mdl.Online.Client client;
        public void Update()
        {
            if (client is null)
            {
                client = FindObjectOfType<Mdl.Online.Client>();
            }
            if (client is null || client.Profile is null || !BepInExPlugin.modEnabled.Value || !Input.GetKeyDown(BepInExPlugin.modKey.Value))
                return;

            var all = new List<IItemData>(ItemDatabase.Instance.GetAllByType(ItemType.Clothing));
            BepInExPlugin.Dbgl($"Acquiring all {all.Count} clothing");
            foreach (var item in all)
            {
                try
                {
                    ItemReward ir = ItemReward.New(item.Item, 1);

                    PlayerProfile profile = client.MetaClient.NetworkData.GetPlayer(client.Profile, client.Profile.HangoutState.dispatcher);
                    client.Profile.Definitions_Rewards_IRewardRecipient_AddItem(item.Item, 1, profile.Dispatcher, client.Profile.HangoutState.context, new Nullable<Item>(), ir.State, true);
                    //client.Profile.AddItem(item.Item, 1, client.MetaClient.NetworkData.GetPlayer(client.Profile, client.Profile.HangoutState.dispatcher).Dispatcher, client.MetaClient.NetworkData.GetWorld(client.Profile.HangoutState.dispatcher, client.Profile).Dispatcher, client.Profile.HangoutState.context);
                    //BepInExPlugin.Dbgl($"adding {item.Name}");
                }
                catch (System.Exception ex)
                {
                    BepInExPlugin.Dbgl($"Error getting {item.Name}: \n\n{ex.StackTrace}");
                }
            }
        }
    }
}