using ReverieMod.Common.UI;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.RGB;
using Terraria.ID;
using Terraria.ModLoader;

namespace ReverieMod.Core.MissionSystem
{

    public class MissionGlobal : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.Zombie)
            {
                ModContent.GetInstance<MissionManager>().UpdateMissionObjective("Guide", "Zombie Hunter", "Kill Zombies", 1);
            }
        }
    }
    public class MissionModPlayer : ModPlayer
    {
        public override void OnEnterWorld()
        {
            var missionManager = ModContent.GetInstance<MissionManager>();

            // Add missions for Guide
            GuideMissions.AddGuideMissions(missionManager);

            // Add missions for Merchant
            MerchantMissions.AddMerchantMissions(missionManager);

            // Example: Update mission objectives
            //missionManager.UpdateMissionObjective("Guide", "Housemaker", "Build a house", 1);
            missionManager.UpdateMissionObjective("Merchant", "Shopper", "Buy items", 2);
        }
    }
    public class ObjectiveTracker : ModPlayer
    {
        public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            var missionManager = ModContent.GetInstance<MissionManager>();
            missionManager.UpdateMissionObjective("Merchant", "Shopper", "Buy items", 1);
        }
        // Additional tracking methods can be added here for other objectives
    }
}