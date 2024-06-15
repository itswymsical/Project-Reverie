using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ObjectData;
using Microsoft.Xna.Framework.Graphics;

namespace ReverieMod.Content.Tiles.Canopy
{
    public class CanopyFoliage : ModTile
    {
        public override string Texture => Assets.Tiles.Canopy + Name;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileSolid[Type] = false;

            TileObjectData.newTile.AnchorValidTiles = [ModContent.TileType<Woodgrass>()];
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;

            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = true;

            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.addTile(Type);

            DustType = DustID.BrownMoss;
            HitSound = SoundID.Grass;
            AddMapEntry(new Color(95, 143, 65));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = DustID.JunglePlants;
        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }
    }
}
