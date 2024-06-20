using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;
using Humanizer;
using ReverieMod.Core.Detours;
using System.IO;
using Terraria.ObjectData;

namespace ReverieMod.Common.Systems
{
    public class TileTagSystem : ModSystem
    {
        private Dictionary<Point, TileTag> tileTags;
        public override void Load() => tileTags = new Dictionary<Point, TileTag>();
        
        public static bool naturallyPlaced = false;
        public override void ClearWorld() => naturallyPlaced = false;
        public override void SaveWorldData(TagCompound tag)
        {
            if (naturallyPlaced)
            {
                tag["naturallyPlaced"] = true;
            }
        }
        public override void LoadWorldData(TagCompound tag) => naturallyPlaced = tag.ContainsKey("naturallyPlaced");
        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = naturallyPlaced;
            writer.Write(flags);
        }
        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            naturallyPlaced = flags[0];
        }

        public void TagTile(int x, int y, TileTag tag)
        {
            Point point = new Point(x, y);
            if (tileTags.ContainsKey(point))
            {
                tileTags[point] = tag;
            }
            else
            {
                tileTags.Add(point, tag);
            }
        }
        public TileTag GetTileTag(int x, int y)
        {
            Point point = new Point(x, y);
            if (tileTags.TryGetValue(point, out TileTag tag))
            {
                return tag;
            }
            return null;
        }
        public void RemoveTileTag(int x, int y)
        {
            Point point = new Point(x, y);
            if (tileTags.ContainsKey(point))
            {
                tileTags.Remove(point);
            }
        }
    }
    public class TileTag
    {
        public string TagName { get; set; }
        public int Value { get; set; }

        public TileTag(string tagName, int value)
        {
            TagName = tagName;
            Value = value;
        }
    }
}