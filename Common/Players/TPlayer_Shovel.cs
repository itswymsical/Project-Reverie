using Humanizer.Localisation;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReverieMod.Common.Global;

namespace ReverieMod.Common.Players
{
    public partial class TPlayer_Shovel : ModPlayer
    {
        public void DigBlocks(int x, int y)
        {
            for (int i = -1; i < 2; i++)
            {
                if (i != 0)
                {
                    BreakTileIfValid(x / 16 + i, y / 16);
                    BreakTileIfValid(x / 16, y / 16 + i);
                }
            }
            BreakTileIfValid(x / 16, y / 16);
        }

        private void BreakTileIfValid(int posX, int posY)
        {
            int digTile = Player.HeldItem.GetGlobalItem<TGlobalItem>().digPower;
            Item item = Main.item[Player.HeldItem.GetGlobalItem<TGlobalItem>().digPower];

            /*if (posX >= 0 && posX < Main.maxTilesX && posY >= 0 && posY < Main.maxTilesY)
            {*/
            if (!IsExcludedTile(posX, posY))
            {
                Player.PickTile(posX, posY, digTile);
            }
        }
        
        private bool IsExcludedTile(int posX, int posY)
        {
            int tileType = Main.tile[posX, posY].TileType;

            // Add more excluded tile types if needed
            return tileType == TileID.DemonAltar ||
                   tileType == TileID.Trees ||
                   tileType == TileID.PalmTree ||
                   tileType == TileID.MushroomTrees ||
                   tileType == TileID.ShadowOrbs ||
                   tileType == TileID.Cactus;
        }
    }
}