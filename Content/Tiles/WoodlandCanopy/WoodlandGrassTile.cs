using Microsoft.Xna.Framework;
using ReLogic.Content;
using ReverieMod.Common.Systems;
using System;
using System.Collections.Generic;
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
            int TRUNK_X;
            int TRUNK_DIR = Main.rand.Next(2);

            int SPAWN_X = Main.maxTilesX / 2;
            int SPAWN_DISTANCE = (Main.maxTilesX - SPAWN_X) / 20;
            if (TRUNK_DIR == 0)
            {
                TRUNK_X = SPAWN_X - SPAWN_DISTANCE;
            }
            else
            {
                TRUNK_X = SPAWN_X + SPAWN_DISTANCE;
            }

            int TRUNK_BOTTOM = (int)(Main.rockLayer + (Main.maxTilesY - Main.rockLayer) / 6);

            int CANOPY_CENTER_X = TRUNK_X;
            int CANOPY_CENTER_Y = TRUNK_BOTTOM + (Main.maxTilesY - TRUNK_BOTTOM) / 4;

            int CANOPY_RADIUS_H = (int)(Main.maxTilesX * 0.035f);
            int CANOPY_RADIUS_V = (int)(Main.maxTilesY * 0.175f);

            if (WorldGen.genRand.NextBool(15))
            {
                for (int x = CANOPY_CENTER_X - CANOPY_RADIUS_H; x <= CANOPY_CENTER_X + CANOPY_RADIUS_H; x++)
                {
                    for (int y = CANOPY_CENTER_Y - CANOPY_RADIUS_V; y <= CANOPY_CENTER_Y + CANOPY_RADIUS_V; y++)
                    {

                        if (ReverieTreeSystem.InsideCanopyRadius(x, y, CANOPY_CENTER_X, CANOPY_CENTER_Y, CANOPY_RADIUS_H, CANOPY_RADIUS_V))
                        {
                            for (int grassX = i - 1; grassX <= i + 1; grassX++)
                            {
                                for (int grassY = j - 1; grassY <= j + 1; grassY++)
                                {
                                    Tile tile2 = Framing.GetTileSafely(grassX, grassY);
                                    if (!tile2.HasTile)
                                    {
                                        if (tile.TileType == TileID.Dirt || TileID.Sets.Grass[tile.TileType])
                                            tile.TileType = Type;

                                        if (tile.HasTile && tile2.WallType == 0)
                                            tile.WallType = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (WorldGen.genRand.NextBool(10) && !tileBelow.HasTile && !(tileBelow.LiquidType == LiquidID.Lava))
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
            if (WorldGen.genRand.NextBool(3) && !tileAbove.HasTile && !(tileBelow.LiquidType == LiquidID.Lava))
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