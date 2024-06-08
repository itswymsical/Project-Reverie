﻿using ReverieMod.Content.Items.ClassTypes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ReverieMod.Content.Items.Shovels
{
    public class IronShovel : ShovelItem
    {
        public override string Texture => Assets.Items.Shovels + Name;
        public override void SetDefaults()
        {
            DiggingPower(38);
            Item.DamageType = DamageClass.Melee;
            Item.damage = 4;
            Item.useTime = Item.useAnimation = 18;
            Item.width = Item.height = 32;
            Item.knockBack = 5;

            Item.autoReuse = Item.useTurn = true;

            Item.value = Item.sellPrice(silver: 4);

            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item18;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 4);
            recipe.AddIngredient(ItemID.IronBar, 9);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}