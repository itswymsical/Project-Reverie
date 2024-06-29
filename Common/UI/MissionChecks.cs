using System.IO;
using System.Security.AccessControl;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using static ReverieMod.Common.UI.NPCData;

namespace ReverieMod.Common.UI
{
    public class MissionChecks : ModSystem
    {
        #region GUIDE MISSIONS
        public static bool GUIDE_MISSIONS_WORLDSTART_ACTIVE = false;
        public static bool GUIDE_MISSIONS_WORLDSTART_COMPLETE = false;
        public static bool GUIDE_MISSIONS_HOUSE_ACTIVE = false;
        public static bool GUIDE_MISSIONS_HOUSE_COMPLETE = false;
        #endregion
        public static bool TEST_MISSIONS_JUMP_ACTIVE = false;
        public static bool TEST_MISSIONS_JUMP_COMPLETE = false;

        public static bool failed = false;

        public static string missionName = string.Empty;
        public static string missionObjective = string.Empty;
        public static int missionObjectiveValue = 0;
        public static int currentObjectiveValue = 0;
        public static MissionChecks Instance => ModContent.GetInstance<MissionChecks>();
        public override void ClearWorld()
        {
            #region GUIDE MISSIONS
            GUIDE_MISSIONS_WORLDSTART_ACTIVE = false;
            GUIDE_MISSIONS_WORLDSTART_COMPLETE = false;
            GUIDE_MISSIONS_HOUSE_ACTIVE = false;
            GUIDE_MISSIONS_HOUSE_COMPLETE = false;
            #endregion

            TEST_MISSIONS_JUMP_ACTIVE = false;
            TEST_MISSIONS_JUMP_COMPLETE = false;
            failed = false;
        }
        public override void SaveWorldData(TagCompound tag)
        {
            #region GUIDE MISSIONS
            if (GUIDE_MISSIONS_WORLDSTART_ACTIVE)
            {
                tag["GUIDE_MISSIONS_WORLDSTART_ACTIVE"] = true;

            }
            if (GUIDE_MISSIONS_WORLDSTART_COMPLETE)
            {
                tag["GUIDE_MISSIONS_WORLDSTART_COMPLETE"] = true;
            }
            if (GUIDE_MISSIONS_HOUSE_ACTIVE)
            {
                tag["GUIDE_MISSIONS_HOUSE_ACTIVE"] = true;
            }
            if (GUIDE_MISSIONS_HOUSE_COMPLETE)
            {
                tag["GUIDE_MISSIONS_HOUSE_COMPLETE"] = true;
            }
            #endregion

            if (TEST_MISSIONS_JUMP_ACTIVE)
            {
                tag["TEST_MISSIONS_JUMP_ACTIVE"] = true;
            }
            if (TEST_MISSIONS_JUMP_COMPLETE)
            {
                tag["TEST_MISSIONS_JUMP_ACTIVE"] = true;
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            #region GUIDE MISSION
            GUIDE_MISSIONS_WORLDSTART_ACTIVE = tag.ContainsKey("GUIDE_MISSIONS_WORLDSTART_ACTIVE");
            GUIDE_MISSIONS_WORLDSTART_COMPLETE = tag.ContainsKey("GUIDE_MISSIONS_WORLDSTART_COMPLETE");
            GUIDE_MISSIONS_HOUSE_ACTIVE = tag.ContainsKey("GUIDE_MISSIONS_HOUSE_ACTIVE");
            GUIDE_MISSIONS_HOUSE_COMPLETE = tag.ContainsKey("GUIDE_MISSIONS_HOUSE_COMPLETE");
            #endregion

            TEST_MISSIONS_JUMP_ACTIVE = tag.ContainsKey("TEST_MISSIONS_JUMP_ACTIVE");
            TEST_MISSIONS_JUMP_COMPLETE = tag.ContainsKey("TEST_MISSIONS_JUMP_COMPLETE");
        }

        public override void NetSend(BinaryWriter writer)
        {
            var guideFlags = new BitsByte();
            guideFlags[0] = GUIDE_MISSIONS_WORLDSTART_ACTIVE;
            guideFlags[1] = GUIDE_MISSIONS_WORLDSTART_COMPLETE;
            guideFlags[2] = GUIDE_MISSIONS_HOUSE_ACTIVE;
            guideFlags[3] = GUIDE_MISSIONS_HOUSE_COMPLETE;

            var testFlags = new BitsByte();
            testFlags[0] = TEST_MISSIONS_JUMP_ACTIVE;
            testFlags[1] = TEST_MISSIONS_JUMP_COMPLETE;
            writer.Write(guideFlags);
            writer.Write(testFlags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte guideFlags = reader.ReadByte();
            BitsByte testFlags = reader.ReadByte();
            GUIDE_MISSIONS_WORLDSTART_ACTIVE = guideFlags[0];
            GUIDE_MISSIONS_WORLDSTART_COMPLETE = guideFlags[1];
            GUIDE_MISSIONS_HOUSE_ACTIVE = guideFlags[2];
            GUIDE_MISSIONS_HOUSE_COMPLETE = guideFlags[3];

            TEST_MISSIONS_JUMP_ACTIVE = testFlags[0];
            TEST_MISSIONS_JUMP_COMPLETE = testFlags[1];
        }
        public override void PostUpdateWorld()
        {
            if (GUIDE_MISSIONS_WORLDSTART_ACTIVE)
            {
                missionName = "Journey's Beginning";
                missionObjective = "Talk to Guide";
            }
            else if (GUIDE_MISSIONS_WORLDSTART_COMPLETE)
            {
                missionName = string.Empty;
                missionObjective = string.Empty;
            }
        }
    }
    public class MissionGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override void OnChatButtonClicked(NPC npc, bool firstButton)
        {
            var mechanic = NPC.FindFirstNPC(NPCID.Mechanic);
            var goblin = NPC.FindFirstNPC(NPCID.GoblinTinkerer);

            /*
            if (npc.type == NPCID.GoblinTinkerer && !firstButton)
            {
                var nPCData = NPCDialogueIDHelper.GetNPCData(NPCDialogueID.Goblin);
                var dialogues = new (string Text, int Delay, int TimeLeft, NPCData NpcData)[]
                {
                ($"I'm quite busy at the moment.", 1, 300, nPCData),
                ($"If you wouldn't mind, I am coming up with a master plan to make {Main.npc[mechanic].GivenName} fall in love with me.", 4, 600, nPCData),
                ($"Goodbye.", 1, 200, nPCData)
                };

                NPCDialogueBox dialogue = NPCDialogueBox.CreateNewDialogueSequence(dialogues);
                InGameNotificationsTracker.AddNotification(dialogue);
            }
            if (npc.type == NPCID.Mechanic && firstButton)
            {
                var nPCData = NPCDialogueIDHelper.GetNPCData(NPCDialogueID.Mechanic);
                var dialogues = new (string Text, int Delay, int TimeLeft, NPCData NpcData)[]
                {
                ($"Hey, have you noticed {Main.npc[goblin].GivenName} is up to something unusual?", 2, 300, nPCData),
                ($"Oh? is that so....", 7, 600, nPCData)
                };

                NPCDialogueBox dialogue = NPCDialogueBox.CreateNewDialogueSequence(dialogues);
                InGameNotificationsTracker.AddNotification(dialogue);
            }*/
        }
        public override void GetChat(NPC npc, ref string chat)
        {
            if (npc.type == NPCID.Guide && !MissionChecks.GUIDE_MISSIONS_WORLDSTART_COMPLETE)
            {
                chat = "Great stuff! Here's 50 silver coins. The first thing we need is shelter, can I trust you to build housing units? Given you hold on to those coins, a merchant will arrive and move in.";
                MissionChecks.GUIDE_MISSIONS_WORLDSTART_COMPLETE = true;
                MissionChecks.GUIDE_MISSIONS_WORLDSTART_ACTIVE = false;
                MissionChecks.failed = false;
                InGameNotificationsTracker.AddNotification(new MissionStatusIndicator());
                MissionStatusIndicator.rewardItem = ItemID.SilverCoin;
                MissionStatusIndicator.rewardStack = 50;
                MissionReward(Main.LocalPlayer, MissionStatusIndicator.rewardItem, 50);
                MissionChecks.TEST_MISSIONS_JUMP_ACTIVE = true;
            }
        }
        public static void MissionReward(Player player, int item, int stack)
        {
            player.QuickSpawnItem(default, item, stack);
        }
    }

    public class MissionSystem
    {
        /// <summary>
        /// Gets the item and stack amount for the mission reward. You can set up to 3 rewards.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="item"></param>
        /// <param name="stack"></param>
        public static void GetMissionReward(Player player, int item, int stack)
        {
            player.QuickSpawnItem(default, item, stack);
        }
        /// <summary>
        /// Gets the item and stack amount for the mission reward. You can set up to 3 rewards.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="item"></param>
        /// <param name="stack"></param>
        public static void GetMissionReward(Player player, int item, int stack, int item2, int stack2)
        {
            player.QuickSpawnItem(default, item, stack);
            player.QuickSpawnItem(default, item2, stack2);
        }
        /// <summary>
        /// Gets the item and stack amount for the mission reward. You can set up to 3 rewards.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="item"></param>
        /// <param name="stack"></param>
        public static void GetMissionReward(Player player, int item, int stack, int item2, int stack2, int item3, int stack3)
        {
            player.QuickSpawnItem(default, item, stack);
            player.QuickSpawnItem(default, item2, stack2);
            player.QuickSpawnItem(default, item3, stack3);
        }


    }
}