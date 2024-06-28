using Microsoft.CodeAnalysis.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReverieMod.Common.Players;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;

namespace ReverieMod.Common.UI
{
    public class MissionStatusIndicator : IInGameNotification
	{
		public bool ShouldBeRemoved => timeLeft <= 0;
		private int timeLeft = 5 * 60;
		private float Scale
		{
			get
			{
				if (timeLeft < 30)
				{
					return MathHelper.Lerp(0f, 1f, timeLeft / 30f);
				}

				if (timeLeft > 285)
				{
					return MathHelper.Lerp(1f, 0f, (timeLeft - 285) / 15f);
				}

				return 1f;
			}
		}
		private float Opacity
		{
			get
			{
				if (Scale <= 0.5f)
				{
					return 0f;
				}

				return (Scale - 0.5f) / 0.5f;
			}
		}
		public void Update()
		{
			if (timeLeft == 5 * 60)
			{
				if (!MissionChecks.failed)
				{
                    SoundEngine.PlaySound(SoundID.AchievementComplete, Main.LocalPlayer.position);
                }
				else
				{
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(ReverieMod)}/Assets/SFX/MitigateDrop"), Main.LocalPlayer.position);
                }
                
            }
			timeLeft--;
			
			if (timeLeft < 0)
				timeLeft = 0;
			
		}
        private float panelWidth = 420f;
        public void DrawInGame(SpriteBatch spriteBatch, Vector2 bottomAnchorPosition)
		{
			if (Opacity <= 0f)
				return;
			
			Player player = Main.LocalPlayer;
			ExperiencePlayer modPlayer = player.GetModPlayer<ExperiencePlayer>();
			var iconTexture = TextureAssets.Item[ItemID.Confetti];
			if (MissionChecks.failed)
			{
				iconTexture = TextureAssets.MapDeath;
			}
			string title = "Mission Complete!";
			if (MissionChecks.failed)
			{
				title = "Mission Failed!";
			}
            float effectiveScale = Scale * 1.1f;
            // Measure the size of the text and calculate the height based on the static width
            Vector2 textSize = FontAssets.ItemStack.Value.MeasureString(title);
            int lineCount = (int)Math.Ceiling(textSize.X / panelWidth);
            float panelHeight = ( new Vector2(35f, 20f).Y * 2f) * effectiveScale;

            // Calculate panel size
            Vector2 panelSize = new Vector2(panelWidth + 18, panelHeight);

            // Calculate the panel's position and centered text position
            Vector2 panelPosition = bottomAnchorPosition + new Vector2(0f, (-100f - panelSize.Y) * 0.5f);
            Rectangle panelRectangle = Utils.CenteredRectangle(panelPosition, panelSize);
			Color color = Color.White;
            Color colorText = new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor / 5, Main.mouseTextColor);
            if (MissionChecks.failed)
			{
				color = Color.Black;
				colorText = Color.Red;

            }
            Utils.DrawInvBG(spriteBatch, panelRectangle, color * 0.5f);
			float iconScale = effectiveScale * 0.7f;
			Vector2 vector = panelRectangle.Right() - Vector2.UnitX * effectiveScale * (12f + iconScale * iconTexture.Width());
			spriteBatch.Draw(iconTexture.Value, vector, null, Color.White * Opacity, 0f, new Vector2(0f, iconTexture.Width() / 2f), iconScale, SpriteEffects.None, 0f);
			Utils.DrawBorderString(color: colorText * Opacity, sb: spriteBatch, text: title, pos: panelPosition, scale: effectiveScale * 0.9f, anchorx: 1f, anchory: 0.4f);
		}

		public void PushAnchor(ref Vector2 positionAnchorBottom) => positionAnchorBottom.Y -= 50f * Opacity;
	}
}