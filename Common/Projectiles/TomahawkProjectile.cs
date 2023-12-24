using Terraria.Audio;
using Trelamium.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Trelamium.Helpers;
using System;
using Terraria;
using Terraria.ID;
using System.Collections.Generic;

namespace Trelamium.Common.Projectiles
{
	public abstract class TomahawkProjectile : StickyProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.tileCollide = true;

			stickToTile = true;
			stickToNPC = true;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 60;

			Projectile.timeLeft = 180;
			Projectile.penetrate = -1;
			Projectile.aiStyle = 0;
		}

		public override void AI()
		{
			if (!stickingToNPC)
			{
				Projectile.ai[0]++;

				if (Projectile.ai[0] >= 20f)
					Projectile.velocity.Y += 0.2f;

				Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) / 25f * Projectile.direction;
			}

			if (Projectile.timeLeft < 255 / 5)
				Projectile.alpha += 5;

			base.AI();
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (!stickingToTile)
			{
				SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
				Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
			}

			return base.OnTileCollide(oldVelocity);
		}
        public override bool PreDraw(ref Color lightColor)
		{

			Projectile.DrawProjectileTrailCentered(default, lightColor);

			return Projectile.DrawProjectileCentered(default, lightColor);

		}
	}
}
