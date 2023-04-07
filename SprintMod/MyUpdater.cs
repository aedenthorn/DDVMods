using Definitions.Items;
using Il2CppSystem;
using System.Collections.Generic;
using Mdl.Avatar;
using Mdl.Navigation;
using Meta;
using UnityEngine;
using Mdl.InputSystem;

namespace SprintMod
{
    public class MyUpdater : MonoBehaviour
    {
        private PlayerAvatar avatar;
        private Dictionary<int, float[]> defaultSpeeds = new Dictionary<int, float[]>();
        public MyUpdater()
        {
            //action = new InputAction(new InputTriggerDown(), new UnityInputProvider());
        }
        public void Update()
        {
            if (!BepInExPlugin.modEnabled.Value)
                return;

            if (avatar is null)
            {
                avatar = FindObjectOfType<PlayerAvatar>();
            }
            if (avatar is null)
                return;
            try
            {
                //foreach (var c in a.GetComponents<Component>())
                //{
                //    BepInExPlugin.Dbgl($"component {c.ToString()}");
                //}

                //a.transform.position += a.GetComponent<Animator>().velocity * 5;
                var animator = avatar.GetComponent<Animator>();
                if (animator == null)
                    return;

                var currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
                if (currentClipInfo == null)
                    return;

                var clipName = currentClipInfo[0].clip.name;
                if (clipName == null)
                    return;
                if (clipName.StartsWith("locomotion_run") && Input.GetKey(BepInExPlugin.modKey.Value))
                {
                    avatar.moveSpeedMultiplier = BepInExPlugin.multiplier.Value;
                    avatar.RunSpeedMultiplier = BepInExPlugin.multiplier.Value;
                    animator.speed = 1 / BepInExPlugin.multiplier.Value;
                }
                else
                {
                    if(defaultSpeeds.TryGetValue(avatar.GetInstanceID(), out float[] speeds))
                    {
                        avatar.RunSpeedMultiplier = speeds[0];
                        avatar.moveSpeedMultiplier = speeds[1];
                        animator.speed = speeds[2];
                    }
                    else
                    {
                        defaultSpeeds[avatar.GetInstanceID()] = new float[] { avatar.RunSpeedMultiplier, avatar.moveSpeedMultiplier, animator.speed };
                    }
                }
            }
            catch { }
            //done = true;
        }
    }
}