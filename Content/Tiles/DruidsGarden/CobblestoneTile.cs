using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using EmpyreanDreamscape.Content.Tiles.DruidsGarden;

namespace EmpyreanDreamscape.Content.Tiles
{
    public class CobblestoneTile : ModTile
    {
        public override string Texture => Assets.Tiles.Directory + "CobblestoneTile";
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            Main.tileMerge[Type][ModContent.TileType<LoamTile>()] = true;
            MineResist = 0.65f;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(105, 106, 105));
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}