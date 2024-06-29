using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReverieMod.Common.Players;
using ReverieMod.Common.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static ReverieMod.Common.UI.NPCData;

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
            var guideData = NPCDialogueIDHelper.GetNPCData(NPCDialogueID.Guide);
            var guide = NPC.FindFirstNPC(NPCID.Guide);
            var dialogues = new (string Text, int Delay, int TimeLeft, NPCData NpcData)[]
            {
                    ($"Hey, {player.name}!", 3, 300, guideData),
                    ($"Nice to meet you, I'm {Main.npc[guide].GivenName}.", 2, 300, guideData),
                    ($"I'll be your guide.", 2, 300, guideData),
                    ("Although I'm only an apprentice guide, I'm more than qualified to help you learn everything about Terraria.", 2, 300, guideData),
                    ("You've been out for a while now, hehe.", 4, 300, guideData),
                    ("Anyways...", 2, 300, guideData),
                    ("Let's get started with our first task!", 3, 300, guideData),
                    ("Come speak with me.", 2, 300, guideData)
            };

            NPCDialogueBox dialogue = NPCDialogueBox.CreateNewDialogueSequence(dialogues);
            InGameNotificationsTracker.AddNotification(dialogue);

            return true;
        }
    }
}
