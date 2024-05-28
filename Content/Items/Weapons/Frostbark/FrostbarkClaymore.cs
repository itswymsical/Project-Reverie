using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ReverieMod.Content.Items.Weapons.Frostbark
{
    public class FrostbarkClaymore : ModItem
    {
        public override string Texture => Assets.Weapons.Frostbark + Name;
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

            Item.width = Item.height = 55;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(10))
            {
                target.AddBuff(BuffID.Frostburn, 90); //1.5 seconds, 60 ticks per sec
            }
        }

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
