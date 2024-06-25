using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReverieMod.Common.Players;
using System.Collections.Generic;
using System.Text;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace ReverieMod.Common.UI
{
	public class WorldNPCDialogueNotification : IInGameNotification
	{
		public bool ShouldBeRemoved => timeLeft <= 0;
		private int timeLeft = 5 * 60;
        public string text;

        public Asset<Texture2D> icon = ModContent.Request<Texture2D>("ReverieMod/Assets/Textures/UI/DialoguePortraits/Sophie");
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
			timeLeft--;

			if (timeLeft < 0)
				timeLeft = 0;
			
		}
        public string Dialogue (string text)
        {
            return text;
        }
        public void DrawInGame(SpriteBatch spriteBatch, Vector2 bottomAnchorPosition)
        {
            if (Opacity <= 0f)
                return;

            Player player = Main.LocalPlayer;
            ExperiencePlayer modPlayer = player.GetModPlayer<ExperiencePlayer>();
            
            string title = Dialogue(text);

            float effectiveScale = Scale * 1.1f;

            // Define a static width for the panel
            float panelWidth = 420f; // Adjust as needed
            Vector2 panelPadding = new Vector2(38f, 20f); // Adjust padding as needed

            // Measure the size of the text and calculate the height based on the static width
            Vector2 textSize = FontAssets.ItemStack.Value.MeasureString(title);
            int lineCount = (int)Math.Ceiling(textSize.X / panelWidth);
            float panelHeight = (textSize.Y * lineCount + panelPadding.Y * 2f) * effectiveScale;

            // Calculate panel size
            Vector2 panelSize = new Vector2(panelWidth + 18, panelHeight);

            // Calculate the panel's position and centered text position
            Vector2 panelPosition = bottomAnchorPosition + new Vector2(0f, (-100f - panelSize.Y) * 0.5f);
            Rectangle panelRectangle = Utils.CenteredRectangle(panelPosition, panelSize);

            // Position for the icon outside the panel, to the left
            float iconScale = effectiveScale * 2.7f;
            Vector2 iconSize = new Vector2(icon.Width(), icon.Height()) * iconScale;
            Vector2 iconPosition = new Vector2(panelRectangle.Left - iconSize.X - 10f, panelRectangle.Center.Y - iconSize.Y / 2f);

            // Position for the text centered within the panel
            Vector2 textPosition = panelRectangle.TopLeft() + panelPadding * effectiveScale;

            // Check if the mouse is hovering over the panel
            bool isHovering = panelRectangle.Contains(Main.MouseScreen.ToPoint());

            // Draw the background panel with a varying opacity based on hover state
            Utils.DrawInvBG(spriteBatch, panelRectangle, Color.Violet * (isHovering ? 0.75f : 0.5f));

            // Draw the icon outside the panel
            spriteBatch.Draw(icon.Value, iconPosition, null, Color.White * Opacity, 0f, Vector2.Zero, iconScale, SpriteEffects.None, 0f);

            // Draw the title text inside the panel, adjusting for lines
            string[] wrappedText = WrapText(title, panelWidth / effectiveScale);
            for (int i = 0; i < wrappedText.Length; i++)
            {
                Vector2 linePosition = textPosition + new Vector2(-38f, i * textSize.Y * effectiveScale);
                Utils.DrawBorderString(spriteBatch, wrappedText[i], linePosition, Color.White * Opacity, effectiveScale * 0.9f, anchorx: 0f, anchory: 0.5f);
            }

            // Trigger mouse over action if hovering
            if (isHovering)
                OnMouseOver();
        }

        private string[] WrapText(string text, float maxWidth)
        {
            List<string> lines = new List<string>();
            StringBuilder currentLine = new StringBuilder();
            float spaceWidth = FontAssets.ItemStack.Value.MeasureString(" ").X;

            foreach (var word in text.Split(' '))
            {
                float wordWidth = FontAssets.ItemStack.Value.MeasureString(word).X;
                if (currentLine.Length > 0 && FontAssets.ItemStack.Value.MeasureString(currentLine.ToString()).X + wordWidth + spaceWidth > maxWidth)
                {
                    lines.Add(currentLine.ToString());
                    currentLine.Clear();
                }
                if (currentLine.Length > 0)
                    currentLine.Append(" ");
                currentLine.Append(word);
            }

            if (currentLine.Length > 0)
                lines.Add(currentLine.ToString());

            return lines.ToArray();
        }
        private void OnMouseOver()
		{
			if (PlayerInput.IgnoreMouseInterface)
				return;			

			Main.LocalPlayer.mouseInterface = true;

			if (!Main.mouseLeft || !Main.mouseLeftRelease)
				return;
			
			Main.mouseLeftRelease = false;

			if (timeLeft > 30)
				timeLeft = 30;
			
		}

		public void PushAnchor(ref Vector2 positionAnchorBottom) => positionAnchorBottom.Y -= 50f * Opacity;
		
	}
}