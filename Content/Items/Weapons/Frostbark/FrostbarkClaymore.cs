using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ReverieMod.Content.Items.Weapons.Frostbark
{
    public class FrostbarkClaymore : ModItem
    {
        public override string Texture => Assets.Weapons.Viking + Name;
        public override void SetDefaults() 
        {
            Item.DamageType = DamageClass.Melee;
            Item.damage = 11;
            Item.crit = 5;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.useTime = Item.useAnimation = 21;
            Item.autoReuse = Item.useTurn  = true;           

            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(silver: 2);

            Item.width = Item.height = 34;
            //Item.Size = new Vector2(30, 40);
        }
        /* public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                // todo: the code
            }
            return base.UseItem(player);
        }*/
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.IceBlock, 20);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 4);
            recipe.AddIngredient(ItemID.BorealWood, 16);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
