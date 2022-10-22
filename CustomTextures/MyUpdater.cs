using Definitions.Items;
using Il2CppSystem;
using Mdl.Avatar;
using Mdl.Navigation;
using Meta;
using UnityEngine;
using Mdl.InputSystem;
using System.Collections.Generic;
using Gameloft.Common;
using Mdl.Online;
using Meta.Online;
using Definitions.Rewards;
using Definitions.Util;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CustomTextures
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
            if (client != null && client.Profile != null && Input.GetKeyDown(BepInExPlugin.modKey.Value))
            {
                BepInExPlugin.Dbgl("Pressed mod key");

                BepInExPlugin.cachedTextures.Clear();
                BepInExPlugin.fileWriteTimes.Clear();
                BepInExPlugin.customTextures.Clear();
                BepInExPlugin.dumpOutput = true;
                BepInExPlugin.LoadTexturesFromDisk();
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "CustomTextures", "scene_dump.txt");
                BepInExPlugin.ReloadTextures();
                File.WriteAllLines(path, BepInExPlugin.outputDump.ToArray());
                BepInExPlugin.dumpOutput = false;
            }
        }
   }
}