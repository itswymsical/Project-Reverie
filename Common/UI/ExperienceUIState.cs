using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using ReverieMod.Common.Players;
using ReverieMod.Common.Systems;
using Terraria.Localization;
using ReLogic.Content;
using Terraria.ID;

namespace ReverieMod.Common.UI
{
    internal class ExperienceTracker : UIState
    {
        private UIText experienceText;
        private UIText levelText;
        private UIText nextLevelText;
        private UIImage icon;
        private Asset<Texture2D> iconTexture;

        public override void OnInitialize()
        {
            // Load the icon texture
            iconTexture = ModContent.Request<Texture2D>("ReverieMod/Assets/Textures/UI/StatBar");

            // Create an image element for the icon
            icon = new UIImage(iconTexture);
            icon.Left.Set(iconTexture.Width() - 90f, 0f);
            icon.Top.Set(275f, 0f);

            icon.Width.Set(iconTexture.Width(), 0f);
            icon.Height.Set(iconTexture.Height(), 0f);
            Append(icon);

            // Create a text element for level
            levelText = new UIText("Lvl: 1");
            levelText.Left.Set(iconTexture.Width() - 60f, 0f); // Adjust position based on icon
            levelText.Top.Set(iconTexture.Height() * 9.7f, 0f);
            levelText.ShadowColor = Color.Black;
            Append(levelText);

            // Create a text element for experience points
            experienceText = new UIText("XP: 0");
            experienceText.Left.Set(iconTexture.Width() - 88f, 0f); // Adjust position based on icon
            experienceText.Top.Set(iconTexture.Height() * 10.6f, 0f);
            experienceText.ShadowColor = Color.Black;
            Append(experienceText);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.playerInventory)
            {
                icon.Left.Set(iconTexture.Width() - 90f, 0f);
                icon.Top.Set(265f, 0f);

                levelText.Left.Set(iconTexture.Width() - 60f, 0f); // Adjust position based on icon
                levelText.Top.Set(iconTexture.Height() * 9.7f, 0f);
                levelText.ShadowColor = Color.Black;

                experienceText.Left.Set(iconTexture.Width() - 88f, 0f); // Adjust position based on icon
                experienceText.Top.Set(iconTexture.Height() * 10.6f, 0f);
                experienceText.ShadowColor = Color.Black;
            }
            else
            {
                return;
            }

            base.Draw(spriteBatch);
        }
        public override void Update(GameTime gameTime)
        {
            Player player = Main.LocalPlayer;
            LevelNotification icon = new LevelNotification();
            ExperiencePlayer modPlayer = player.GetModPlayer<ExperiencePlayer>();
            int nextLevelXP = ExperiencePlayer.GetNextExperienceThreshold(modPlayer.experienceLevel);
            experienceText.SetText($"XP: {modPlayer.experienceValue} / {nextLevelXP}");
            levelText.SetText($"Lvl: {modPlayer.experienceLevel}");
            base.Update(gameTime);

        }
    }
    [Autoload(Side = ModSide.Client)]
    internal class ReverieUISystem : ModSystem
    {
        private UserInterface ExperienceTrackerUserInterface;

        internal ExperienceTracker ExperienceTracker;

        public static LocalizedText ExperienceText { get; private set; }

        public override void Load()
        {
            ExperienceTracker = new();
            ExperienceTrackerUserInterface = new();
            ExperienceTrackerUserInterface.SetState(ExperienceTracker);

            string category = "UI";
            ExperienceText ??= Mod.GetLocalization($"{category}.ExperienceTracker");
        }

        public override void UpdateUI(GameTime gameTime)
        {
            ExperienceTrackerUserInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "ReverieMod: Experience Level",
                    delegate
                    {
                        ExperienceTrackerUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}