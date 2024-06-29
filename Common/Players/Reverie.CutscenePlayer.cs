using ReverieMod.Common;
using ReverieMod.Core.Loaders;

using Microsoft.Xna.Framework;
using System.Linq;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReverieMod.Common.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using ReLogic.Content;
using static ReverieMod.Common.UI.NPCData;
using System;

namespace ReverieMod.Common.Players
{
	public class CutscenePlayer : ModPlayer
    {
        public Vector2[] CreditsScreenPosition = new Vector2[4];
        public bool RegisteredCreditsPositions;

        public bool PlayCredits;

        public override void ModifyScreenPosition()
        {
            foreach (var cutscene in CutsceneLoader.Cutscenes.Where(c => c.Visible))
                cutscene.ModifyScreenPosition();
        }

        public override void PostUpdateMiscEffects()
        {
            if (!RegisteredCreditsPositions)
            {
                RegisterCreditsPositions();
                RegisteredCreditsPositions = true;
            }
        }

        private void RegisterCreditsPositions()
        {
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile tile = Framing.GetTileSafely(i, j);

                    if (tile.TileType == TileID.Emerald || tile.TileType == TileID.Sapphire)
                        CreditsScreenPosition[0] = new Vector2(i, j) * 16f;

                    if (tile.TileType == TileID.Cactus)
                        CreditsScreenPosition[1] = new Vector2(i, j) * 16f;

                    CreditsScreenPosition[2] = new Vector2(Main.dungeonX, Main.dungeonY) * 16f;

                    if (tile.TileType == TileID.JunglePlants)
                        CreditsScreenPosition[3] = new Vector2(i, j) * 16f;
                }
            }
        }
    }
}
