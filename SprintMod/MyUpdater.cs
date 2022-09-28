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
        private InputAction action;
        public MyUpdater()
        {
            //action = new InputAction(new InputTriggerDown(), new UnityInputProvider());
        }
        public void Update()
        {
            if (!BepInExPlugin.modEnabled.Value)
                return;

            var a = FindObjectOfType<PlayerAvatar>();
            if (a is null)
                return;
            try
            {
                //foreach (var c in a.GetComponents<Component>())
                //{
                //    BepInExPlugin.Dbgl($"component {c.ToString()}");
                //}

                //a.transform.position += a.GetComponent<Animator>().velocity * 5;
                var animator = a.GetComponent<Animator>();
                if (animator == null)
                    return;

                var currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
                if (currentClipInfo == null)
                    return;

                var clipName = currentClipInfo[0].clip.name;
                if (clipName == null)
                    return;
                if (clipName.StartsWith("locomotion_run"))
                {
                    a.RunSpeedMultiplier = BepInExPlugin.multiplier.Value;
                    animator.speed = 1 / BepInExPlugin.multiplier.Value;
                }
                else
                {
                    a.RunSpeedMultiplier = 1;
                    animator.speed = 1;
                }
            }
            catch { }
            //done = true;
        }
    }
}