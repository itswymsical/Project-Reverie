using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Trelamium.Content.Tiles.DruidsGarden
{
    public class AlluviumOreTile : ModTile
    {
        public override string Texture => Assets.Tiles.DruidsGarden + "AlluviumOreTile";
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type]
                [ModContent.TileType<LoamTile>()] = true;

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