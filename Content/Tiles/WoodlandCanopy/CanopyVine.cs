using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace ReverieMod.Content.Tiles.WoodlandCanopy
{
    public class CanopyVine : ModTile
    {
        public override string Texture => Assets.Tiles.WoodlandCanopy + Name;
        public override void SetStaticDefaults()
        {
            TileID.Sets.VineThreads[Type] = true;
            TileID.Sets.IsVine[Type] = true;

            Main.tileCut[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLavaDeath[Type] = true;

            Main.tileNoFail[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = false;

            HitSound = SoundID.Grass;
            DustType = DustID.Grass;

            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);
            AddMapEntry(new Color(95, 143, 65));
        }
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Framing.GetTileSafely(i, j - 1);
            int type = -1;
            if (tile.HasTile && !tile.BottomSlope)
                type = tile.TileType;

            if (type == ModContent.TileType<WoodlandGrassTile>() || type == Type) {
                return true;
            }

            WorldGen.KillTile(i, j);
            return true;
        }
        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j + 1); //check for tiles below
            if (WorldGen.genRand.NextBool(10) && !tile.HasTile && !(tile.LiquidType == LiquidID.Lava))
            {
                tile.HasTile = false;
            }
            else
            {
                tile.TileType = Type;
                tile.HasTile = true;
                WorldGen.SquareTileFrame(i, j + 1, true);
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendTileSquare(-1, i, j + 1, 3, 0);
                }
            }
        }
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Tile tile = Framing.GetTileSafely(i, j + 1);
            if (tile.HasTile && tile.TileType == Type)
            {
                WorldGen.KillTile(i, j + 1);
            }
        }
        
    }
}
