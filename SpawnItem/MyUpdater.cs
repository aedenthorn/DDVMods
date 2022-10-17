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
using Mdl.Ui;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using HarmonyLib;
using System.IO;

namespace SpawnItem
{
    public class MyUpdater : MonoBehaviour
    {
        public Dictionary<ItemType, List<string>> items = new Dictionary<ItemType, List<string>>();
        private bool spawning = true;
        private string nameString = "";
        private string lastNameString = "";
        public List<string> candidates = new List<string>();
        private string amountString;
        private BaseUiRoot root;
        private GUIStyle style;
        private Texture2D boxTex;
        public void Awake()
        {
            boxTex = new Texture2D(1, 1);
            boxTex.SetPixel(0,0, Color.black);
            boxTex.Apply();


            style = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
            style.normal.textColor = Color.white;
        }

        public void Update()
        {

            if(root is null)
            {
                root = FindObjectOfType<BaseUiRoot>();
            }

            if (!BepInExPlugin.modEnabled.Value || root is null)
                return;

            if(items.Count == 0)
            {
                if (ItemDatabase.Instance is null)
                {
                    return;
                }
                foreach (var type in System.Enum.GetValues(typeof(ItemType)))
                {
                    items[(ItemType)type] = new List<string>();
                    var all = new List<IItemData>(ItemDatabase.Instance.GetAllByType((ItemType)type));
                    foreach (var item in all)
                    {
                        items[(ItemType)type].Add(item.Name);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Slash))
            {
                spawning = !spawning;

                BepInExPlugin.Dbgl($"spawner active: {spawning}");
            }
            if (Input.GetKeyDown(KeyCode.Semicolon))
            {
                DumpItems();
            }
        }

        public float winx;
        public float winy;

        public void OnGUI()
        {
            if (BepInExPlugin.modEnabled.Value && spawning && items.ContainsKey(ItemType.ActivityItem))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                GUILayout.BeginVertical(style);
                GUILayout.BeginHorizontal(style);
                GUILayout.BeginVertical(style, new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(50) });
                GUILayout.Label("<b>Name</b>", style, new GUILayoutOption[] { GUILayout.Height(25) });
                nameString = GUILayout.TextField(nameString, new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(25) });
                GUILayout.EndVertical();
                GUILayout.BeginVertical(style, new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(50) });
                GUILayout.Label("<b>Amount</b>", style, new GUILayoutOption[] { GUILayout.Height(25) });
                amountString = GUILayout.TextField(amountString, new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(25) });
                GUILayout.EndVertical();
                GUILayout.BeginVertical(style, new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(50) });
                GUILayout.Label("", style, new GUILayoutOption[] { GUILayout.Height(25) });
                if (GUILayout.Button("Spawn", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(25) }))
                {
                    SpawnItem();
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                if(!string.IsNullOrEmpty(nameString) && nameString != lastNameString)
                {
                    candidates.Clear();

                    foreach (var n in items[ItemType.ActivityItem])
                    {
                        if (n.ToLower().StartsWith(nameString.ToLower()))
                        {
                            candidates.Add(n);
                        }
                    }
                    BepInExPlugin.Dbgl($"new string {nameString}, {candidates.Count}/{items[ItemType.ActivityItem].Count} candidates");
                }
                foreach (var n in candidates)
                {
                    if (GUILayout.Button(n, new GUILayoutOption[] { GUILayout.Width(500), GUILayout.Height(25) }))
                    {
                        nameString = n;
                    }
                }
                GUILayout.EndVertical();
                lastNameString = nameString;
            }
            else
            {
                Cursor.visible = false;
            }
        }

        public void SpawnItem()
        {
            if (string.IsNullOrEmpty(nameString))
            {
                BepInExPlugin.Dbgl($"missing name");
                return;
            }
            if (!int.TryParse(amountString, out int amount))
            {
                BepInExPlugin.Dbgl($"invalid amount {amountString}");
                return;
            }
            var client = FindObjectOfType<Client>();
            if(client is null)
            {
                BepInExPlugin.Dbgl($"no client");
                return;
            }
            foreach (var type in items.Keys)
            {
                var all = new List<IItemData>(ItemDatabase.Instance.GetAllByType(type));
                foreach (var item in all)
                {
                    if(item.Name == nameString)
                    {
                        BepInExPlugin.Dbgl($"Spawning {amount} {nameString} of type {type.ToString()}");
                        client.Profile.Player.Backpack.AddItem(item.Item, amount, client.Profile.HangoutState.dispatcher);
                        return;
                    }
                }
            }
            BepInExPlugin.Dbgl($"{nameString} not found");
        }
        public void DumpItems()
        {
            foreach (var type in items.Keys)
            {
                List<string> list = new List<string>();
                var all = new List<IItemData>(ItemDatabase.Instance.GetAllByType(type));
                foreach (var item in all)
                {
                    list.Add($"{item.Name}:{item.ID}");
                }
                var fn = ((ItemType)type).ToString();
                File.WriteAllLines($"{fn}.txt", list.ToArray());
                BepInExPlugin.Dbgl($"dumped {list.Count} items to {fn}.txt");
            }
        }
    }
}