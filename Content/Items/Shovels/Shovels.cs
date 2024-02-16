using ReverieMod;
using ReverieMod.Common.Global;
using ReverieMod.Common.Players;
using ReverieMod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace ReverieMod.Content.Items.Shovels
{

    public abstract class ShovelItem : ModItem
    {
        // Constants
        protected const int DefaultShovelRange = 5;

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
            if (player.Distance(Main.MouseWorld) < 16 * rangeinBlocks)
                player.GetModPlayer<ReveriePlayer_Shovel>().DigBlocks((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y);
        }
        public override bool? UseItem(Player player)
        {
            UseShovel(player, DefaultShovelRange);
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

            PrefixID.Large,
            PrefixID.Tiny,

            PrefixID.Bulky,
            PrefixID.Heavy,

            PrefixID.Damaged,
            PrefixID.Broken,

            PrefixID.Unhappy,
            PrefixID.Nimble,
            PrefixID.Dull,
            PrefixID.Awkward
        });

        public class WoodShovel : ShovelItem
        {
            public override string Texture => Assets.Items.Shovels + Name;
            public override void SetDefaults()
            {
                DiggingPower(28);
                Item.DamageType = DamageClass.Melee;               
                Item.damage = 2;
                Item.useTime = Item.useAnimation = 25;
                Item.width = Item.height = 32;

                Item.autoReuse = Item.useTurn = true;

                Item.value = Item.sellPrice(copper: 5);             
                Item.useStyle = ItemUseStyleID.Swing;
                Item.UseSound = SoundID.Item18;
            }
            public override void AddRecipes()
            {
                Recipe recipe = CreateRecipe();
                recipe.AddIngredient(ItemID.Wood, 20);
                recipe.AddTile(TileID.WorkBenches);
                recipe.Register();
            }
        }
        public class CopperShovel : ShovelItem
        {
            public override string Texture => Assets.Items.Shovels + Name;
            public override void SetDefaults()
            {
                DiggingPower(32);
                Item.DamageType = DamageClass.Melee;
                Item.damage = 3;
                Item.useTime = Item.useAnimation = 25;
                Item.width = Item.height = 32;

                Item.autoReuse = Item.useTurn = true;

                Item.value = Item.sellPrice(copper: 18);

                Item.useStyle = ItemUseStyleID.Swing;
                Item.UseSound = SoundID.Item18;
            }
            public override void AddRecipes()
            {
                Recipe recipe = CreateRecipe();
                recipe.AddIngredient(ItemID.Wood, 8);
                recipe.AddIngredient(ItemID.CopperBar, 10);
                recipe.AddTile(TileID.Anvils);
                recipe.Register();
            }
        }
        public class TinShovel : ShovelItem
        {
            public override string Texture => Assets.Items.Shovels + Name;
            public override void SetDefaults()
            {
                DiggingPower(34);

                Item.DamageType = DamageClass.Melee;
                Item.damage = 2;
                Item.useTime = Item.useAnimation = 25;
                Item.width = Item.height = 32;

                Item.autoReuse = Item.useTurn = true;

                Item.value = Item.sellPrice(copper: 19);

                Item.useStyle = ItemUseStyleID.Swing;
                Item.UseSound = SoundID.Item18;
            }
            public override void AddRecipes()
            {
                Recipe recipe = CreateRecipe();
                recipe.AddIngredient(ItemID.Wood, 8);
                recipe.AddIngredient(ItemID.TinBar, 10);
                recipe.AddTile(TileID.Anvils);
                recipe.Register();
            }
        }
        public class IronShovel : ShovelItem
        {
            public override string Texture => Assets.Items.Shovels + Name;
            public override void SetDefaults()
            {
                DiggingPower(36);

                Item.DamageType = DamageClass.Melee;
                Item.damage = 2;
                Item.useTime = Item.useAnimation = 23;
                Item.width = Item.height = 32;

                Item.autoReuse = Item.useTurn = true;

                Item.value = Item.sellPrice(copper: 26);

                Item.useStyle = ItemUseStyleID.Swing;
                Item.UseSound = SoundID.Item18;
            }
            public override void AddRecipes()
            {
                Recipe recipe = CreateRecipe();
                recipe.AddIngredient(ItemID.Wood, 8);
                recipe.AddIngredient(ItemID.IronBar, 10);
                recipe.AddTile(TileID.Anvils);
                recipe.Register();
            }
        }
        public class LeadShovel : ShovelItem
        {
            public override string Texture => Assets.Items.Shovels + Name;
            public override void SetDefaults()
            {
                DiggingPower(38);

                Item.DamageType = DamageClass.Melee;
                Item.damage = 3;
                Item.useTime = Item.useAnimation = 23;
                Item.width = Item.height = 32;

                Item.autoReuse = Item.useTurn = true;

                Item.value = Item.sellPrice(copper: 32);

                Item.useStyle = ItemUseStyleID.Swing;
                Item.UseSound = SoundID.Item18;
            }
            public override void AddRecipes()
            {
                Recipe recipe = CreateRecipe();
                recipe.AddIngredient(ItemID.Wood, 8);
                recipe.AddIngredient(ItemID.LeadBar, 10);
                recipe.AddTile(TileID.Anvils);
                recipe.Register();
            }
        }
        public class SilverShovel : ShovelItem
        {
            public override string Texture => Assets.Items.Shovels + Name;
            public override void SetDefaults()
            {
                DiggingPower(42);

                Item.DamageType = DamageClass.Melee;
                Item.damage = 3;
                Item.useTime = Item.useAnimation = 21;
                Item.width = Item.height = 32;

                Item.autoReuse = Item.useTurn = true;

                Item.value = Item.sellPrice(silver: 2);

                Item.useStyle = ItemUseStyleID.Swing;
                Item.UseSound = SoundID.Item18;
            }
            public override void AddRecipes()
            {
                Recipe recipe = CreateRecipe();
                recipe.AddIngredient(ItemID.Wood, 8);
                recipe.AddIngredient(ItemID.SilverBar, 10);
                recipe.AddTile(TileID.Anvils);
                recipe.Register();
            }
        }
        public class TungstenShovel : ShovelItem
        {
            public override string Texture => Assets.Items.Shovels + Name;
            public override void SetDefaults()
            {
                DiggingPower(48);

                Item.DamageType = DamageClass.Melee;
                Item.damage = 4;
                Item.useTime = Item.useAnimation = 20;
                Item.width = Item.height = 32;

                Item.autoReuse = Item.useTurn = true;

                Item.value = Item.sellPrice(silver: 2);

                Item.useStyle = ItemUseStyleID.Swing;
                Item.UseSound = SoundID.Item18;
            }
            public override void AddRecipes()
            {
                Recipe recipe = CreateRecipe();
                recipe.AddIngredient(ItemID.Wood, 8);
                recipe.AddIngredient(ItemID.TungstenBar, 10);
                recipe.AddTile(TileID.Anvils);
                recipe.Register();
            }
        }
        public class GoldShovel : ShovelItem
        {
            public override string Texture => Assets.Items.Shovels + Name;
            public override void SetDefaults()
            {
                DiggingPower(50);

                Item.DamageType = DamageClass.Melee;
                Item.damage = 5;
                Item.useTime = Item.useAnimation = 19;
                Item.width = Item.height = 32;

                Item.autoReuse = Item.useTurn = true;

                Item.value = Item.sellPrice(silver: 10);

                Item.useStyle = ItemUseStyleID.Swing;
                Item.UseSound = SoundID.Item18;
            }
            public override void AddRecipes()
            {
                Recipe recipe = CreateRecipe();
                recipe.AddIngredient(ItemID.Wood, 8);
                recipe.AddIngredient(ItemID.GoldBar, 10);
                recipe.AddTile(TileID.Anvils);
                recipe.Register();
            }
        }
        public class PlatinumShovel : ShovelItem
        {
            public override string Texture => Assets.Items.Shovels + "PlatinumShovel";
            public override void SetDefaults()
            {
                DiggingPower(58);

                Item.DamageType = DamageClass.Melee;
                Item.damage = 5;
                Item.useTime = Item.useAnimation = 18;
                Item.width = Item.height = 32;

                Item.autoReuse = Item.useTurn = true;

                Item.value = Item.sellPrice(silver: 13);

                Item.useStyle = ItemUseStyleID.Swing;
                Item.UseSound = SoundID.Item18;
            }
            public override void AddRecipes()
            {
                Recipe recipe = CreateRecipe();
                recipe.AddIngredient(ItemID.Wood, 8);
                recipe.AddIngredient(ItemID.PlatinumBar, 10);
                recipe.AddTile(TileID.Anvils);
                recipe.Register();
            }
        }
    }
}