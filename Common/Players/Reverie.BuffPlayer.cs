﻿using Terraria.ModLoader;

namespace ReverieMod.Common.Players
{
	public class BuffPlayer : ModPlayer
	{
        public bool gleamingNectar;

        public bool solarAura;

        public override void ResetEffects()
		{
            gleamingNectar = false;
            solarAura = false;
		}
        public override void UpdateBadLifeRegen()
        {
            if (gleamingNectar)
            {
                Player.lifeRegen += 4;
                Player.manaRegen += 4;
            }
        }
    }
}