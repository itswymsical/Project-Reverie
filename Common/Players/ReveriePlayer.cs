using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ModLoader;

namespace ReverieMod.Common.Players
{
    public partial class ReveriePlayer : ModPlayer
    {
        public float ScreenShakeIntensity;

        public bool toadstoolExplode;

        public int toadstoolCount;

        public bool Harvest;

        public bool ZoneWoodlandCanopy;

        public bool ZoneEmberite;
        public override void ResetEffects()
        {
            toadstoolExplode = false;
        }

        /*
        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            if (mediumCoreDeath)
                return [new Item(ModContent.ItemType<CopperShovel>())];
            
            return [new Item(ModContent.ItemType<CopperShovel>())];
        }*/

        public override void ModifyScreenPosition()
        {
            if (ScreenShakeIntensity > 0.195f)
            {
                var shake = new Vector2(Main.rand.NextFloat(-ScreenShakeIntensity, ScreenShakeIntensity), Main.rand.NextFloat(-ScreenShakeIntensity, ScreenShakeIntensity));

                Main.screenPosition += shake;

                ScreenShakeIntensity *= 0.95f;
            }
        }
    }
}
