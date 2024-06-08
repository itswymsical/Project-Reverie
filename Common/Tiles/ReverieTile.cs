using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using ReverieMod.Helpers;
using ReverieMod.Common.Players;
using Terraria.ObjectData;

namespace ReverieMod.Common.Tiles
{
	public class ReverieTile : GlobalTile
	{
		public bool placedByPlayer;
		public override void FloorVisuals(int type, Player player)
		{
			player.GetModPlayer<ReveriePlayer>().onSand =
				(TileID.Sets.Conversion.Sand[type] || TileID.Sets.Conversion.Sandstone[type] || TileID.Sets.Conversion.HardenedSand[type]);
		}
        //This is gonna look really ugly, i had intended to make this a few simple and cohesive lines of code then i remembered how shit tile code is
        public override void Drop(int i, int j, int type)
        {
            Player player = Main.LocalPlayer;
            var modPlayer = player.GetModPlayer<ReveriePlayer>();

            int amount = Main.rand.Next(1, 3);
            #region Fortune III shitcode
            if (modPlayer.Harvest && player.HeldItem.IsShovel()
            || modPlayer.Harvest && player.HeldItem.IsPickaxe())
            {
                if (Main.rand.NextBool(8))
                {
                    if (type == TileID.Copper)
                    {
                        player.QuickSpawnItem(default, ItemID.CopperOre, amount);
                        CombatText.NewText(player.Hitbox, new Color(255, 146, 97), "Copper +" + amount);
                    }
                    if (type == TileID.Tin)
                    {
                        player.QuickSpawnItem(default, ItemID.TinOre, amount);
                        CombatText.NewText(player.Hitbox, new Color(229, 220, 163), "Tin +" + amount);
                    }
                    if (type == TileID.Iron)
                    {
                        player.QuickSpawnItem(default, ItemID.IronOre, amount);
                        CombatText.NewText(player.Hitbox, new Color(105, 105, 105), "Iron +" + amount);
                    }
                    if (type == TileID.Lead)
                    {
                        player.QuickSpawnItem(default, ItemID.LeadOre, amount);
                        CombatText.NewText(player.Hitbox, new Color(104, 141, 151), "Lead +" + amount);
                    }
                    if (type == TileID.Silver)
                    {
                        player.QuickSpawnItem(default, ItemID.SilverOre, amount);
                        CombatText.NewText(player.Hitbox, new Color(162, 174, 175), "Silver +" + amount);
                    }
                    if (type == TileID.Tungsten)
                    {
                        player.QuickSpawnItem(default, ItemID.TungstenOre, amount);
                        CombatText.NewText(player.Hitbox, new Color(76, 119, 73), "Tungsten +" + amount);
                    }
                    if (type == TileID.Gold)
                    {
                        player.QuickSpawnItem(default, ItemID.GoldOre, amount);
                        CombatText.NewText(player.Hitbox, new Color(234, 208, 138), "Gold +" + amount);
                    }
                    if (type == TileID.Platinum)
                    {
                        player.QuickSpawnItem(default, ItemID.PlatinumOre, amount);
                        CombatText.NewText(player.Hitbox, new Color(182, 195, 218), "Platinum +" + amount);
                    }
                } //Pre-Hardmode Tier
                if (Main.rand.NextBool(10))
                {
                    if (type == TileID.Amethyst)
                    {
                        player.QuickSpawnItem(default, ItemID.Amethyst, amount);
                        CombatText.NewText(player.Hitbox, new Color(166, 0, 237), "Amethyst +" + amount);
                    }
                    if (type == TileID.Topaz)
                    {
                        player.QuickSpawnItem(default, ItemID.Topaz, amount);
                        CombatText.NewText(player.Hitbox, new Color(255, 199, 0), "Topaz +" + amount);
                    }
                    if (type == TileID.Sapphire)
                    {
                        player.QuickSpawnItem(default, ItemID.Sapphire, amount);
                        CombatText.NewText(player.Hitbox, new Color(16, 148, 235), "Sapphire +" + amount);
                    }
                    if (type == TileID.Emerald)
                    {
                        player.QuickSpawnItem(default, ItemID.Emerald, amount);
                        CombatText.NewText(player.Hitbox, new Color(128, 247, 203), "Emerald  +" + amount);
                    }
                    if (type == TileID.Ruby)
                    {
                        player.QuickSpawnItem(default, ItemID.Ruby, amount);
                        CombatText.NewText(player.Hitbox, new Color(243, 115, 114), "Ruby +" + amount);
                    }
                    if (type == TileID.Diamond)
                    {
                        player.QuickSpawnItem(default, ItemID.Diamond, amount);
                        CombatText.NewText(player.Hitbox, new Color(224, 231, 239), "Diamond +" + amount);
                    }
                    if (type == TileID.Meteorite)
                    {
                        player.QuickSpawnItem(default, ItemID.Meteorite, amount);
                        CombatText.NewText(player.Hitbox, new Color(228, 162, 172), "Meteorite +" + amount);
                    }
                    if (type == TileID.Demonite)
                    {
                        player.QuickSpawnItem(default, ItemID.DemoniteOre, amount);
                        CombatText.NewText(player.Hitbox, new Color(164, 151, 220), "Demonite +" + amount);
                    }
                    if (type == TileID.Crimtane)
                    {
                        player.QuickSpawnItem(default, ItemID.CrimtaneOre, amount);
                        CombatText.NewText(player.Hitbox, new Color(217, 56, 61), "Crimtane +" + amount);
                    }
                } //Gem & Misc Tier
                if (Main.rand.NextBool(11))
                {
                    if (type == TileID.Cobalt)
                    {
                        player.QuickSpawnItem(default, ItemID.CobaltOre, amount);
                        CombatText.NewText(player.Hitbox, new Color(27, 141, 235), "Cobalt +" + amount);
                    }
                    if (type == TileID.Palladium)
                    {
                        player.QuickSpawnItem(default, ItemID.PalladiumOre, amount);
                        CombatText.NewText(player.Hitbox, new Color(239, 90, 49), "Palladium +" + amount);
                    }
                    if (type == TileID.Mythril)
                    {
                        player.QuickSpawnItem(default, ItemID.MythrilOre, amount);
                        CombatText.NewText(player.Hitbox, new Color(22, 119, 125), "Mythril +" + amount);
                    }
                    if (type == TileID.Orichalcum)
                    {
                        player.QuickSpawnItem(default, ItemID.OrichalcumOre, amount);
                        CombatText.NewText(player.Hitbox, new Color(249, 42, 243), "Orichalcum +" + amount);
                    }
                    if (type == TileID.Adamantite)
                    {
                        player.QuickSpawnItem(default, ItemID.AdamantiteOre, amount);
                        CombatText.NewText(player.Hitbox, new Color(210, 25, 28), "Adamantite +" + amount);
                    }
                    if (type == TileID.Titanium)
                    {
                        player.QuickSpawnItem(default, ItemID.TitaniumOre, amount);
                        CombatText.NewText(player.Hitbox, new Color(86, 84, 94), "Titanium Harvest +" + amount);
                    }
                    if (type == TileID.Chlorophyte)
                    {
                        player.QuickSpawnItem(default, ItemID.ChlorophyteOre, amount);
                        CombatText.NewText(player.Hitbox, new Color(161, 236, 0), "Chlorophyte +" + amount);
                    }
                }
            }
            #endregion
            return;
        }
    }
}
