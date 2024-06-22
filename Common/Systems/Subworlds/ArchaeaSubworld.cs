using SubworldLibrary;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;
using ReverieMod.Utilities;
using ReverieMod.Content.Tiles.Archaea.RedDesert;
using Terraria.ID;
using ReverieMod.Helpers;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ReverieMod.Common.Systems.Subworlds
{
    public class ArchaeaSubworld : Subworld
    {
        public override int Width => 2200;
        public override int Height => 1400;
        public override bool ShouldSave => false;
        public override bool NoPlayerSaving => true;

        public override List<GenPass> Tasks => new List<GenPass>()
        {
            new DesertPass()
        };

        public override void OnLoad() => Main.time += Main.time / 2; // Timezones, Baby :swag:
        public override void OnUnload() => Main.time -= Main.time / 2;
        public override void Update()
        {
            Main.time += 1;
            Player player = Main.LocalPlayer;
            if (player.ZoneForest || player.ZoneSkyHeight || player.ZonePurity)
            {
                player.ZoneDesert = true;
            }
        }
        public override void OnEnter()
        {
            SubworldSystem.hideUnderworld = true;
        }
    }
    public class DesertPass : GenPass
    {
        //TODO: remove this once tML changes generation passes
        public DesertPass() : base("Terrain", 1) { }
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating Terrain";
            FastNoiseLite noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(4);
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
}