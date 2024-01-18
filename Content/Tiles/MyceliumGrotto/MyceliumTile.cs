using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Trelamium.Content.Tiles.MyceliumGrotto
{
    public class MyceliumTile : ModTile
    {
        public override string Texture => Assets.Tiles.Grotto + Name;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            DustType = 0;
            MineResist = 0.25f;

            HitSound = SoundID.Dig;

            Main.tileMerge[Type][ModContent.TileType<CobblestoneTile>()] = true;           
            Main.tileMerge[Type][ModContent.TileType<MyceliumGrassTile>()] = true;

            AddMapEntry(new Color(87, 63, 43));
        }
        public override void RandomUpdate(int i, int j) {
            if (WorldGen.genRand.NextBool(3))
                WorldGen.SpreadGrass(i, j, ModContent.TileType<MyceliumTile>(), ModContent.TileType<MyceliumGrassTile>(), true, default);          
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;    
    }
    public class MyceliumGrassTile : ModTile
    {
        public override string Texture => Assets.Tiles.Grotto + Name;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileMerge[Type]
                [ModContent.TileType<MyceliumTile>()] = true;

            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<MyceliumTile>();
            
            DustType = 39;
            MineResist = 0f;
            HitSound = SoundID.Grass;
            //SetModTree(new AlderwoodTree())/* tModPorter Note: Removed. Assign GrowsOnTileId to this tile type in ModTree.SetStaticDefaults instead */;
            AddMapEntry(new Color(221, 162, 37));
        }

        #region RandomUpdate, Dust, KillTile
        public override void NumDust(int i, int j, bool fail, ref int num) 
            => num = fail ? 1 : 3;

        /* tModPorter Note: Removed. Use ModTree.SaplingGrowthType */
     
        public override void RandomUpdate(int i, int j)
        {
            if (WorldGen.genRand.NextBool(9))
                WorldGen.SpreadGrass(i, j, ModContent.TileType<MyceliumTile>(), ModContent.TileType<MyceliumGrassTile>(), true, default);          
        }
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!effectOnly)
            {
                fail = true;
                Main.tile[i, j].TileType = (ushort)ModContent.TileType<MyceliumTile>();
                WorldGen.SquareTileFrame(i, j, true);
                for (int i1 = 0; i1 < 3; i1++)
                {
                    Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.YellowTorch, 0f, 0f, 0, default, 1.0f);
                }
            }
        }
        #endregion
    }
}