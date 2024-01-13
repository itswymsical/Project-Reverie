using Terraria;
using Terraria.ModLoader;

namespace Trelamium.Content.NPCs.Boss.Fungore.Projectiles
{
    public class FungoreSmoke : ModProjectile
    {
        public override string Texture => Assets.NPCs.Fungore + "FungoreSmoke";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 172;
            Projectile.height = 104;
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

            if (Projectile.frame > 3)
            {
                Projectile.Kill();
                Projectile.frame = 0;
            }
        }
    }
}