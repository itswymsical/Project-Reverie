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
using Terraria.Audio;
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
            var dialogues = new (string Text, int Delay)[]
            {
                ($"Hey, {player.name}!", 3),
                ($"Nice to meet you, I'm you're guide.", 2),
                ("Although i'm only an apprentice guide, I'm more than qualified to help you learn everything about Terraria.", 2),
                ("You've been out for a while now, hehe.", 4),
                ("Anyways...", 2),
                ("I bet you're wondering what to do from here.", 3),
                ("Come talk to me, I'll answer any questions you have.", 2)
            };
            NPCDialogueBox dialogue = NPCDialogueBox.CreateNewDialogueSequence(dialogues);
            dialogue.npcName = "Guide";
            dialogue.currentDialogue = "???"; // currentDialogue is ALWAYS the intial dialogue.
            dialogue.iconTexture = ModContent.Request<Texture2D>("ReverieMod/Assets/Textures/UI/DialoguePortraits/Guide");
            dialogue.color = Color.LightBlue;
            dialogue.characterSound = SoundID.MenuOpen;
            InGameNotificationsTracker.AddNotification(dialogue);      
            
            return true;
        }
    }
}
