using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReverieMod.Content.Tiles.DruidsGarden;

namespace ReverieMod.Content.Items.Tiles
{
    public class Loam : ModItem
    {
        public override string Texture => Assets.Items.Tiles + "Loam";
        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.White;
            
            Item.width = Item.height = 24;
            Item.useAnimation = Item.useTime = 14;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = Item.autoReuse = Item.consumable = true;

            Item.value = Item.sellPrice(copper: 0);
            Item.createTile = ModContent.TileType<LoamTile>();
        }
    }
}