using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using Gameloft.Common;
using HarmonyLib;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;
using Mdl.Navigation;
using Mdl.Avatar;
using Meta;
using System.Threading.Tasks;
using Definitions.Items;

namespace CustomTextures
{
    [BepInPlugin("aedenthorn.CustomTextures", "CustomTextures", "0.1.0")]
    public class BepInExPlugin : BasePlugin
    {
        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<bool> isDebug;
        public static ConfigEntry<KeyCode> modKey;

        public static Dictionary<int, List<int>> avatarSMRs = new Dictionary<int, List<int>>();


        public static Dictionary<string, string> customTextures = new Dictionary<string, string>();
        public static Dictionary<string, System.DateTime> fileWriteTimes = new Dictionary<string, System.DateTime>();
        public static List<string> texturesToLoad = new List<string>();
        public static List<string> layersToLoad = new List<string>();
        public static Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();

        public static List<string> outputDump = new List<string>();
        public static bool dumpOutput;

        public static BepInExPlugin context;
        public static void Dbgl(string str = "")
        {
            if (isDebug.Value)
                context.Log.LogInfo(str);
        }

        public static bool pressedKey = false;

        public override void Load()
        {
            context = this;
            modEnabled = Config.Bind("General", "Enabled", true, "Enable this mod");
            isDebug = Config.Bind<bool>("General", "IsDebug", true, "Enable debug logs");
            modKey = Config.Bind<KeyCode>("General", "ModKey", KeyCode.Equals, "Press this key to reload textures");
                        
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            

            LoadTexturesFromDisk();


            AddComponent<MyUpdater>();
            Dbgl("mod loaded");

        }
        [HarmonyPatch(typeof(PlayerNavigation), nameof(PlayerNavigation.OnCurrentSceneChanged))]
        static class PlayerNavigation_OnCurrentSceneChanged_Patch
        {
            static void Postfix()
            {
                if (!modEnabled.Value)
                    return;
                Dbgl("Scene changed");
                ReloadTextures();
            }
        }
        [HarmonyPatch(typeof(AvatarAppearance), nameof(AvatarAppearance.Update))]
        static class AvatarAppearance_Update_Patch
        {
            static void Postfix(AvatarAppearance __instance)
            {
                if (!modEnabled.Value)
                    return;
                var smrs = __instance.GetComponentsInChildren<SkinnedMeshRenderer>();
                if (!avatarSMRs.TryGetValue(__instance.GetInstanceID(), out List<int> list))
                {
                    avatarSMRs[__instance.GetInstanceID()] = list = new List<int>();
                    foreach (SkinnedMeshRenderer s in smrs)
                    {
                        foreach (Material m in s.materials)
                        {
                            list.Add(m.GetInstanceID());
                        }
                    }
                    return;
                }
                foreach (var smr in smrs)
                {
                    foreach (Material m in smr.materials)
                    {
                        if (!list.Contains(m.GetInstanceID()))
                        {
                            Dbgl("AvatarAppearance changed");
                            list.Clear();
                            foreach (var r in smrs)
                            {
                                foreach (Material m2 in r.materials)
                                {
                                    list.Add(m2.GetInstanceID());
                                    try
                                    {
                                        ReplaceMaterialTextures(m2, "SkinnedMeshRenderer", r.name);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            return;
                        }
                    }

                }
            }

            private static async void AsyncTextureChange(AvatarAppearance instance)
            {
                await Task.Delay(1000);
                foreach (var r in instance.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    if (dumpOutput)
                        outputDump.Add($"\tSkinnedMeshRenderer name: {r.name}");
                    foreach (Material m in r.materials)
                    {
                        try
                        {
                            if (dumpOutput)
                                outputDump.Add($"\t\t{m.name}:");

                            ReplaceMaterialTextures(m, "MeshRenderer", r.name);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }
        public static void LoadTexturesFromDisk()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "CustomTextures");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return;
            }


            texturesToLoad.Clear();

            foreach (string file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
            {
                string fileName = Path.GetFileName(file);
                string id = Path.GetFileNameWithoutExtension(fileName);


                if (!fileWriteTimes.ContainsKey(id) || (cachedTextures.ContainsKey(id) && !DateTime.Equals(File.GetLastWriteTimeUtc(file), fileWriteTimes[id])))
                {
                    cachedTextures.Remove(id);
                    texturesToLoad.Add(id);
                    fileWriteTimes[id] = File.GetLastWriteTimeUtc(file);
                    Dbgl($"adding new {fileName} custom texture: {id}.");
                }

                customTextures[id] = file;
            }
        }

        public static void ReloadTextures()
        {
            outputDump.Clear();
            foreach (var r in Object.FindObjectsOfType<SkinnedMeshRenderer>())
            {
                if (dumpOutput)
                    outputDump.Add($"\tSkinnedMeshRenderer name: {r.name}");
                foreach (Material m in r.materials)
                {
                    try
                    {
                        if (dumpOutput)
                            outputDump.Add($"\t\t{m.name}:");

                        ReplaceMaterialTextures(m, "MeshRenderer", r.name);
                    }
                    catch
                    {
                    }
                }
            }
            foreach (var r in Object.FindObjectsOfType<MeshRenderer>())
            {
                if (dumpOutput)
                    outputDump.Add($"\tSkinnedMeshRenderer name: {r.name}");
                foreach (Material m in r.materials)
                {
                    try
                    {
                        if (dumpOutput)
                            outputDump.Add($"\t\t{m.name}:");

                        ReplaceMaterialTextures(m, "MeshRenderer", r.name);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public static void ReplaceMaterialTextures(Material m, string rendererType, string rendererName)
        {
            if (dumpOutput)
                outputDump.Add("\t\t\tproperties:");

            foreach (string property in m.GetTexturePropertyNames())
            {
                if (dumpOutput)
                    outputDump.Add($"\t\t\t\t{property} {m.GetTexture(property)?.name}");

                int propHash = Shader.PropertyToID(property);

                string tName = m.GetTexture(propHash)?.name;

                CheckSetMatTextures(m, rendererType, rendererName, tName, property);

            }
        }
        public static bool ShouldLoadCustomTexture(string id)
        {
            return texturesToLoad.Contains(id) || layersToLoad.Contains(id);
        }

        public static void CheckSetMatTextures(Material m, string rendererType, string rendererName, string tName, string property)
        {
            foreach (string str in MakePrefixStrings(rendererName, m.name, tName))
            {
                int propHash = Shader.PropertyToID(property);
                if (m.HasProperty(propHash))
                {
                    Texture vanilla = m.GetTexture(propHash);

                    Texture2D result = null;

                    bool isBump = property.Contains("Bump") || property.Contains("Normal");


                    if (ShouldLoadCustomTexture(str + property))
                        result = LoadTexture(str + property, vanilla, isBump);
                    else if (property == "_MainTex" && ShouldLoadCustomTexture(str + "_texture"))
                        result = LoadTexture(str + "_texture", vanilla, isBump);
                    else if (property == "_BumpMap" && ShouldLoadCustomTexture(str + "_bump"))
                        result = LoadTexture(str + "_bump", vanilla, isBump);
                    else if (property == "_MapsTex" && ShouldLoadCustomTexture(str + "_maps"))
                        result = LoadTexture(str + "_maps", vanilla, isBump);
                    else if (property == "_DecalTex" && ShouldLoadCustomTexture(str + "_decal"))
                        result = LoadTexture(str + "_decal", vanilla, isBump);

                    if (result == null)
                        continue;

                    result.name = tName;

                    m.SetTexture(propHash, result);
                    if (result != null && property == "_MainTex")
                        m.SetColor(propHash, Color.white);
                    break;
                }
            }
        }
        public static string[] MakePrefixStrings(string rendererName, string matName, string name)
        {
            return new string[]
            {
                "mesh_"+rendererName,
                "renderer_"+rendererName,
                "mat_"+matName,
                "texture_"+name
            };
        }

        public static Texture2D LoadTexture(string id, Texture vanilla, bool isBump, bool point = false, bool needCustom = false, bool isSprite = false)
        {
            BepInExPlugin.Dbgl($"Trying to load texture for {id}.");
            Texture2D texture;
            if (cachedTextures.ContainsKey(id))
            {
                texture = cachedTextures[id];
                if (customTextures.ContainsKey(id))
                {
                    if (customTextures[id].Contains("bilinear"))
                    {
                        texture.filterMode = FilterMode.Bilinear;
                    }
                    else if (customTextures[id].Contains("trilinear"))
                    {
                        texture.filterMode = FilterMode.Trilinear;
                    }
                    else if (customTextures[id].Contains($"{Path.DirectorySeparatorChar}point{Path.DirectorySeparatorChar}"))
                    {
                        texture.filterMode = FilterMode.Point;
                    }
                }
                return texture;
            }

            if (!customTextures.ContainsKey(id))
            {
                if (needCustom)
                    return null;
                return (Texture2D)vanilla;
            }


            if (vanilla == null)
            {
                texture = new Texture2D(2, 2, TextureFormat.RGBA32, true, isBump);

            }
            else
                texture = new Texture2D(vanilla.width, vanilla.height, TextureFormat.RGBA32, true, isBump);

            if (customTextures.ContainsKey(id))
            {
                if (customTextures[id].Contains($"{Path.DirectorySeparatorChar}bilinear{Path.DirectorySeparatorChar}"))
                {
                    texture.filterMode = FilterMode.Bilinear;
                }
                else if (customTextures[id].Contains($"{Path.DirectorySeparatorChar}trilinear{Path.DirectorySeparatorChar}"))
                {
                    texture.filterMode = FilterMode.Trilinear;
                }
                else if (customTextures[id].Contains($"{Path.DirectorySeparatorChar}point{Path.DirectorySeparatorChar}"))
                {
                    texture.filterMode = FilterMode.Trilinear;
                }
                else if (point)
                    texture.filterMode = FilterMode.Point;
            }
            else if (point)
                texture.filterMode = FilterMode.Point;

            if (customTextures.ContainsKey(id))
            {
                byte[] imageData = File.ReadAllBytes(customTextures[id]);
                texture.LoadImage(imageData);
            }
            else if (vanilla != null)
            {

                // https://support.unity.com/hc/en-us/articles/206486626-How-can-I-get-pixels-from-unreadable-textures-

                // Create a temporary RenderTexture of the same size as the texture
                RenderTexture tmp = RenderTexture.GetTemporary(
                                    texture.width,
                                    texture.height,
                                    0,
                                    RenderTextureFormat.Default,
                                    isBump ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.Default);

                // Blit the pixels on texture to the RenderTexture
                Graphics.Blit(vanilla, tmp);

                // Backup the currently set RenderTexture
                RenderTexture previous = RenderTexture.active;

                // Set the current RenderTexture to the temporary one we created
                RenderTexture.active = tmp;

                // Create a new readable Texture2D to copy the pixels to it
                Texture2D myTexture2D = new Texture2D(vanilla.width, vanilla.height, TextureFormat.RGBA32, true, isBump);

                // Copy the pixels from the RenderTexture to the new Texture
                myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
                myTexture2D.Apply();

                // Reset the active RenderTexture
                RenderTexture.active = previous;

                // Release the temporary RenderTexture
                RenderTexture.ReleaseTemporary(tmp);

                // "myTexture2D" now has the same pixels from "texture" and it's readable.

                texture.SetPixels(myTexture2D.GetPixels());
                texture.Apply();
            }

            cachedTextures[id] = texture;
            return texture;
        }

    }
}
