using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ReverieMod.Core.MissionSystem
{
    public class MissionManager : ModSystem
    {
        private Dictionary<string, MissionCategory> missionCategories = new Dictionary<string, MissionCategory>();

        public void AddMissionCategory(string categoryName)
        {
            if (!missionCategories.ContainsKey(categoryName))
            {
                missionCategories[categoryName] = new MissionCategory(categoryName);
            }
        }

        public void AddMission(string categoryName, string missionName, string description, List<(string, int)> objectives, List<Item> rewards)
        {
            if (!missionCategories.ContainsKey(categoryName))
            {
                missionCategories[categoryName] = new MissionCategory(categoryName);
            }
            missionCategories[categoryName].AddMission(missionName, description, objectives, rewards);
        }

        public void UpdateMissionObjective(string categoryName, string missionName, string objective, int value)
        {
            if (missionCategories.TryGetValue(categoryName, out var category))
            {
                var mission = category.GetMission(missionName);
                mission?.UpdateObjective(objective, value);
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            List<TagCompound> categories = new List<TagCompound>();
            foreach (var category in missionCategories)
            {
                TagCompound categoryTag = new TagCompound
                {
                    ["CategoryName"] = category.Key,
                    ["Missions"] = new List<TagCompound>()
                };

                foreach (var mission in category.Value.Missions)
                {
                    TagCompound missionTag = new TagCompound
                    {
                        ["MissionName"] = mission.Value.MissionName,
                        ["Description"] = mission.Value.Description,
                        ["Objectives"] = mission.Value.Objectives,
                        ["Rewards"] = mission.Value.Rewards.ConvertAll(item => ItemIO.Save(item)),
                        ["IsComplete"] = mission.Value.IsComplete
                    };
                    categoryTag.Get<List<TagCompound>>("Missions").Add(missionTag);
                }

                categories.Add(categoryTag);
            }
            tag["MissionCategories"] = categories;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            missionCategories.Clear();
            if (tag.ContainsKey("MissionCategories"))
            {
                var categories = tag.GetList<TagCompound>("MissionCategories");
                foreach (var categoryTag in categories)
                {
                    var categoryName = categoryTag.GetString("CategoryName");
                    var category = new MissionCategory(categoryName);

                    var missions = categoryTag.GetList<TagCompound>("Missions");
                    foreach (var missionTag in missions)
                    {
                        var missionName = missionTag.GetString("MissionName");
                        var description = missionTag.GetString("Description");
                        var objectives = missionTag.Get<List<(string, int)>>("Objectives");
                        var rewards = missionTag.Get<List<TagCompound>>("Rewards").ConvertAll(ItemIO.Load);
                        var isComplete = missionTag.GetBool("IsComplete");
                        var mission = new Mission(missionName, description, objectives, rewards);
                        mission.SetComplete(isComplete);
                        category.Missions[missionName] = mission;
                    }

                    missionCategories[categoryName] = category;
                }
            }
        }
    }
}