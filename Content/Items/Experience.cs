using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ReverieMod.Content.Items
{
    public class Experience : ModItem
    {
        public override string Texture => Assets.PlaceholderTexture;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatShouldNotBeInInventory[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 12;
            Item.rare = ItemRarityID.White;
        }
    }
}
