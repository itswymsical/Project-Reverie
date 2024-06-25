using ReverieMod.Core.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ModLoader;
using ReverieMod.Common.UI;
using Terraria.UI;

namespace ReverieMod.Common.Cutscenes
{
	public class WorldOpenup : Cutscene
	{
		private int timer;
        /*
		public override void Draw()
		{
			SpriteBatch spriteBatch = Main.spriteBatch;

			var screenCenter = new Vector2(Main.screenWidth, Main.screenHeight) / 2f;

			timer++;
            if (!MissionChecks.GUIDE_MISSIONS_WORLDSTART)
            {
                SophieNotification guide = new SophieNotification();
                guide.text = "Oh! You're awake. I was hoping you'd get up sooner.";
                InGameNotificationsTracker.AddNotification(guide);
                if (guide.ShouldBeRemoved)
                {
                    SophieNotification guideLine2 = new SophieNotification();
                    guideLine2.text = $"Nice to meet you, {Main.LocalPlayer.name}. Welcome to Terraria.";
                }
            }

            if (timer > 300)
				End();
		}
		public override void End()
		{
			timer = 0;
			MissionChecks.GUIDE_MISSIONS_WORLDSTART = true;
			base.End();
		}*/
    }
}