using Terraria;
using Terraria.ModLoader;

namespace Trelamium.Content.NPCs.Boss.Fungore.Projectiles
{
    public class FungoreSlam : ModProjectile
    {
        public override string Texture => Assets.NPCs.Fungore + "FungoreSlam";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }
        public override void SetDefaults()
        {
            Projectile.width = 244;
            Projectile.height = 208;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }

            if (Projectile.frame > 5)
            {
                Projectile.Kill();
                Projectile.frame = 0;
            }
        }
    }
}