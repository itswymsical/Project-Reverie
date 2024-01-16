using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Trelamium.Content.Tiles.DruidsGarden;

namespace Trelamium.Content.Items.Tiles
{
    public class Loam : ModItem
    {
        public override string Texture => Assets.Items.Tiles + "Loam";
        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.White;
            
            Item.width = Item.height = 24;
            Item.useAnimation = Item.useTime = 20;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = Item.autoReuse = Item.consumable = true;

            Item.value = Item.sellPrice(copper: 0);
            Item.createTile = ModContent.TileType<LoamTile>();
        }
    }
}