using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReverieMod;
using ReverieMod.Common.Global;
using ReverieMod.Common.Players;
using ReverieMod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terraria.ModLoader.ModContent;

namespace ReverieMod.Content.Items.Shovels
{

    public abstract class ShovelItem : ModItem
    {
        public int ShovelRange = 5;
        protected override bool CloneNewInstances => false;

        public void DiggingPower(int digPower)
        {
            Item.GetGlobalItem<ReverieGlobalItem>().digPower = digPower;
            Item.GetGlobalItem<ReverieGlobalItem>().radius = 6;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tooltipLine = new TooltipLine(Mod, "ReverieMod:Digging Power", $"{Item.GetGlobalItem<ReverieGlobalItem>().digPower}% digging power");
            tooltips.Add(tooltipLine);
        }

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

        public static void UseShovel(Player player, int rangeinBlocks)
        {
            //Vector2 pos = new Vector2(Main.MouseWorld.ToTileCoordinates16().X, Main.MouseWorld.ToTileCoordinates16().Y);
            if (player.Distance(Main.MouseWorld) < 16 * rangeinBlocks)
                player.GetModPlayer<ShovelPlayer>().DigBlocks((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y);
        }

        public override bool? UseItem(Player player)
        {
            UseShovel(player, ShovelRange);
            return true;
        }

        public override int ChoosePrefix(UnifiedRandom rand) => rand.Next(new int[]
        {
            PrefixID.Agile,
            PrefixID.Quick,
            PrefixID.Light,

            PrefixID.Slow,
            PrefixID.Sluggish,
            PrefixID.Lazy,

            PrefixID.Bulky,
            PrefixID.Heavy,

            PrefixID.Damaged,
            PrefixID.Broken,

            PrefixID.Unhappy,
            PrefixID.Nimble,
            PrefixID.Dull
        });
    }
}