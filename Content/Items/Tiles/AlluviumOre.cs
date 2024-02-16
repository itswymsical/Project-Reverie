using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReverieMod.Content.Tiles.WoodlandCanopy;

namespace ReverieMod.Content.Items.Tiles
{
    public class AlluviumOre : ModItem
    {
        public override string Texture => Assets.Items.Tiles + Name;
        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Blue;

            Item.width = Item.height = 24;
            Item.useAnimation = Item.useTime = 15;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = Item.autoReuse = Item.consumable = true;

            Item.value = Item.sellPrice(copper: 38);
            Item.createTile = ModContent.TileType<AlluviumOreTile>();
        }
    }
}