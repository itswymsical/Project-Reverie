
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using ReverieMod.Content.Items.Tiles;
using System.Collections.Generic;

namespace ReverieMod.Content.Tiles.DruidsGarden
{
    public class SlateTile : ModTile
    {
        public override string Texture => Assets.Tiles.DruidsGarden + "SlateTile";
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            DustType = 0;
            MineResist = 0.465f;

            HitSound = SoundID.Tink;
            Main.tileMerge[Type][ModContent.TileType<LoamTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<AlluviumOreTile>()] = true;
            AddMapEntry(new Color(94, 92, 89));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
