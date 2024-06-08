using ReverieMod.Common.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace ReverieMod.Content.Items
{
    public class Microlith : ModItem
    {
        public override string Texture => Assets.Items.Accessory + Name;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.width = Item.height = 30;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ReveriePlayer reverie = player.GetModPlayer<ReveriePlayer>();
            reverie.Harvest = true;
        }
    }
}
