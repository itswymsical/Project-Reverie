using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ReverieMod.Core.MissionSystem
{
    public class MissionManager : ModSystem
    {
        private Dictionary<string, List<Mission>> npcMissions = new Dictionary<string, List<Mission>>();
        private Dictionary<string, HashSet<string>> completedMissions; // To store completed missions by NPC name
        public void AddMission(string npcName, string missionName, string description, List<(string Objective, int Value)> objectives, List<Item> rewards, int priority = 0)
        {
            var mission = new Mission(missionName, description, objectives, rewards, priority);

            if (!npcMissions.ContainsKey(npcName))
            {
                npcMissions[npcName] = new List<Mission>();
            }

            npcMissions[npcName].Add(mission);
        }
        public bool IsMissionActive(string missionName)
        {
            foreach (var npcMission in npcMissions.Values)
            {
                foreach (var mission in npcMission)
                {
                    if (mission.MissionName == missionName && !mission.IsComplete)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public Mission GetPriorityMission()
        {
            Mission priorityMission = null;
            int highestPriority = int.MinValue; // Initialize to a very low priority

            foreach (var npcMission in npcMissions.Values)
            {
                foreach (var mission in npcMission)
                {
                    if (!mission.IsComplete && mission.Priority > highestPriority)
                    {
                        priorityMission = mission;
                        highestPriority = mission.Priority;
                    }
                }
            }

            return priorityMission;
        }
        public void UpdateMissionObjective(string npcName, string missionName, string objectiveName, int value)
        {
            if (npcMissions.ContainsKey(npcName))
            {
                foreach (var mission in npcMissions[npcName])
                {
                    if (mission.MissionName == missionName)
                    {
                        mission.UpdateObjective(objectiveName, value);
                        if (mission.IsComplete)
                        {
                            CompleteMission(npcName, missionName);
                        }
                        break;
                    }
                }
            }
        }     
        public void CompleteMission(string npcName, string missionName)
        {
            if (npcMissions.ContainsKey(npcName))
            {
                var mission = npcMissions[npcName].FirstOrDefault(m => m.MissionName == missionName);
                if (mission != null)
                {
                    mission.IsComplete = true;
                    if (!completedMissions.ContainsKey(npcName))
                    {
                        completedMissions[npcName] = new HashSet<string>();
                    }
                    completedMissions[npcName].Add(missionName);
                    // Optionally, call SaveWorldData if needed to immediately save the state.
                    GiveMissionRewards(npcName, missionName);
                }
            }
        }
        public override void OnWorldLoad()
        {
            completedMissions = new Dictionary<string, HashSet<string>>();
        }
        private void GiveMissionRewards(string npcName, string missionName)
        {
            // Retrieve the mission details
            Mission mission = GetMission(npcName, missionName);
            if (mission != null)
            {
                // Give rewards to the player
                foreach (var reward in mission.Rewards)
                {
                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("Mission"), reward.type, reward.stack);
                    
                    // Example: Add reward items to the player's inventory
                    // Replace with your actual implementation for giving rewards to the player
                    // For example, if using Terraria.ModLoader.PlayerHooks, you can use:
                    // PlayerHooks.ModifyInventory(player.whoAmI, reward.type, reward.stack);
                    // This example assumes player is accessible where this method is called.
                }
            }
        }
        private Mission GetMission(string npcName, string missionName)
        {
            if (npcMissions.TryGetValue(npcName, out var missions))
            {
                return missions.FirstOrDefault(m => m.MissionName == missionName);
            }
            return null;
        }
    }
}