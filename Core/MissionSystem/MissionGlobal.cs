using ReverieMod.Common.UI;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.RGB;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static ReverieMod.Common.UI.NPCData;

namespace ReverieMod.Core.MissionSystem
{
    public class MissionGlobal : GlobalNPC
    {
        public override void GetChat(NPC npc, ref string chat)
        {
            var missionManager = ModContent.GetInstance<MissionManager>();
            if (npc.type == NPCID.Guide)
            {
                string missionName = "Journey's Begin";
                string objectiveName = "Talk to the guide";
                if (missionManager.IsMissionActive(missionName))
                {
                    missionManager.UpdateMissionObjective("Guide", missionName, objectiveName, 1);
                    chat = "Great stuff! Here's 50 silver coins. The first thing we need is shelter, can I trust you to build housing units? Given you hold on to those coins, a merchant will arrive and move in.";

                    InGameNotificationsTracker.AddNotification(new MissionStatusIndicator());
                }
            }
        }
    }
    public class MissionModPlayer : ModPlayer
    {
        public override void OnEnterWorld()
        {
            var missionManager = ModContent.GetInstance<MissionManager>();
            GuideMissions.AddGuideMissions(missionManager);
            if (missionManager.IsMissionActive("Journey's Begin"))
            {
                var guideData = NPCDialogueIDHelper.GetNPCData(NPCDialogueID.Guide);
                var guide = NPC.FindFirstNPC(NPCID.Guide);
                var dialogues = new (string Text, int Delay, int TimeLeft, NPCData NpcData)[]
                {
                    ($"Hey, {Player.name}!", 3, 300, guideData),
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
            }
        }
    }
    public class ObjectiveTracker : ModPlayer
    {
        /*
        public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            var missionManager = ModContent.GetInstance<MissionManager>();
            missionManager.UpdateMissionObjective("Merchant", "Shopper", "Buy items", 1);
        }*/

    }
}