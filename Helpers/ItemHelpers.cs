using Terraria;
using Trelamium.Common.Global;

namespace Trelamium.Helpers
{
    internal static partial class Helper
    {
        public static bool IsShovel(this Item item) => item.GetGlobalItem<TGlobalItem>().Shovel;
        public static bool IsPickaxe(this Item item) => item.pick > 0;
        
    }
}