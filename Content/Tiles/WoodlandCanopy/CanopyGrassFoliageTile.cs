using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ObjectData;

namespace ReverieMod.Content.Tiles.WoodlandCanopy
{
    public class CanopyGrassFoliageTile : ModTile
    {
        public override string Texture => Assets.Tiles.WoodlandCanopy + Name;
        public override void SetStaticDefaults()
        {

            Main.tileCut[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = true;

            HitSound = SoundID.Grass;
            DustType = DustID.Grass;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Style = 0;
            //TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);

            for (int i = 0; i < 7; i++)
            {
                TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
                TileObjectData.addSubTile(TileObjectData.newSubTile.Style);
            }
            TileID.Sets.SwaysInWindBasic[Type] = true;
            AddMapEntry(new Color(95, 143, 65));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) 
            => num = DustID.Grass;
    }
}
