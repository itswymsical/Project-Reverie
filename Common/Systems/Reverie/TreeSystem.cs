using Terraria.ModLoader;
using Terraria;
using Terraria.WorldBuilding;
using Terraria.IO;
using Terraria.ID;
using System;
using Microsoft.Xna.Framework;
using ReverieMod.Content.Tiles;
using ReverieMod.Helpers;
using ReverieMod.Content.Tiles.WoodlandCanopy;

namespace ReverieMod.Common.Systems.Reverie
{
    public class TreeSystem : ModSystem
    {
        private int trunkDir = Main.rand.Next(2);
        private static int POINT_X = Main.maxTilesX / 2;
        private static int POINT_Y = (int)Main.worldSurface - (Main.maxTilesY / 16);

        private int TRUNK_BOTTOM_Y = (int)(Main.rockLayer + (Main.maxTilesY - Main.rockLayer) / 7);
        private int TRUNK_TOP_Y = (POINT_Y - (Main.maxTilesY - POINT_Y) / 8);

        private float CURVE_FREQUENCY = Main.rand.NextFloat(-0.0765f, 0.0765f);
        private const int CURVE_AMPLITUDE = 4;

        private const int MaxDepth = 5; // Maximum recursion depth
        private const float LengthReductionFactor = 0.7f; // Factor by which branch length is reduced
        private const float AngleChange = MathHelper.Pi / 6; // Angle change for branching

        public static void GenerateTree(Vector2 position, float length, float angle, int depth)
        {
            if (depth <= 0)
            {
                return;
            }

            float endX = position.X + length * (float)Math.Cos(angle);
            float endY = position.Y + length * (float)Math.Sin(angle);

            GenerateTree(new Vector2(endX, endY), length * LengthReductionFactor, angle - AngleChange, depth - 1);

            GenerateTree(new Vector2(endX, endY), length * LengthReductionFactor, angle + AngleChange, depth - 1);
        }

        public void GenTrunk()
        {
            int trunkX;
            int distance = (Main.maxTilesX - POINT_X) / 20;
            
            int trunkWidth = 10;
            if (trunkDir == 0) {
                trunkX = POINT_X - distance;
            }
            else {
                trunkX = POINT_X + distance;
            }
            
            trunkX = Math.Clamp(trunkX, 0, Main.maxTilesX - 1);
            for (int yBottom = TRUNK_BOTTOM_Y; yBottom <= TRUNK_BOTTOM_Y; yBottom++) {
                
                int curWidth = trunkWidth + (yBottom % 5 == 0 ? 2 : 0);
                int curveOffset = (int)(Math.Sin(yBottom * CURVE_FREQUENCY) * CURVE_AMPLITUDE);

                int leftBound = trunkX - curWidth / 2 + curveOffset;
                int rightBound = trunkX + curWidth / 2 + curveOffset;

                for (int xBottom = leftBound; xBottom <= rightBound; xBottom++)
                {
                    WorldGen.KillWall(xBottom, yBottom);
                    WorldGen.PlaceTile(xBottom, yBottom, TileID.LivingWood, forced: true);
                }
            }
            for (int yTop = TRUNK_TOP_Y; yTop <= TRUNK_BOTTOM_Y; yTop++)
            {
                int tunnelWidth = (trunkWidth / 2) + (yTop % 5 == 0 ? 2 : 0);
                int tunnelOffset = (int)(Math.Sin(yTop * CURVE_FREQUENCY) * CURVE_AMPLITUDE);
                int leftBound = trunkX - tunnelWidth / 2 + tunnelOffset;
                int rightBound = trunkX + tunnelWidth / 2 + tunnelOffset;
                for (int xTop = leftBound; xTop <= rightBound; xTop++)
                {
                    WorldGen.KillTile(xTop, yTop);
                    WorldGen.PlaceWall(xTop, yTop, WallID.LivingWoodUnsafe);
                }
            }
        }

        public void GenBranches()
        {

        }
    }
}