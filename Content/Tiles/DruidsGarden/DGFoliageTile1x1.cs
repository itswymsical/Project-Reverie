using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Trelamium.Content.Items.Tiles;

namespace Trelamium.Content.Tiles
{
	public abstract class DGFoliageTile1x1 : ModTile
	{
		public override string Texture => Assets.Tiles.DruidsGarden + "DGFoliageTile1x1";

		public override void SetStaticDefaults() {
			Main.tileFrameImportant[Type] = true;
			Main.tileNoFail[Type] = true;
			Main.tileObsidianKill[Type] = true;

			DustType = DustID.JungleGrass;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);

			TileObjectData.addTile(Type);

			AddMapEntry(new Color(152, 171, 198));
		}
	}
	public class DGFoliageTile1x1Natural : DGFoliageTile
    {
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
            TileObjectData.GetTileData(Type, 0).LavaDeath = true;
            TileObjectData.GetTileData(Type, 0).WaterDeath = true;
        }

		public override void DropCritterChance(int i, int j, ref int wormChance, ref int grassHopperChance, ref int jungleGrubChance) {
			wormChance = 12;
			grassHopperChance = 7;
		}
	}
}