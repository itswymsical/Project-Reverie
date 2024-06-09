using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ObjectData;

namespace ReverieMod.Content.Tiles.WoodlandCanopy
{
    public class CanopyFoliage : ModTile
    {
        public override string Texture => Assets.Tiles.WoodlandCanopy + Name;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileMergeDirt[Type] = true;
            Main.tileLighted[Type] = false;

            DustType = DustID.GrassBlades;
            HitSound = SoundID.Grass;

            TileMaterials.SetForTileId(Type,TileMaterials._materialsByName["Plant"]);
            TileID.Sets.SwaysInWindBasic[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = new int[] { 20 };
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newTile.Style = 0;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorValidTiles = new int[] { ModContent.TileType<WoodlandGrassTile>() };
            TileObjectData.newTile.AnchorAlternateTiles = new int[] { TileID.ClayPot, TileID.PlanterBox };

            for (int i = 0; i < 8; i++)
            {
                TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
                TileObjectData.addSubTile(TileObjectData.newSubTile.Style);
            }
            TileID.Sets.SwaysInWindBasic[Type] = true;
            AddMapEntry(new Color(95, 143, 65));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) 
            => num = DustID.Grass;
    }
}
