using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Trelamium.Core.Loaders;

namespace Trelamium.Common.Systems
{
    public class TrelamiumSystem : ModSystem
    {
        public override void PostUpdateEverything()
        {
            /* if (!Main.dedServ) // TODO: Ask naka if we can use his Trelamium 2 code, then fix it
            {
                ParticleManager.Instance.UpdateParticles();
                TrailManager.Instance.UpdateTrails();
                VerletManager.Instance.UpdateChains();

            } */
            base.PostUpdateEverything();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)/* tModPorter Note: Removed. Use ModSystem.ModifyInterfaceLayers */
        {
            for (int i = 0; i < CutsceneLoader.Cutscenes.Count; i++)
            {
                var cutscene = CutsceneLoader.Cutscenes[i];
                CutsceneLoader.AddCutsceneLayer(layers, cutscene, cutscene.InsertionIndex(layers), cutscene.Visible);
            }

            /* if (CutsceneLoader.GetCutscene<Credits>().Visible)
                foreach (var layer in layers.Where(l => !l.Name.Equals("TM:Credits")))
                    layer.Active = false;
            */
        }
    }
}
