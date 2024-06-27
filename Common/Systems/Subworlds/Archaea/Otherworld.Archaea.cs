using Microsoft.Xna.Framework.Graphics;
using ReverieMod.Common.Systems.WorldGeneration;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.WorldBuilding;

namespace ReverieMod.Common.Systems.Subworlds.Archaea
{
    public class ArchaeaSubworld : Otherworld
    {
        public override int Width => 2200;
        public override int Height => 1400;
        public override bool ShouldSave => false;
        public override bool NoPlayerSaving => true;
        new Texture2D otherworldTitle = (Texture2D)TextureAssets.Logo2;
        public override List<GenPass> Tasks => new()
        {
            new DesertPass("[Archaea] Desert", 1f),
            new CavernPass("[Archaea] Caverns", 1f),
            new SmoothPass("Smooth World - Reverie", 1f),
            new PlantPass("[Archaea] Plants", 1f)
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
}