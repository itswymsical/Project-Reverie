using System.IO;
using Terraria.ModLoader.IO;
using Terraria.ModLoader;
using Terraria;

namespace ReverieMod.Common.Systems
{
    public class DownedBossSystem : ModSystem
    {
        public static bool downedFungore = false;

        public override void ClearWorld()
        {
            downedFungore = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (downedFungore)
            {
                tag["downedFungore"] = true;
            }

        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedFungore = tag.ContainsKey("downedFungore");
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = downedFungore;
            // flags[1] = downedOtherBoss;
            writer.Write(flags);

            /*
			Remember that Bytes/BitsByte only have up to 8 entries. If you have more than 8 flags you want to sync, use multiple BitsByte:
				This is wrong:
			flags[8] = downed9thBoss; // an index of 8 is nonsense.

				This is correct:
			flags[7] = downed8thBoss;
			writer.Write(flags);

			BitsByte flags2 = new BitsByte(); // create another BitsByte
			flags2[0] = downed9thBoss; // start again from 0
			// up to 7 more flags here
			writer.Write(flags2); // write this byte
			*/

        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedFungore = flags[0];
            // downedOtherBoss = flags[1];

            // As mentioned in NetSend, BitBytes can contain up to 8 values. If you have more, be sure to read the additional data:
            // BitsByte flags2 = reader.ReadByte();
            // downed9thBoss = flags2[0];

            // System.Collections.BitArray approach:
            /*
			int length = reader.ReadInt32();
			byte[] bytes = reader.ReadBytes(length);

			BitArray bitArray = new BitArray(bytes);
			downedMinionBoss = bitArray[0];
			downedOtherBoss = bitArray[1];
			*/
        }
    }
}
