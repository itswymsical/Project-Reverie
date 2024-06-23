using Microsoft.Xna.Framework;
using ReverieMod.Common.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ReverieMod.Content.Items
{
    public class ExperienceReader : ModItem
    {
        public override string Texture => Assets.PlaceholderTexture;
        public override void SetDefaults()
        {
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.width = Item.height = 30;
        }
        public override bool? UseItem(Player player)
        {
            ExperiencePlayer experience = player.GetModPlayer<ExperiencePlayer>();
            Main.NewText($"Level {experience.experienceLevel}, {experience.experienceValue} XP, {experience.skillPoints} Skill points.", Color.Green);
            return true;
        }
    }
}
