using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace EmpyreanDreamscape.Content.Tiles.DruidsGarden
{
    public class LoamTile : ModTile
    {
        public override string Texture => Assets.Tiles.DruidsGarden + Name;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            DustType = 0;
            MineResist = 0.25f;

            HitSound = SoundID.Dig;

            Main.tileMerge[Type][ModContent.TileType<CobblestoneTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<LoamTileGrass>()] = true;

            AddMapEntry(new Color(126, 82, 52));
        }
        public override void RandomUpdate(int i, int j)
        {
            if (WorldGen.genRand.NextBool(3))
                WorldGen.SpreadGrass(i, j, ModContent.TileType<LoamTile>(), ModContent.TileType<LoamTileGrass>(), true, default);
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
    public class LoamTileGrass : ModTile
    {
        public override string Texture => Assets.Tiles.DruidsGarden + Name;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<LoamTile>()] = true;

            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<LoamTile>();

            DustType = 39;
            MineResist = 0f;
            HitSound = SoundID.Grass;
            //SetModTree(new AlderwoodTree())/* tModPorter Note: Removed. Assign GrowsOnTileId to this tile type in ModTree.SetStaticDefaults instead */;
            AddMapEntry(new Color(32, 103, 29));
        }
        #region RandomUpdate, Dust, KillTile
        public override void NumDust(int i, int j, bool fail, ref int num)
            => num = fail ? 1 : 3;

        /* tModPorter Note: Removed. Use ModTree.SaplingGrowthType */

        public override void RandomUpdate(int i, int j)
        {
            if (WorldGen.genRand.NextBool(9))
                WorldGen.SpreadGrass(i, j, ModContent.TileType<LoamTile>(), ModContent.TileType<LoamTileGrass>(), true, default);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!effectOnly)
            {
                fail = true;
                Main.tile[i, j].TileType = (ushort)ModContent.TileType<LoamTile>();
                WorldGen.SquareTileFrame(i, j, true);
                for (int i1 = 0; i1 < 3; i1++)
                {
                    Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.JungleGrass, 0f, 0f, 0, default, 1.0f);
                }
            }
        }
        #endregion
    }
}