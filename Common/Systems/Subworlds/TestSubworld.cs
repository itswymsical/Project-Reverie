using ReverieMod.Utilities;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace ReverieMod.Common.Systems.Subworlds
{
    public class TestSubworld : Subworld
    {
        public override int Width => 2000;
        public override int Height => 1800;

        public override bool ShouldSave => true;
        public override bool NoPlayerSaving => false;

        public override List<GenPass> Tasks => new List<GenPass>()
        {
            new TestWorld()
        };

        public override void OnLoad()
        {
            Main.dayTime = true;
            Main.time = 27000;
        }
        public override void OnEnter()
        {
            SubworldSystem.hideUnderworld = true;
        }
    }
    public class TestWorld : GenPass
    {
        //TODO: remove this once tML changes generation passes
        public TestWorld() : base("Terrain", 1) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Loading Archaea"; // Sets the text displayed for this pass
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.worldSurface; j++)
                {
                    progress.Set((j + i * Main.maxTilesY) / (float)(Main.maxTilesX * Main.maxTilesY)); // Controls the progress bar, should only be set between 0f and 1f
                    Tile tile = Main.tile[i, j];
                    tile.HasTile = true;
                    tile.TileType = TileID.HardenedSand;
                }
            }
            FastNoiseLite noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetFractalType(FastNoiseLite.FractalType.Ridged);
            noise.SetFractalGain(0.300f);
            noise.SetFractalOctaves(4);
            noise.SetFrequency(0.014f);

            int posx = Main.maxTilesX;
            int posy = (int)Main.rockLayer;
            float threshold = 0.7f; // Define your noise threshold
                                    // Gather noise data
            float[,] noiseData = new float[posx, posy];

            for (int x = 0; x < posx / 2; x++)
            {
                for (int y = 0; y < posy; y++)
                {
                    noiseData[x, y] = noise.GetNoise(x, y);
                }
            }

            for (int x = 0; x < posx; x++)
            {
                for (int y = 0; y < posy; y++)
                {
                    if (noiseData[x, y] < threshold)
                    {
                        int worldX = x + posx;
                        int worldY = y + posy;
                        WorldGen.PlaceTile(worldX, worldY, TileID.Sandstone, forced: true);
                    }
                }
            }
        }
    }
}