using Definitions.Items;
using Il2CppSystem;
using Mdl.Avatar;
using Mdl.InputSystem;
using Mdl.Navigation;
using Meta;
using UnityEngine;

namespace SprintMod
{
    public class MyUpdater : MonoBehaviour
    {
        private InputAction action;

        public void Update()
        {
            if (!BepInExPlugin.modEnabled.Value)
                return;
            if (AedenthornUtils.CheckKeyDown("x"))
            {
                BepInExPlugin.Dbgl("Pressed hotkey");
            }
            var a = FindObjectOfType<PlayerAvatar>();
            var n = FindObjectOfType<PlayerNavigation>();
            if (a is null && n is null)
                return;
            try
            {
                Mdl.Systems.SystemRoot.Instance.Client.Profile.Player.AddBuff(BuffOrigin.Default, BuffType.AvatarMovementSpeedMultiplier, BuffPersistence.Persisted, 100000, new DateTime(long.MaxValue), null);

            }
            catch { }
            //a.RunSpeedMultiplier = BepInExPlugin.multiplier.Value;
        }
    }
}