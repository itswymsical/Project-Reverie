using Microsoft.Xna.Framework;
using ReverieMod.Common.Systems;
using ReverieMod.Content.Items.Tiles.Canopy;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace ReverieMod.Content.Tiles.Canopy
{
    public class Woodgrass : ModTile
    {
        public override string Texture => Assets.Tiles.Canopy + Name;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[TileID.LivingWood][Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = TileID.LivingWood;
            Main.tileMerge[Type][Type] = true;
            TileID.Sets.Grass[Type] = true;

            //TileID.Sets.Conversion.Grass[Type] = true;
            //TileID.Sets.CanBeDugByShovel[Type] = true;

            MineResist = 0.3f;
            DustType = DustID.t_LivingWood;
            AddMapEntry(new Color(95, 143, 65));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);

            if (WorldGen.genRand.NextBool(15) && !tileBelow.HasTile && !(tileBelow.LiquidType == LiquidID.Lava))
            {
                if (!tile.BottomSlope)
                {
                    tileBelow.TileType = (ushort)ModContent.TileType<CanopyVine>();
                    tileBelow.HasTile = true;
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j + 1, 3, 0);
                    }
                }
            }

            if (WorldGen.genRand.NextBool() && !tileAbove.HasTile && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
            {
                WorldGen.PlaceTile(i, j - 1, (ushort)ModContent.TileType<CanopyFoliage>());
                tileAbove.TileFrameY = 0;
                tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(10) * 18);
                WorldGen.SquareTileFrame(i, j + 1, true);
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendTileSquare(-1, i, j - 1, 1, TileChangeType.None);
                }
            }
        }
        public override bool CanExplode(int i, int j)
        {
            WorldGen.KillTile(i, j, false, false, true);
            return true;
        }
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (Main.rand.NextBool(30))
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 48, ModContent.ItemType<WoodgrassSeeds>());
        }
    }
}