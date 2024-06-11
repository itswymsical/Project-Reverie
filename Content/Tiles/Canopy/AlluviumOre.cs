using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ReverieMod.Content.Tiles.Canopy
{
    public class AlluviumOre : ModTile
    {
        public override string Texture => Assets.Tiles.Canopy + Name;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;

            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            DustType = 151;
            MineResist = 0.35f;
            MinPick = 50;
            HitSound = SoundID.Tink;

            AddMapEntry(new Color(108, 187, 86));
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}