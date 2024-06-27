using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using ReverieMod.Common.Systems.WorldGeneration;

namespace ReverieMod.Content.Biomes
{
    public class WoodlandCanopyBiome : ModBiome
	{
        public override int Music => MusicLoader.GetMusicSlot(Mod, Assets.Music + "Woodhaven");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium; // We have set the SceneEffectPriority to be BiomeLow for purpose of example, however default behavior is BiomeLow.
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<CanopyBackgroundStyle>();
        public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

		public override bool IsBiomeActive(Player player) => (player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight) && ModContent.GetInstance<TileCountSystem>().canopyBlockCount >= 50;
        	
	}
}
