using System;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace ReverieMod.Core.MissionSystem
{
    public class Mission
    {
        public string MissionName { get; private set; }
        public string Description { get; private set; }
        public Queue<(string Objective, int Value)> Objectives { get; private set; }
        public List<Item> Rewards { get; private set; }
        public bool IsComplete { get; set; }
        public int Priority { get; private set; } // Priority level of the mission
        public Mission(string missionName, string description, List<(string Objective, int Value)> objectives, List<Item> rewards, int priority = 0)
        {
            MissionName = missionName;
            Description = description;
            Objectives = new Queue<(string Objective, int Value)>(objectives);
            Rewards = rewards;
            IsComplete = false;
            Priority = priority; // Assign priority level
        }

        public void UpdateObjective(string objectiveName, int value)
        {
            if (Objectives.Count > 0 && Objectives.Peek().Objective == objectiveName)
            {
                var currentObjective = Objectives.Dequeue();
                currentObjective.Value += value;
                if (currentObjective.Value >= 1) // Assuming 1 is the required value for completion
                {
                    if (Objectives.Count == 0)
                    {
                        IsComplete = true;
                    }
                }
                else
                {
                    Objectives.Enqueue(currentObjective);
                }
            }
        }
    }
    public class MissionCategory
    {
        public string Name { get; private set; }
        public Dictionary<string, Mission> Missions { get; private set; }

        public MissionCategory(string name)
        {
            Name = name;
            Missions = new Dictionary<string, Mission>();
        }

        public void AddMission(string missionName, string description, List<(string, int)> objectives, List<Item> rewards)
        {
            Missions[missionName] = new Mission(missionName, description, objectives, rewards);
        }

        public Mission GetMission(string missionName)
        {
            return Missions.TryGetValue(missionName, out var mission) ? mission : null;
        }
    }

    public static class GuideMissions
    {
        public static void AddGuideMissions(MissionManager missionManager)
        {
            List<Item> rewards = new List<Item> { new Item(ItemID.SilverCoin, 50) };
            missionManager.AddMission("Guide", "Journey's Begin", "Talk to the guide", new List<(string, int)> { ("Talk to the guide", 1) }, rewards, priority: 2);
        }
    }
}