using Terraria.ModLoader;

namespace Trelamium.Common.Players
{
	public class MinionPlayer : ModPlayer
	{
		public bool BloomBulb;

		public bool floralSpirit;

		public bool funguy;

		public bool mossMonarch;

		public bool pholiotaMinion;

		public bool tuskscaleMinion;

		public bool Tumbleweed;

        public override void ResetEffects()
		{
			BloomBulb = false;

			floralSpirit = false;

			funguy = false;

			mossMonarch = false;

			pholiotaMinion = false;

			Tumbleweed = false;
		}
	}
}
