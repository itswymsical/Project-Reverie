using Humanizer.Localisation;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReverieMod.Common.Global;
using Terraria.GameInput;
using Terraria.GameContent;
using MonoMod.Cil;

namespace ReverieMod.Common.Players
{
    public partial class ShovelPlayer : ModPlayer
    {
        public void DigBlocks(int i, int j)
        {
            for (int num = -1; num < 2; num++)
            {
                if (num != 0)
                {
                    BreakTileIfValid(i / 16 + num, j / 16);
                    BreakTileIfValid(i / 16, j / 16 + num);
                }
            }
            BreakTileIfValid(i / 16, j / 16);
        }
        
        private void BreakTileIfValid(int i, int j)
        {
            int digTile = Player.HeldItem.GetGlobalItem<ReverieGlobalItem>().digPower;

            if (!IsExcludedTile(i, j))
            {
                Player.PickTile(i, j, digTile);
            }
        }
        
        private bool IsExcludedTile(int i, int j)
        {
            int tileType = Main.tile[i, j].TileType;

            // Add more excluded tile types if needed
            return Main.tileAxe[tileType] || Main.tileHammer[tileType];
        }
        private bool IsSoftTile(int i, int j)
        {
            int tileType = Main.tile[i, j].TileType;
            return TileID.Sets.CanBeDugByShovel[tileType];
        }
    }
}