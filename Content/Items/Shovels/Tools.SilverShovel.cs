using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ReverieMod.Content.Items.Shovels
{
    public class SilverShovel : ShovelItem
    {
        public override string Texture => Assets.Items.Shovels + Name;
        public override void SetDefaults()
        {
            DiggingPower(45);
            Item.DamageType = DamageClass.Melee;
            Item.damage = 5;
            Item.useTime = Item.useAnimation = 16;
            Item.width = Item.height = 32;
            Item.knockBack = 5;

            Item.autoReuse = Item.useTurn = true;

            Item.value = Item.sellPrice(silver: 10);

            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item18;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 4);
            recipe.AddIngredient(ItemID.SilverBar, 9);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}