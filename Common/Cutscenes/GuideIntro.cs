using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReverieMod.Common.UI;
using ReverieMod.Core.Mechanics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace ReverieMod.Common.Cutscenes
{
	public class GuideIntro : Cutscene
	{
		private int timer;

		public override void Draw()
		{
			Main.newMusic = MusicID.AltOverworldDay;
			SpriteBatch spriteBatch = Main.spriteBatch;
			var screenCenter = new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
			timer++;

			if (timer > 2400)
				End();
		}

		public override void End()
		{
			timer = 0;
            base.End();
		}
	}
}