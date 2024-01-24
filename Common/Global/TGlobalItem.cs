using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using EmpyreanDreamscape.Common.Players;

namespace EmpyreanDreamscape.Common.Global
{
    public class TGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => true;

        public bool Shovel;
        public int digPower;
        public int radius = 2;
        public override void ExtractinatorUse(int extractType, int extractinatorBlockType, ref int resultType, ref int resultStack)
        {
            /*
            if (extractType == ItemID.DesertFossil)
            {
                if (Main.rand.NextFloat() < 0.005f)
                {
                    //resultType = ModContent.ItemType<Microlith>();
                }
            }*/
            base.ExtractinatorUse(extractType, extractinatorBlockType, ref resultType, ref resultStack);
        }

        public static int GetDigPower(int shovel)
        {
            Item i = ModContent.GetModItem(shovel).Item;
            return i.GetGlobalItem<TGlobalItem>().digPower;
        }
        public static int GetShovelRadius(int shovel)
        {
            Item i = ModContent.GetModItem(shovel).Item;
            return i.GetGlobalItem<TGlobalItem>().radius;
        }
        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            ArmorSetPlayer armorSetPlayer = Main.LocalPlayer.GetModPlayer<ArmorSetPlayer>();
            if (armorSetPlayer.vikingSet && item.CountsAsClass(DamageClass.Melee))
            {
                if (Main.rand.NextBool(12))
                    target.AddBuff(BuffID.Frostburn, Main.rand.Next(60, 140));
            }
        }
    }
}