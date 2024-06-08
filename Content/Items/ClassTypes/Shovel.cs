using ReverieMod.Common.Global;
using ReverieMod.Common.Players;
using ReverieMod.Helpers;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Biomes;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static ReverieMod.Assets;

namespace ReverieMod.Content.Items.ClassTypes
{

    public abstract class ShovelItem : ModItem
    {
        public int ShovelRange = 5;
        protected override bool CloneNewInstances => false;
        public void DiggingPower(int digPower)
        {
            Item.GetGlobalItem<ReverieGlobalItem>().digPower = digPower;
            //Item.GetGlobalItem<ReverieGlobalItem>().radius = 6;
            Item.pick = digPower;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine power = new TooltipLine(Mod, "ReverieMod:Shovel Power", $"{Item.GetGlobalItem<ReverieGlobalItem>().digPower}% digging power");
            tooltips.Add(power);

            TooltipLine message = new TooltipLine(Mod, "ReverieMod:Shovel Info", "Stronger on soft tiles" +
                "\nSmart cursor utilizes 1x1 digging");
            tooltips.Add(message);

            if (Item.pick <= 0)
                return;

            foreach (TooltipLine line in tooltips.Where(line => line.Name == "PickPower"))
                line.Hide();

            /*
            if (ModLoader.HasMod("OreExcavator")) //lol
            {
                TooltipLine veinMine = new TooltipLine(Mod, "ReverieMod: veinMiner Warning", $"[c/ff0000:(BUG)] Shovels are scuffed with OE. Wiggle your cursor while excavating!");
                tooltips.Add(veinMine);
            }*/      
        }
        public static void UseShovel(Player player, int rangeinBlocks)
        {
            //Vector2 pos = new Vector2(Main.MouseWorld.ToTileCoordinates16().X, Main.MouseWorld.ToTileCoordinates16().Y);
            if (player.Distance(Main.MouseWorld) < 16 * rangeinBlocks)
                player.GetModPlayer<ShovelPlayer>().DigBlocks((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y);
        }
        public override bool? UseItem(Player player)
        {
            if (!Main.SmartCursorWanted)
            {
                UseShovel(player, ShovelRange + Item.tileBoost);
            }
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