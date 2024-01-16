using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Trelamium.Content.Tiles
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

            MineResist = 0.65f;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(155, 115, 155));
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}