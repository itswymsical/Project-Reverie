using SubworldLibrary;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;
using ReverieMod.Utilities;
using Terraria.ID;
using ReverieMod.Helpers;


namespace ReverieMod.Common.Systems.Subworlds
{
    public class ArchaeaSubworld : Subworld
    {
        public override int Width => 2200;
        public override int Height => 1400;
        public override bool ShouldSave => false;
        public override bool NoPlayerSaving => true;

        public override List<GenPass> Tasks => new()
        {
            new DesertPass("DesertPass", 1f),
            new CavernPass("CavernPass", 1f),
            new SmoothPass("SmoothingPass", 1f),
            new PlantPass("PlantPass", 1f)
        };
        public override void Update()
        {
            Main.time += 1;
            Player player = Main.LocalPlayer;
            if (player.ZoneForest || player.ZoneSkyHeight || player.ZonePurity)
            {
                player.ZoneDesert = true;
            }
            if (player.ZoneRockLayerHeight)
            {
                player.ZoneUndergroundDesert = true;
            }
        }
        public override void OnEnter()
        {
            SubworldSystem.hideUnderworld = true;
        }
    }
    public class DesertPass : GenPass
    {
        public DesertPass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating Terrain";
            FastNoiseLite noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(2);
            noise.SetFrequency(0.01f);

            Main.spawnTileX = Main.maxTilesX / 2;
            Main.spawnTileY = (int)Main.worldSurface - 10;
            // Full Desert
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                // Get noise value for current x position.
                float noiseValue = noise.GetNoise(x, Main.maxTilesY / 2);

                // Scale noise value to fit within terrain height range starting from worldSurface.
                int height = (int)((Main.worldSurface - 10) + noiseValue * 25); // Adjust 20 for more or less height variation.

                // Ensure height is within valid range.
                height = Math.Clamp(height, 0, Main.maxTilesY - 1);

                // Generate terrain based on noise value.
                for (int y = height; y < Main.UnderworldLayer; y++)
                {
                    // Place sandstone tiles below the sand to prevent it from falling
                    WorldGen.KillTile(x, y, noItem: true);
                    WorldGen.PlaceTile(x, y, TileID.Sandstone, true, true);
                    WorldGen.PlaceWall(x, y, WallID.SandstoneEcho);
                }
                for (int y = height; y < (int)Main.worldSurface + (Main.worldSurface / 4); y++)
                {
                    WorldGen.PlaceTile(x, y, TileID.Sand, forced: true);
                }
            }
            /* Red Desert
            int position = Main.rand.Next(Main.maxTilesX / 5, Main.maxTilesX + (Main.maxTilesX / 5));
            for (int x = 0; x < 300; x++)
            {
                // Get noise value for current x position.
                float noiseValue = noise.GetNoise(x, Main.maxTilesY / 2);

                // Scale noise value to fit within terrain height range starting from worldSurface.
                int height = (int)((Main.worldSurface - 10) + noiseValue * 25); // Adjust 20 for more or less height variation.

                // Ensure height is within valid range.
                height = Math.Clamp(height, 0, Main.maxTilesY - 1);

                for (int y = height; y < Main.UnderworldLayer; y++)
                {
                    WorldGen.KillTile(x, y, noItem: true);
                    WorldGen.PlaceTile(position, y, TileID.Sandstone, true, true);

                }
                for (int y = height; y < (int)Main.worldSurface + (Main.worldSurface / 4); y++)
                {
                    WorldGen.PlaceTile(position, y, TileType<RedSand>(), forced: true);
                }
            }
            */
            Helper.SmoothTerrain(Main.maxTilesX, Main.maxTilesY);
        }
    }
    public class CavernPass : GenPass
    {
        public CavernPass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating caves";
            FastNoiseLite noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(5);
            noise.SetFrequency(0.085f);
            float threshold = 0.1f; // This is basically which noise values are we going to ignore/kill.
            int posx = Main.maxTilesX;
            int posy = (int)Main.UnderworldLayer;
            float[,] noiseData = new float[posx, posy];

            for (int x = 0; x < posx; x++)
            {
                for (int y = 0; y < posy; y++)
                {
                    int worldX = x;
                    int worldY = y;

                    noiseData[x, y] = noise.GetNoise(worldX, worldY);
                }
            }
            for (int x = 0; x < posx; x++)
            {
                for (int y = (int)Main.rockLayer; y < posy; y++)
                {
                    int worldX = x;
                    int worldY = y;
                    if (noiseData[x, y] > -1f && noiseData[x, y] < -0.09f)
                        WorldGen.PlaceTile(worldX, worldY, TileID.HardenedSand, forced: true);

                    if (noiseData[x, y] > threshold)
                        WorldGen.KillTile(worldX, worldY);

                    progress.Set((float)((x * posy + y) + (posx * posy)) / (2 * posx * posy));
                }
            }
        }
    }
    public class SmoothPass : GenPass
    {
        public SmoothPass(string name, float loadWeight) : base(name, loadWeight) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Smoothing the World";
            // WARNING x WARNING x WARNING
            // Heavily nested code copied from decompiled code
            for (int tileX = 20; tileX < Main.maxTilesX - 20; tileX++)
            {
                float percentAcrossWorld = (float)tileX / (float)Main.maxTilesX;
                progress.Set(percentAcrossWorld);
                for (int tileY = 20; tileY < Main.maxTilesY - 20; tileY++)
                {
                    if (Main.tile[tileX, tileY].TileType != 48 && Main.tile[tileX, tileY].TileType != 137 && Main.tile[tileX, tileY].TileType != 232 && Main.tile[tileX, tileY].TileType != 191 && Main.tile[tileX, tileY].TileType != 151 && Main.tile[tileX, tileY].TileType != 274)
                    {
                        if (!Main.tile[tileX, tileY - 1].HasTile)
                        {
                            if (WorldGen.SolidTile(tileX, tileY) && TileID.Sets.CanBeClearedDuringGeneration[Main.tile[tileX, tileY].TileType])
                            {
                                if (!Main.tile[tileX - 1, tileY].IsHalfBlock && !Main.tile[tileX + 1, tileY].IsHalfBlock && Main.tile[tileX - 1, tileY].Slope == SlopeType.Solid && Main.tile[tileX + 1, tileY].Slope == SlopeType.Solid)
                                {
                                    if (WorldGen.SolidTile(tileX, tileY + 1))
                                    {
                                        if (!WorldGen.SolidTile(tileX - 1, tileY) && !Main.tile[tileX - 1, tileY + 1].IsHalfBlock && WorldGen.SolidTile(tileX - 1, tileY + 1) && WorldGen.SolidTile(tileX + 1, tileY) && !Main.tile[tileX + 1, tileY - 1].HasTile)
                                        {
                                            if (WorldGen.genRand.NextBool(2))
                                                WorldGen.SlopeTile(tileX, tileY, 2);
                                            else
                                                WorldGen.PoundTile(tileX, tileY);
                                        }
                                        else if (!WorldGen.SolidTile(tileX + 1, tileY) && !Main.tile[tileX + 1, tileY + 1].IsHalfBlock && WorldGen.SolidTile(tileX + 1, tileY + 1) && WorldGen.SolidTile(tileX - 1, tileY) && !Main.tile[tileX - 1, tileY - 1].HasTile)
                                        {
                                            if (WorldGen.genRand.NextBool(2))
                                                WorldGen.SlopeTile(tileX, tileY, 1);
                                            else
                                                WorldGen.PoundTile(tileX, tileY);
                                        }
                                        else if (WorldGen.SolidTile(tileX + 1, tileY + 1) && WorldGen.SolidTile(tileX - 1, tileY + 1) && !Main.tile[tileX + 1, tileY].HasTile && !Main.tile[tileX - 1, tileY].HasTile)
                                        {
                                            WorldGen.PoundTile(tileX, tileY);
                                        }

                                        if (WorldGen.SolidTile(tileX, tileY))
                                        {
                                            if (WorldGen.SolidTile(tileX - 1, tileY) && WorldGen.SolidTile(tileX + 1, tileY + 2) && !Main.tile[tileX + 1, tileY].HasTile && !Main.tile[tileX + 1, tileY + 1].HasTile && !Main.tile[tileX - 1, tileY - 1].HasTile)
                                            {
                                                WorldGen.KillTile(tileX, tileY);
                                            }
                                            else if (WorldGen.SolidTile(tileX + 1, tileY) && WorldGen.SolidTile(tileX - 1, tileY + 2) && !Main.tile[tileX - 1, tileY].HasTile && !Main.tile[tileX - 1, tileY + 1].HasTile && !Main.tile[tileX + 1, tileY - 1].HasTile)
                                            {
                                                WorldGen.KillTile(tileX, tileY);
                                            }
                                            else if (!Main.tile[tileX - 1, tileY + 1].HasTile && !Main.tile[tileX - 1, tileY].HasTile && WorldGen.SolidTile(tileX + 1, tileY) && WorldGen.SolidTile(tileX, tileY + 2))
                                            {
                                                if (WorldGen.genRand.NextBool(5))
                                                    WorldGen.KillTile(tileX, tileY);
                                                else if (WorldGen.genRand.NextBool(5))
                                                    WorldGen.PoundTile(tileX, tileY);
                                                else
                                                    WorldGen.SlopeTile(tileX, tileY, 2);
                                            }
                                            else if (!Main.tile[tileX + 1, tileY + 1].HasTile && !Main.tile[tileX + 1, tileY].HasTile && WorldGen.SolidTile(tileX - 1, tileY) && WorldGen.SolidTile(tileX, tileY + 2))
                                            {
                                                if (WorldGen.genRand.NextBool(5))
                                                    WorldGen.KillTile(tileX, tileY);
                                                else if (WorldGen.genRand.NextBool(5))
                                                    WorldGen.PoundTile(tileX, tileY);
                                                else
                                                    WorldGen.SlopeTile(tileX, tileY, 1);
                                            }
                                        }
                                    }

                                    if (WorldGen.SolidTile(tileX, tileY) && !Main.tile[tileX - 1, tileY].HasTile && !Main.tile[tileX + 1, tileY].HasTile)
                                        WorldGen.KillTile(tileX, tileY);
                                }
                            }
                            else if (!Main.tile[tileX, tileY].HasTile && Main.tile[tileX, tileY + 1].TileType != 151 && Main.tile[tileX, tileY + 1].TileType != 274)
                            {
                                if (Main.tile[tileX + 1, tileY].TileType != 190 && Main.tile[tileX + 1, tileY].TileType != 48 && Main.tile[tileX + 1, tileY].TileType != 232 && WorldGen.SolidTile(tileX - 1, tileY + 1) && WorldGen.SolidTile(tileX + 1, tileY) && !Main.tile[tileX - 1, tileY].HasTile && !Main.tile[tileX + 1, tileY - 1].HasTile)
                                {
                                    WorldGen.PlaceTile(tileX, tileY, Main.tile[tileX, tileY + 1].TileType, mute: true);
                                    if (WorldGen.genRand.NextBool(2))
                                        WorldGen.SlopeTile(tileX, tileY, 2);
                                    else
                                        WorldGen.PoundTile(tileX, tileY);
                                }

                                if (Main.tile[tileX - 1, tileY].TileType != 190 && Main.tile[tileX - 1, tileY].TileType != 48 && Main.tile[tileX - 1, tileY].TileType != 232 && WorldGen.SolidTile(tileX + 1, tileY + 1) && WorldGen.SolidTile(tileX - 1, tileY) && !Main.tile[tileX + 1, tileY].HasTile && !Main.tile[tileX - 1, tileY - 1].HasTile)
                                {
                                    WorldGen.PlaceTile(tileX, tileY, Main.tile[tileX, tileY + 1].TileType, mute: true);
                                    if (WorldGen.genRand.NextBool(2))
                                        WorldGen.SlopeTile(tileX, tileY, 1);
                                    else
                                        WorldGen.PoundTile(tileX, tileY);
                                }
                            }
                        }
                        else if (!Main.tile[tileX, tileY + 1].HasTile && WorldGen.genRand.NextBool(2) && WorldGen.SolidTile(tileX, tileY) && !Main.tile[tileX - 1, tileY].IsHalfBlock && !Main.tile[tileX + 1, tileY].IsHalfBlock && Main.tile[tileX - 1, tileY].Slope == SlopeType.Solid && Main.tile[tileX + 1, tileY].Slope == SlopeType.Solid && WorldGen.SolidTile(tileX, tileY - 1))
                        {
                            if (WorldGen.SolidTile(tileX - 1, tileY) && !WorldGen.SolidTile(tileX + 1, tileY) && WorldGen.SolidTile(tileX - 1, tileY - 1))
                                WorldGen.SlopeTile(tileX, tileY, 3);
                            else if (WorldGen.SolidTile(tileX + 1, tileY) && !WorldGen.SolidTile(tileX - 1, tileY) && WorldGen.SolidTile(tileX + 1, tileY - 1))
                                WorldGen.SlopeTile(tileX, tileY, 4);
                        }

                        if (TileID.Sets.Conversion.Sand[Main.tile[tileX, tileY].TileType])
                            Tile.SmoothSlope(tileX, tileY, applyToNeighbors: false);
                    }
                }
            }

            for (int tileX = 20; tileX < Main.maxTilesX - 20; tileX++)
            {
                for (int tileY = 20; tileY < Main.maxTilesY - 20; tileY++)
                {
                    if (WorldGen.genRand.NextBool(2) && !Main.tile[tileX, tileY - 1].HasTile && Main.tile[tileX, tileY].TileType != 137 && Main.tile[tileX, tileY].TileType != 48 && Main.tile[tileX, tileY].TileType != 232 && Main.tile[tileX, tileY].TileType != 191 && Main.tile[tileX, tileY].TileType != 151 && Main.tile[tileX, tileY].TileType != 274 && Main.tile[tileX, tileY].TileType != 75 && Main.tile[tileX, tileY].TileType != 76 && WorldGen.SolidTile(tileX, tileY) && Main.tile[tileX - 1, tileY].TileType != 137 && Main.tile[tileX + 1, tileY].TileType != 137)
                    {
                        if (WorldGen.SolidTile(tileX, tileY + 1) && WorldGen.SolidTile(tileX + 1, tileY) && !Main.tile[tileX - 1, tileY].HasTile)
                            WorldGen.SlopeTile(tileX, tileY, 2);

                        if (WorldGen.SolidTile(tileX, tileY + 1) && WorldGen.SolidTile(tileX - 1, tileY) && !Main.tile[tileX + 1, tileY].HasTile)
                            WorldGen.SlopeTile(tileX, tileY, 1);
                    }

                    if (Main.tile[tileX, tileY].Slope == SlopeType.SlopeDownLeft && !WorldGen.SolidTile(tileX - 1, tileY))
                    {
                        WorldGen.SlopeTile(tileX, tileY);
                        WorldGen.PoundTile(tileX, tileY);
                    }

                    if (Main.tile[tileX, tileY].Slope == SlopeType.SlopeDownRight && !WorldGen.SolidTile(tileX + 1, tileY))
                    {
                        WorldGen.SlopeTile(tileX, tileY);
                        WorldGen.PoundTile(tileX, tileY);
                    }
                }
            }
        }
    }
    public class PlantPass : GenPass
    {
        public PlantPass(string name, float loadWeight) : base(name, loadWeight)
        {
        }
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Growing plants";
            int fail = 0;
            for (int i = 1; i < Main.maxTilesX - 1; i++)
            {
                progress.Set(i / Main.maxTilesX);

                if (fail > 0)
                {
                    fail--;
                    continue;
                }

                if (WorldGen.genRand.NextBool(5))
                {
                    for (int j = 1; j < Main.worldSurface; j++)
                    {
                        if (Main.tile[i, j].HasTile && Main.tile[i, j].BlockType == BlockType.Solid)
                        {
                            WorldGen.PlaceTile(i, j - 1, TileID.Cactus, mute: true);
                            fail = WorldGen.genRand.Next(5, 10);
                            break;
                        }
                    }
                }

                if (WorldGen.genRand.NextBool(22))
                {
                    for (int j = 1; j < Main.worldSurface; j++)
                    {
                        if (Main.tile[i, j].HasTile && Main.tile[i, j].BlockType == BlockType.Solid)
                        {
                            WorldGen.PlaceTile(i, j - 1, TileID.Saplings, mute: true);
                            fail = WorldGen.genRand.Next(5, 10);
                            WorldGen.GrowPalmTree(i, j - 1);
                            break;
                        }
                    }
                }
            }
        }
    }
}