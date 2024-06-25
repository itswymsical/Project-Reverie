using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReverieMod.Common.Players;
using ReverieMod.Common.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace ReverieMod.Content.Items
{
    public class DialogueTest : ModItem
    {
        public override string Texture => Assets.PlaceholderTexture;
        public override void SetDefaults()
        {
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.width = Item.height = 30;
        }
        public override bool? UseItem(Player player)
        {
            WorldNPCDialogueNotification sophie = new WorldNPCDialogueNotification();
            sophie.text = "Hey there! I see you over there slaughtering slimes, how about you come kill this fella I got over here?";
            sophie.icon = ModContent.Request<Texture2D>("ReverieMod/Assets/Textures/UI/DialoguePortraits/Sophie");
            InGameNotificationsTracker.AddNotification(sophie);      
            
            return true;
        }
    }
}
