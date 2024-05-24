using Terraria;
using Terraria.Audio;
using Terraria.ID;

using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using ReverieMod.Helpers;
using ReverieMod.Core;
using ReverieMod.Common.Players;

namespace ReverieMod.Content.Projectiles.Melee
{
	public class FrostbarkClaymoreProjectile : ModProjectile
	{
        public override string Texture => Assets.Projectiles.Frostbark + Name;
        private enum AIState
		{
			Spawning,
			Swinging = 2
		}
		AIState State
		{
			get => (AIState)Projectile.ai[0];
			set => Projectile.ai[0] = (int)value;
		}
		private bool IsMaxCharge;
		private readonly float MaxChargeTime = 32f;
		private float RotationStart => MathHelper.PiOver2 + (Projectile.direction == -1 ? MathHelper.Pi : 0);
		private float RotationOffset => Projectile.direction == 1 ? 0 : MathHelper.PiOver2;
		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 58;
			Projectile.penetrate = -1;

			Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly =
				Projectile.netImportant =
				Projectile.ownerHitCheck =
				Projectile.manualDirectionChange = true;

			Projectile.tileCollide = IsMaxCharge = false;
		}
		public override bool PreAI()
		{
			Player owner = Main.player[Projectile.owner];
			if (!owner.active || owner.dead)
			{
				Projectile.Kill();
			}

			if (State == AIState.Swinging)
			{
				Projectile.ai[1] -= 4;

				if (Projectile.ai[1] <= 0)
				{
					Projectile.Kill();
				}
			}
			else
			{
				if (++Projectile.ai[1] >= MaxChargeTime)
				{
					IsMaxCharge = true;
					Projectile.ai[1] = MaxChargeTime;
				}

				if (Main.myPlayer == Projectile.owner && !owner.channel && Projectile.ai[1] >= (MaxChargeTime / 2))
				{
					State = AIState.Swinging;
					Projectile.netUpdate = true;
				}
			}

			SetProjectilePosition(owner);

			SetOwnerAnimation(owner);

			return false;
		}

		public override bool? CanDamage() => State != AIState.Spawning;

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact);

			Helper.SpawnDustCloud(Projectile.position, Projectile.width, Projectile.height, 0, 60);
		}
		public override void OnKill(int timeLeft)
		{
            Player player = Main.player[Projectile.owner];
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact);
            if (IsMaxCharge && Main.myPlayer == Projectile.owner)
			{
				for (int i = 0; i < 3; ++i)
				{
					Projectile.NewProjectile(default, Projectile.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2) * 8f, ProjectileID.IceBolt, (int)(Projectile.damage * 0.5f), 0.5f, Projectile.owner);
				}
			}
			Helper.SpawnDustCloud(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, 50);
		}

		private void SetProjectilePosition(Player owner)
		{
			Projectile.spriteDirection = Projectile.direction;

			Vector2 rotatedPoint = owner.RotatedRelativePoint(owner.MountedCenter);

			Projectile.rotation = RotationStart - (MathHelper.Pi / MaxChargeTime * Projectile.ai[1]) * Projectile.direction;
			Projectile.Center = rotatedPoint + (Projectile.rotation - MathHelper.PiOver4 - RotationOffset).ToRotationVector2() * 60;
		}

		private void SetOwnerAnimation(Player owner)
		{
			owner.itemTime = owner.itemAnimation = 10;

			owner.heldProj = Projectile.whoAmI;

			float currentAnimationFraction = Projectile.ai[1] / MaxChargeTime;

			if (currentAnimationFraction < 0.4f)
				owner.bodyFrame.Y = owner.bodyFrame.Height * 3;
			
			else if (currentAnimationFraction < 0.75f)
				owner.bodyFrame.Y = owner.bodyFrame.Height * 2;
			
			else
				owner.bodyFrame.Y = owner.bodyFrame.Height;
			
		}
	}
}