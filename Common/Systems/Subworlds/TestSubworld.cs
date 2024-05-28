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
        public override int Width => 1000;
        public override int Height => 1000;

        public override bool ShouldSave => false;
        public override bool NoPlayerSaving => true;

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
            SubworldSystem.hideUnderworld = false;
        }
    }
    public class TestWorld : GenPass
    {
        //TODO: remove this once tML changes generation passes
        public TestWorld() : base("Terrain", 1) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating dirt world"; // Sets the text displayed for this pass
            Main.worldSurface = Main.maxTilesY - 42; // Hides the underground layer just out of bounds
            Main.rockLayer = Main.maxTilesY; // Hides the cavern layer way out of bounds
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY / 3; j++)
                {
                    progress.Set((j + i * Main.maxTilesY) / (float)(Main.maxTilesX * Main.maxTilesY)); // Controls the progress bar, should only be set between 0f and 1f
                    Tile tile = Main.tile[i, j];
                    tile.HasTile = true;
                    tile.TileType = TileID.Dirt;
                }
            }
        }
    }
}