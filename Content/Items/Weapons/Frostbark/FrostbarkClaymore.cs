using Microsoft.Xna.Framework;
using ReverieMod.Content.Projectiles.Melee;
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

            Item.UseSound = SoundID.Item1;

            Item.useTime = Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic =
                Item.noMelee =
                Item.channel = true;

            Item.autoReuse = false;

            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(silver: 2);

            Item.width = Item.height = 34;

            Item.shootSpeed = 6f;
            Item.shoot = ModContent.ProjectileType<FrostbarkClaymoreProjectile>();
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int direction = player.direction;
            if (direction == 0)
            {
                direction = 1;
            }

            Projectile projectile = Projectile.NewProjectileDirect(default, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
            projectile.netUpdate = true;
            projectile.direction = direction;

            return false;
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
