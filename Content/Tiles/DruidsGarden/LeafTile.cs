using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Trelamium.Content.Items.Tiles;

namespace Trelamium.Content.Tiles
{
	public abstract class LeafTile : ModTile
	{
		public override string Texture => Assets.Tiles.DruidsGarden + "LeafTile";

		public override void SetStaticDefaults() {
			Main.tileFrameImportant[Type] = true;
			Main.tileNoFail[Type] = true;
			Main.tileObsidianKill[Type] = true;

			DustType = DustID.JungleGrass;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);

			TileObjectData.addTile(Type);

			AddMapEntry(new Color(152, 171, 198));
		}
	}
	public class LeafTileNatural : DGFoliageTile
    {
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			TileObjectData.GetTileData(Type, 0).LavaDeath = false;
		}

		public override void DropCritterChance(int i, int j, ref int wormChance, ref int grassHopperChance, ref int jungleGrubChance) {
			wormChance = 9;
			grassHopperChance = 7;
		}
	}
}