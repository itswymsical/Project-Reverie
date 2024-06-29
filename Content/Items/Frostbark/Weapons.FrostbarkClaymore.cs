using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace ReverieMod.Content.Items.Frostbark
{
    public class FrostbarkClaymore : ModItem
    {
        public override string Texture => Assets.Items.Frostbark + Name;
        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.DamageType = DamageClass.Melee;
            Item.width = Item.height = 40;
            Item.useTime = 29; 
            Item.useAnimation = 29;
            Item.knockBack = 1.7f;
            Item.value = Item.sellPrice(silver: 18);
            Item.rare = ItemRarityID.Blue;
            Item.useTurn = false;
            Item.useStyle = ItemUseStyleID.Swing;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(5))
                target.AddBuff(BuffID.Frostburn, 90);
            
        }
    }
}
