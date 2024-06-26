using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReverieMod.Common.Players;
using System;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ReverieMod.Common.UI
{
    public class NPCDialogueBox : IInGameNotification
    {
        public bool ShouldBeRemoved => timeLeft <= 0 && dialogueQueue.Count == 0 && charIndex >= currentDialogue.Length;
        private int timeLeft = 5 * 60;

        public Asset<Texture2D> iconTexture = ModContent.Request<Texture2D>("ReverieMod/Assets/Textures/UI/DialoguePortraits/Guide");
        public string npcName = "?";
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

        private Queue<(string Text, int Delay)> dialogueQueue = new Queue<(string, int)>();
        public string currentDialogue = "...";
        private int charIndex = 0;
        public int charDisplayDelay = 2;
        private int charDisplayTimer = 0;
        private float panelWidth = 420f;
        public Color color = Color.White;
        public Color colorText = Color.White;
        public Color colorIcon = Color.White;
        public SoundStyle characterSound;
        public void Update()
        {
            if (dialogueQueue.Count > 0 && charIndex >= currentDialogue.Length && timeLeft <= 0)
            {
                var nextDialogue = dialogueQueue.Dequeue();
                currentDialogue = nextDialogue.Text;
                charDisplayDelay = nextDialogue.Delay; // Use specific delay for this dialogue
                charIndex = 0;
                timeLeft = 5 * 60; // Reset timeLeft for new dialogue
            }

            if (charIndex < currentDialogue.Length)
            {
                charDisplayTimer++;
                if (charDisplayTimer >= charDisplayDelay)
                {
                    charDisplayTimer = 0;
                    charIndex++;
                    PlayCharacterSound(characterSound);
                }
            }

            timeLeft--;

            if (timeLeft < 0)
                timeLeft = 0;
        }
        private void PlayCharacterSound(SoundStyle sound)
        {
            SoundEngine.PlaySound(sound, Main.LocalPlayer.position);
        }
        public void AddDialogue(string dialogue, int delay = 2)
        {
            dialogueQueue.Enqueue((dialogue, delay));
        }

        public void DrawInGame(SpriteBatch spriteBatch, Vector2 bottomAnchorPosition)
        {
            if (Opacity <= 0f)
                return;

            Player player = Main.LocalPlayer;
            ExperiencePlayer modPlayer = player.GetModPlayer<ExperiencePlayer>();

            float effectiveScale = Scale * 1.1f;
            string displayText = currentDialogue.Substring(0, Math.Min(charIndex, currentDialogue.Length));

            // Measure the size of the text and calculate the height based on the static width
            Vector2 textSize = FontAssets.ItemStack.Value.MeasureString(displayText);
            int lineCount = (int)Math.Ceiling(textSize.X / panelWidth);
            float panelHeight = (textSize.Y * lineCount + new Vector2(35f, 20f).Y * 2f) * effectiveScale;

            // Calculate panel size
            Vector2 panelSize = new Vector2(panelWidth + 18, panelHeight);

            // Calculate the panel's position and centered text position
            Vector2 panelPosition = bottomAnchorPosition + new Vector2(0f, (-100f - panelSize.Y) * 0.5f);
            Rectangle panelRectangle = Utils.CenteredRectangle(panelPosition, panelSize);

            // Position for the icon outside the panel, to the left
            float iconScale = effectiveScale * 2.7f;
            Vector2 iconSize = new Vector2(iconTexture.Width(), iconTexture.Height()) * iconScale;
            Vector2 iconPosition = new Vector2(panelRectangle.Left - iconSize.X - 10f, panelRectangle.Center.Y - iconSize.Y / 2f);

            // Position for the NPC name textbox below the icon
            Vector2 nameTextSize = FontAssets.ItemStack.Value.MeasureString(npcName);
            Vector2 nameTextPosition = new Vector2(iconPosition.X + iconSize.X / 2 - nameTextSize.X / 2, iconPosition.Y + iconSize.Y + 5f);


            // Position for the text centered within the panel
            Vector2 textPosition = panelRectangle.TopLeft() + new Vector2(10f, 20f) * effectiveScale;

            // Check if the mouse is hovering over the panel
            bool isHovering = panelRectangle.Contains(Main.MouseScreen.ToPoint());

            // Draw the background panel with a varying opacity based on hover state
            Utils.DrawInvBG(spriteBatch, panelRectangle, color * (isHovering ? 0.75f : 0.5f));

            // Draw the icon outside the panel
            spriteBatch.Draw(iconTexture.Value, iconPosition, null, colorIcon * Opacity, 0f, Vector2.Zero, iconScale, SpriteEffects.None, 0f);
            Utils.DrawBorderString(spriteBatch, npcName, nameTextPosition, Color.White * Opacity, effectiveScale, anchorx: 0f, anchory: 0.5f);


            // Draw the title text inside the panel, adjusting for lines
            string[] wrappedText = WrapText(displayText, panelWidth / effectiveScale);
            for (int i = 0; i < wrappedText.Length; i++)
            {
                Vector2 linePosition = textPosition + new Vector2(0f, i * textSize.Y * effectiveScale);
                Utils.DrawBorderString(spriteBatch, wrappedText[i], linePosition, colorText * Opacity, effectiveScale * 0.9f, anchorx: 0f, anchory: 0.5f);
            }

            // Trigger mouse over action if hovering
            if (isHovering)
                OnMouseOver(panelRectangle);
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

        private void OnMouseOver(Rectangle panelRectangle)
        {
            if (PlayerInput.IgnoreMouseInterface)
                return;

            Main.LocalPlayer.mouseInterface = true;

            if (!Main.mouseLeft || !Main.mouseLeftRelease)
                return;

            Main.mouseLeftRelease = false;

            if (charIndex < currentDialogue.Length)
            {
                charIndex = currentDialogue.Length; // Skip to the end of the current dialogue
            }
            else if (dialogueQueue.Count > 0)
            {
                var nextDialogue = dialogueQueue.Dequeue();
                currentDialogue = nextDialogue.Text;
                charDisplayDelay = nextDialogue.Delay; // Use specific delay for this dialogue
                charIndex = 0;
                timeLeft = 5 * 60; // Reset timeLeft for new dialogue
            }
            else
            {
                timeLeft = 0; // End the dialogue if there are no more sequences
            }
        }

        public void PushAnchor(ref Vector2 positionAnchorBottom) => positionAnchorBottom.Y -= 50f * Opacity;

        public static NPCDialogueBox CreateNewDialogueSequence(params (string Text, int Delay)[] dialogues)
        {
            NPCDialogueBox notification = new NPCDialogueBox();
            foreach (var dialogue in dialogues)
            {
                notification.AddDialogue(dialogue.Text, dialogue.Delay);
            }
            return notification;
        }
    }
}
