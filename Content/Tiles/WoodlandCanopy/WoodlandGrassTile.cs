using Microsoft.Xna.Framework;
using ReverieMod.Common.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace ReverieMod.Content.Tiles.WoodlandCanopy
{
    public class WoodlandGrassTile : ModTile
    {
        public override string Texture => Assets.Tiles.WoodlandCanopy + Name;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[TileID.Dirt][Type] = true;
            Main.tileMerge[Type][Type] = true;

            TileID.Sets.Grass[Type] = true;
            //TileID.Sets.Conversion.Grass[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;
            MineResist = 0.1f;
            DustType = 39;
            AddMapEntry(new Color(111, 155, 37));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Tile tileAbove = Framing.GetTileSafely(i, j);
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);

            if (WorldGen.genRand.NextBool() && !tileBelow.HasTile && !(tileBelow.LiquidType == LiquidID.Lava))
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
            if (WorldGen.genRand.NextBool() && !tileAbove.HasTile && !(tileBelow.LiquidType == LiquidID.Lava))
            {
                if (!tile.BottomSlope && !tile.TopSlope && !tile.IsHalfBlock && !tile.TopSlope)
                {
                    tileAbove.TileType = (ushort)ModContent.TileType<CanopyGrassFoliageTile>();
                    tileAbove.HasTile = true;
                    tileAbove.TileFrameY = 0;
                    tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(8) * 18);
                    WorldGen.SquareTileFrame(i, j - 1, true);
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendTileSquare(-1, i, j - 1, 1, TileChangeType.None);
                }
            }
            if (WorldGen.genRand.NextBool() && !tileAbove.HasTile && !(tileBelow.LiquidType == LiquidID.Lava))
            {
                if (!tile.BottomSlope && !tile.TopSlope && !tile.IsHalfBlock && !tile.TopSlope)
                {
                    tileAbove.TileType = (ushort)ModContent.TileType<AlderwoodSapling>();
                    tileAbove.HasTile = true;
                    WorldGen.SquareTileFrame(i, j - 1, true);
                    if (Main.netMode == NetmodeID.Server)
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
            if (!fail)
            {
                fail = true;
                Framing.GetTileSafely(i, j).TileType = TileID.Dirt;
            }
        }
    }
}