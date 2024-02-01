using System;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using EmpyreanDreamscape.Common.Systems.Subworlds;

namespace EmpyreanDreamscape.Content.Items
{
    public class TestItem : ModItem
    {
        public override string Texture => Assets.PlaceholderTexture;
        public override void SetDefaults()
        {
            Item.maxStack = 1;
            Item.width = 34;
            Item.height = 38;
            Item.rare = 12;
            Item.useStyle = 4;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.UseSound = SoundID.Item1;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                SubworldSystem.Enter<TestSubworld>();

            if (SubworldSystem.IsActive<TestSubworld>())
                SubworldSystem.Exit();

            return true;
        }
    }
}
