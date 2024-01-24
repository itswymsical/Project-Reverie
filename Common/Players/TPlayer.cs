using Microsoft.Xna.Framework;

using Terraria;
using Terraria.DataStructures;

using Terraria.ModLoader;

using EmpyreanDreamscape.Helpers;
using Terraria.ID;

namespace EmpyreanDreamscape.Common.Players
{
    public partial class TPlayer : ModPlayer
    {
        //LOCAL
        public float ScreenShakeIntensity;

        public bool dustrollerSkates;

        public bool gluttonAmulet;

        public bool microlith;

        public bool onSand;

        public bool scarabIdol;

        public bool theMagnolia;

        public bool toadstoolExplode;

        public int toadstoolCount;

        public bool mushroomHeal;

        // BIOMES
        public bool ZoneDruidsGarden;

        public bool ZoneDustifiedCaverns;
        public override void ResetEffects()
        {
            dustrollerSkates = false;

            gluttonAmulet = false;

            microlith = false;

            onSand = false;

            scarabIdol = false;

            theMagnolia = false;

            toadstoolExplode = false;

            mushroomHeal = false;
        }
        /*
        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (Player.ZoneForest() && Main.rand.NextBool(6))
            {
               caughtType = ModContent.ItemType<Fleurer>();
            }

            if (Player.ZoneForest() && Main.rand.NextBool(5))
            {
                caughtType = ModContent.ItemType<Barkfish>();
            }

            if (Player.ZoneForest() && Main.rand.NextBool(5))
            {
                caughtType = ModContent.ItemType<ShreemCarp>();
            }

            if (Player.ZoneDesert && Main.rand.NextBool(11))
            {
                caughtType = ModContent.ItemType<UraeusEel>();
            }

            if (Player.ZoneDesert && Main.rand.NextBool(7))
            {
                caughtType = ModContent.ItemType<Sunfish>();
            }

            if (Player.ZoneDesert && Main.rand.NextBool(5))
            {
                caughtType = ModContent.ItemType<Scaracod>();
            }
        }*/
        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            if (gluttonAmulet)
                healValue = (int)(healValue * 1.33f);

            if (mushroomHeal && item.type == ItemID.Mushroom)
                healValue = (int)(healValue * 3f);
        }
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
