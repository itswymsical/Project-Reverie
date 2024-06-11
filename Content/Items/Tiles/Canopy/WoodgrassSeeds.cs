using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using ReverieMod.Content.Tiles.Canopy;

namespace ReverieMod.Content.Items.Tiles.Canopy
{
    public class WoodgrassSeeds : ModItem
    {
        public override string Texture => Assets.Items.CanopyTiles + Name;
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Woodgrass>());
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 2);
        }
    }
}