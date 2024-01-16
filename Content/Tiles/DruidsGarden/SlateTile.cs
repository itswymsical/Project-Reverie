﻿
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Trelamium.Content.Tiles.DruidsGarden
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
            MineResist = 0.35f;
            //ItemDrop = ModContent.ItemType<Items.Placeable.Slate>();

            HitSound = SoundID.Dig;
            Main.tileMerge[Type][ModContent.TileType<LoamTile>()] = true;

            AddMapEntry(new Color(145, 150, 160));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
