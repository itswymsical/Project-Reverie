using Terraria;
using ReverieMod.Common.Global;
using Terraria.ID;

namespace ReverieMod.Helpers
{
    internal static partial class Helper
    {
        public static bool IsShovel(this Item item) => item.GetGlobalItem<ReverieGlobalItem>().Shovel;
        public static bool IsPickaxe(this Item item) => item.pick > 0;
        public static bool IsShiny(this Item item) => (item.Name.Contains("Ore"));
    }
}