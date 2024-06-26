using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using ReverieMod.Core.Loaders;
using System.Linq;
using ReverieMod.Core.Mechanics.Particles;
using ReverieMod.Core.Mechanics.Trails;
using ReverieMod.Core.Mechanics.Verlet;

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