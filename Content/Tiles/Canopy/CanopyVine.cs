using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using ReverieMod.Utilities;

namespace ReverieMod.Content.Tiles.Canopy
{
    public class CanopyVine : ModTile
    {
        public override string Texture => Assets.Tiles.Canopy + Name;
        public override void SetStaticDefaults()
        {
            TileID.Sets.VineThreads[Type] = true;
            TileID.Sets.IsVine[Type] = true;
            TileID.Sets.SwaysInWindBasic[Type] = true;

            Main.tileCut[Type] = true;     
            Main.tileLavaDeath[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            HitSound = SoundID.Grass;
            DustType = DustID.JunglePlants;

            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);
            AddMapEntry(new Color(95, 143, 65));
        }
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Tile tile = Framing.GetTileSafely(i, j + 1);
            if (tile.HasTile && tile.TileType == Type)
            {
                WorldGen.KillTile(i, j + 1);
            }
        }
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Framing.GetTileSafely(i, j - 1);
            int type = -1;
            if (tile.HasTile && !tile.BottomSlope)
                type = tile.TileType;

            if (type == ModContent.TileType<Woodgrass>() || type == TileID.LivingWood || type == Type) {
                return true;
            }

            WorldGen.KillTile(i, j);
            return true;
        }
        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j + 1);
            if (WorldGen.genRand.NextBool(10) && !tile.HasTile && !(tile.LiquidType == LiquidID.Lava))
            {
                bool placed = false;
                int Test = j;
                while (Test > j - 10)
                {
                    Tile testTile = Framing.GetTileSafely(i, Test);
                    if (testTile.BottomSlope)
                    {
                        break;
                    }
                    else if (!testTile.HasTile || testTile.TileType != ModContent.TileType<Woodgrass>())
                    {
                        Test--;
                        continue;
                    }
                    placed = true;
                    break;
                }

                if (placed)
                {
                    tile.TileType = Type;
                    tile.HasTile = true;
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                    }
                }
            }
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) => true;
        
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Color colour = new Color(5, 143, 65);

            Texture2D glow = ModContent.Request<Texture2D>("ReverieMod/Assets/Textures/Tiles/Canopy/CanopyVine_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            spriteBatch.Draw(glow, new Vector2(i, j) - Main.screenPosition + zero - new Vector2(0, 0), new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), colour * .6f);
        }
    }
}
