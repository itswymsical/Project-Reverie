using ReverieMod.Common.Systems;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace ReverieMod.Content.Biomes
{
	public class WoodlandCanopyBiome : ModBiome
	{
        public static int trunkX;
        public static int trunkDir = Main.rand.Next(2);
        public static int spawnX = Main.maxTilesX / 2;
        public static int spawnY = (int)Main.worldSurface - (Main.maxTilesY / 16);

        public static int trunkTopY = (int)(spawnY - (Main.maxTilesY - spawnY) / 8);
        public static int trunkBottomY = (int)(Main.rockLayer + (Main.maxTilesY - Main.rockLayer) / 7);

        public override int Music => MusicLoader.GetMusicSlot(Mod, Assets.Music + "Ruins");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow; // We have set the SceneEffectPriority to be BiomeLow for purpose of example, however default behavior is BiomeLow.

		public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

		public override bool IsBiomeActive(Player player)
		{
            return ModContent.GetInstance<TileCountSystem>().canopyBlockCount >= 40;
        }		
	}
}
