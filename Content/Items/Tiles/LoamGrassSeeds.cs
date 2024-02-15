using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReverieMod.Content.Tiles.DruidsGarden;

namespace ReverieMod.Content.Items.Tiles
{
    public class LoamGrassSeeds : ModItem
    {
        public override string Texture => Assets.Items.Tiles + "LoamGrassSeeds";
        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.White;

            Item.width = Item.height = 24;
            Item.useAnimation = Item.useTime = 15;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = Item.autoReuse = Item.consumable = true;

            Item.value = Item.sellPrice(copper: 8);
        }


        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
                return false;

            Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
            if (tile.HasTile && tile.TileType == ModContent.TileType<LoamTile>() && player.WithinRange(new Microsoft.Xna.Framework.Vector2(Player.tileTargetX, Player.tileTargetY), default))
            {
                WorldGen.PlaceTile(Player.tileTargetX, Player.tileTargetY, ModContent.TileType<LoamTileGrass>(), forced: true);
                player.inventory[player.selectedItem].stack--;
            }
            return true;
        }
    }
}