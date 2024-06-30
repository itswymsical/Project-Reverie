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
        public bool IsComplete { get; private set; }

        public Mission(string missionName, string description, List<(string, int)> objectives, List<Item> rewards)
        {
            MissionName = missionName;
            Description = description;
            Objectives = new Queue<(string, int)>(objectives);
            Rewards = rewards;
            IsComplete = false;
        }

        public void UpdateObjective(string objective, int value)
        {
            if (Objectives.Count == 0) return;

            var currentObjective = Objectives.Peek();

            if (currentObjective.Objective == objective)
            {
                if (value >= currentObjective.Value)
                {
                    Objectives.Dequeue(); // Objective completed
                    if (Objectives.Count == 0)
                    {
                        CompleteMission();
                    }
                }
            }
        }

        public void SetComplete(bool complete)
        {
            IsComplete = complete;
            if (complete)
            {
                GiveRewards();
            }
        }

        private void CompleteMission()
        {
            IsComplete = true;
            GiveRewards();
        }

        private void GiveRewards()
        {
            Player player = Main.LocalPlayer;
            foreach (var reward in Rewards)
            {
                player.QuickSpawnItem(player.GetSource_Misc("MissionComplete"), reward.type, reward.stack);
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
            List<Item> rewards = new List<Item> { new Item(ItemID.Wood, 10), new Item(ItemID.Torch, 5) };
            missionManager.AddMission("Guide", "Housemaker", "Build a house", new List<(string, int)> { ("Build a house", 1) }, rewards);

            rewards = new List<Item> { new Item(ItemID.IronPickaxe, 1) };
            missionManager.AddMission("Guide", "Explorer", "Explore the world", new List<(string, int)> { ("Explore the world", 1) }, rewards);
        }
    }

    public static class MerchantMissions
    {
        public static void AddMerchantMissions(MissionManager missionManager)
        {
            List<Item> rewards = new List<Item> { new Item(ItemID.GoldCoin, 1) };
            missionManager.AddMission("Merchant", "Shopper", "Buy items", new List<(string, int)> { ("Buy items", 5) }, rewards);

            rewards = new List<Item> { new Item(ItemID.SilverCoin, 50) };
            missionManager.AddMission("Merchant", "Seller", "Sell items", new List<(string, int)> { ("Sell items", 5) }, rewards);
        }
    }
}