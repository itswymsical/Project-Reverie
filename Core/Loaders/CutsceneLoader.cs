using ReverieMod.Core.Abstraction.Interfaces;
using ReverieMod.Core.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;
using Terraria.UI;

namespace ReverieMod.Core.Loaders
{
    internal sealed class CutsceneLoader : ILoadableReverie
    {
        public float Priority => 1f;
        public bool LoadOnDedServer => false;

        public static List<Cutscene> Cutscenes = new List<Cutscene>();

        public void Load()
        {
            Assembly modAssembly = Assembly.GetExecutingAssembly();

            foreach (Type t in modAssembly.GetTypes())
            {
                if (t.IsSubclassOf(typeof(Cutscene)))
                {
                    try
                    {
                        var cutscene = (Cutscene)Activator.CreateInstance(t, null);
                        Cutscenes.Add(cutscene);
                    }
                    catch (Exception ex)
                    {
                        // Log the error to help with debugging
                        ModContent.GetInstance<ReverieMod>().Logger.Error($"Failed to create instance of {t.FullName}: {ex.Message}");
                    }
                }
            }

            // Verify that cutscenes have been loaded
            if (Cutscenes.Count == 0)
            {
                ModContent.GetInstance<ReverieMod>().Logger.Warn("No cutscenes were loaded.");
            }
        }

        public void Unload() => Cutscenes?.Clear();

        public static void AddCutsceneLayer(List<GameInterfaceLayer> layers, Cutscene cutscene, int index, bool visible)
        {
            string name = cutscene == null ? "Unknown" : cutscene.GetType().Name;

            layers.Insert(index, new LegacyGameInterfaceLayer(ReverieMod.AbbreviationPrefix + name,
                delegate
                {
                    if (visible)
                        cutscene?.Draw();

                    return true;
                },
                InterfaceScaleType.UI
            ));
        }

        public static T GetCutscene<T>() where T : Cutscene
        {
            T cutscene = Cutscenes.FirstOrDefault(c => c is T) as T;

            if (cutscene == null)
            {
                ModContent.GetInstance<ReverieMod>().Logger.Warn($"Cutscene of type {typeof(T).FullName} not found.");
            }

            return cutscene;
        }
    }
}