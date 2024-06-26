using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace ReverieMod.Common.UI
{
    public class MissionChecks : ModSystem
    {      
        public static bool GUIDE_MISSIONS_WORLDSTART = false;
        public static bool GUIDE_MISSIONS_OBTAIN_MIRROR = false;
        public static MissionChecks Instance => ModContent.GetInstance<MissionChecks>();
        public override void ClearWorld()
        {
            GUIDE_MISSIONS_WORLDSTART = false;
            GUIDE_MISSIONS_OBTAIN_MIRROR = false;
        }
        public override void SaveWorldData(TagCompound tag)
        {
            if (GUIDE_MISSIONS_WORLDSTART)
            {
                tag["GUIDE_MISSIONS_WORLDSTART"] = true;
            }
            if (GUIDE_MISSIONS_OBTAIN_MIRROR)
            {
                tag["GUIDE_MISSIONS_OBTAIN_MIRROR"] = true;
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            GUIDE_MISSIONS_WORLDSTART = tag.ContainsKey("GUIDE_MISSIONS_WORLDSTART");
            GUIDE_MISSIONS_OBTAIN_MIRROR = tag.ContainsKey("GUIDE_MISSIONS_OBTAIN_MIRROR");
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = GUIDE_MISSIONS_WORLDSTART;
            flags[0] = GUIDE_MISSIONS_OBTAIN_MIRROR;
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            GUIDE_MISSIONS_WORLDSTART = flags[0];
            GUIDE_MISSIONS_OBTAIN_MIRROR = flags[0];
        }
    }
    public class MissionGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
    }
}
