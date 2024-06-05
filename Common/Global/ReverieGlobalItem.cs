using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReverieMod.Common.Players;

namespace ReverieMod.Common.Global
{
    public class ReverieGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => true;

        public bool Shovel;
        public int digPower;
        public int radius = 2;

        public static int GetDigPower(int shovel)
        {
            Item i = ModContent.GetModItem(shovel).Item;
            return i.GetGlobalItem<ReverieGlobalItem>().digPower;
        }
        public static int GetShovelRadius(int shovel)
        {
            Item i = ModContent.GetModItem(shovel).Item;
            return i.GetGlobalItem<ReverieGlobalItem>().radius;
        }
    }
}