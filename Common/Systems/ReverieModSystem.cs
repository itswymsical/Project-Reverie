using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using ReverieMod.Core.Loaders;
using System.Linq;
using ReverieMod.Core.Mechanics.Particles;
using ReverieMod.Core.Mechanics.Trails;
using ReverieMod.Core.Mechanics.Verlet;
using ReverieMod.Core.Abstraction.Interfaces;
using System;
using System.Reflection;

namespace ReverieMod.Common.Systems
{
    public class ReverieModSystem : ModSystem
    {
        public override void PostUpdateEverything()
        {
            if (!Main.dedServ)
            {
                //ParticleManager.Instance.UpdateParticles();
                //TrailManager.Instance.UpdateTrails();
                //VerletManager.Instance.UpdateChains();
            }
            base.PostUpdateEverything();
        }

        private List<ILoadableReverieMod> loadCache;

        public override void Unload() => UnloadCache();

        public override void PostSetupContent() => PostLoad();

        private void PostLoad()
        {
            Assembly modAssembly = Assembly.GetExecutingAssembly();

            foreach (Type type in modAssembly.GetTypes())
            {
                if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(IPostLoadable)))
                {
                    (Activator.CreateInstance(type) as IPostLoadable).Load();
                }
            }
        }

        private void LoadCache()
        {
            loadCache = new List<ILoadableReverieMod>();
            Assembly modAssembly = Assembly.GetExecutingAssembly();

            foreach (Type type in modAssembly.GetTypes())
            {
                if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(ILoadableReverieMod)))
                {
                    loadCache.Add(Activator.CreateInstance(type) as ILoadableReverieMod);
                }
            }

            loadCache.Sort((x, y) => x.Priority > y.Priority ? 1 : -1);

            for (int i = 0; i < loadCache.Count; ++i)
            {
                if (Main.dedServ && !loadCache[i].LoadOnDedServer)
                {
                    continue;
                }

                loadCache[i].Load();
            }
        }

        private void UnloadCache()
        {
            foreach (var loadable in loadCache)
            {
                loadable?.Unload();
            }

            loadCache?.Clear();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            for (int i = 0; i < CutsceneLoader.Cutscenes.Count; i++)
            {
                var cutscene = CutsceneLoader.Cutscenes[i];
                CutsceneLoader.AddCutsceneLayer(layers, cutscene, cutscene.InsertionIndex(layers), cutscene.Visible);
            }
        }
    }
}