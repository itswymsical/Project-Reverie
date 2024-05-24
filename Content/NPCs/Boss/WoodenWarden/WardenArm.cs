using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReverieMod.Helpers;
using ReverieMod.Content.NPCs.Boss.WoodenWarden;
using ReverieMod.Core.Mechanics;

namespace ReverieMod.Content.NPCs.Bosses.WoodenWarden
{
	[AutoloadBossHead]
	public class WardenArmLeft : ModNPC
	{
        public override string Texture => Assets.NPCs.WoodenWarden + Name;
        private enum AIState
		{
			Idle,
			Dash,
			Beam,
			Grab
		}
		private AIState State
		{
			get => (AIState)NPC.ai[1];
			set => NPC.ai[1] = (int)value;
		}
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            Main.npcFrameCount[NPC.type] = 3;
        }
        public override void SetDefaults()
		{
			NPC.aiStyle = -1;
			NPC.width = 62;
			NPC.height = 96;

			NPC.damage = 30;
			NPC.defense = 8;
			NPC.lifeMax = 240;

			NPC.noTileCollide = true;
			NPC.noGravity = true;
			NPC.lavaImmune = true;
			NPC.netUpdate = true;

			NPC.knockBackResist = 0f;
			NPC.HitSound = SoundID.DD2_CrystalCartImpact;
			NPC.DeathSound = SoundID.Item14;

            NPC.scale = 1.15f;
		}
		private int AITimer;
		private int BeamTimer;
		private int DashTimer;
		public override void FindFrame(int frameHeight)
		{
			NPC.spriteDirection = NPC.direction;
			if (State == AIState.Dash || State == AIState.Idle)
			{
				NPC.frame.Y = 2 * frameHeight;
			}
			if (State == AIState.Beam)
			{
				NPC.frame.Y = 1 * frameHeight;
			}
			if (State == AIState.Grab)
			{
				NPC.frame.Y = 0 * frameHeight;
			}
		}
        public static Vector2 CalculatePoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector2 p = uu * p0;
            p += 2 * u * t * p1;
            p += tt * p2;

            return p;
        }

        public override void AI()
		{
			Player target = Main.player[NPC.target];
			NPC boss = Main.npc[(int)NPC.ai[0]];
			if (target.dead || !target.active)
			{
				NPC.TargetClosest(false);
				NPC.velocity = new Vector2(0f, -10f);
				NPC.timeLeft--;
				if (NPC.timeLeft <= 0)
				{
					NPC.active = false;
				}
			}
			AITimer++;
			if (AITimer == 500)
				State = AIState.Dash;

			if (AITimer == 900)
				State = AIState.Beam;

			if (AITimer >= 1200)
            {
                State = AIState.Idle;
                AITimer = 0;
            }        

			if (State == AIState.Idle)
			{
				var angle = target.Center - NPC.Center;
				NPC.rotation = angle.ToRotation() + MathHelper.PiOver2;
				angle.Normalize();
				angle.X *= 3f;
				angle.Y *= 3f;

				var position = boss.Center - new Vector2(180, -70);
				float speed = Vector2.Distance(NPC.Center, position);
				speed = MathHelper.Clamp(speed, -10f, 10f);

				Move(position, speed);
			}
			if (State == AIState.Beam) // This is the same as the "Idle" state but it shoots beams.
			{
				BeamTimer++;
				var angle = target.Center - NPC.Center;
				NPC.rotation = angle.ToRotation() + MathHelper.PiOver2;
				angle.Normalize();
				angle.X *= 3f;
				angle.Y *= 3f;

				var position = boss.Center - new Vector2(220, -70);
				float speed = Vector2.Distance(NPC.Center, position);
				speed = MathHelper.Clamp(speed, -14f, 14f);

				Move(position, speed);
				if (BeamTimer >= 120)
				{
					Projectile.NewProjectile(default, NPC.Center, angle * 2.5f, ProjectileID.EyeBeam, 5, 0f, Main.myPlayer);
					BeamTimer = 0;
				}
				if (NPC.life < NPC.lifeMax * 0.20f)
				{
					if (BeamTimer >= 30)
					{
						Projectile.NewProjectile(default, NPC.Center, angle * 2.5f, ProjectileID.EyeBeam, 6, 0f, Main.myPlayer);
						BeamTimer = 0;
					}
				}
			}
			if (State == AIState.Dash)
			{
				var angle = target.Center - NPC.Center;
				NPC.rotation = angle.ToRotation() + MathHelper.PiOver2;
				angle.Normalize();
				angle.X *= 2f;
				angle.Y *= 2f;
				DashTimer++;
				if (DashTimer >= Main.rand.Next(70, 100))
				{
					NPC.TargetClosest();
					NPC.netUpdate = true;
					Vector2 PlayerPosition = new Vector2(target.Center.X - NPC.Center.X, target.Center.Y - NPC.Center.Y);
                    for (float t = 0; t <= 1; t += 0.01f)
                    {
                        PlayerPosition.Normalize();
                        Vector2 point = CalculatePoint(t, target.Center, NPC.Center, PlayerPosition);
                        NPC.velocity = point * 8f;
                    }
                    
					DashTimer = 0;
				}
				if (NPC.life < NPC.lifeMax * 0.20f)
				{
					if (DashTimer >= 60)
					{
						NPC.TargetClosest();
						NPC.netUpdate = true;
						Vector2 PlayerPosition = new Vector2(target.Center.X - NPC.Center.X, target.Center.Y - NPC.Center.Y);
						PlayerPosition.Normalize();
						NPC.velocity = PlayerPosition * 10.5f;
						DashTimer = 0;
					}
				}
			}
		}
		public override void BossHeadRotation(ref float rotation) => rotation = NPC.rotation;
		private void Move(Vector2 position, float speed)
		{
			Vector2 direction = NPC.DirectionTo(position);

			Vector2 velocity = direction * speed;

			NPC.velocity = Vector2.SmoothStep(NPC.velocity, velocity, 0.2f);
		}
		public override bool CheckActive()
		{
			if (!NPC.AnyNPCs(ModContent.NPCType<WoodenWardenBoss>()))
				NPC.active = false;

			return base.CheckActive();
		}
		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0)
			{
				for (int num2 = 0; num2 < 30; num2++)
				{
					int num = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Dirt, 0f, 0f, 125, default, 0.65f);
					Main.dust[num].velocity *= 3f;
				}
			}
		}
    }

    [AutoloadBossHead]
    public class WardenArmRight : ModNPC
    {
        public override string Texture => Assets.NPCs.WoodenWarden + Name;
        private enum AIState
        {
            Idle,
            Dash,
            Beam,
            Grab
        }
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (int)value;
        }
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            Main.npcFrameCount[NPC.type] = 3;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.width = 62;
            NPC.height = 96;

            NPC.damage = 30;
            NPC.defense = 8;
            NPC.lifeMax = 240;

            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.netUpdate = true;

            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.DD2_CrystalCartImpact;
            NPC.DeathSound = SoundID.Item14;

            NPC.scale = 1.15f;
        }
        private int AITimer;
        private int BeamTimer;
        private int DashTimer;
        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            if (State == AIState.Dash || State == AIState.Idle)
            {
                NPC.frame.Y = 2 * frameHeight;
            }
            if (State == AIState.Beam)
            {
                NPC.frame.Y = 1 * frameHeight;
            }
            if (State == AIState.Grab)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }
        public override void AI()
        {
            Player target = Main.player[NPC.target];
            NPC boss = Main.npc[(int)NPC.ai[0]];
            if (target.dead || !target.active)
            {
                NPC.TargetClosest(false);
                NPC.velocity = new Vector2(0f, -10f);
                NPC.timeLeft--;
                if (NPC.timeLeft <= 0)
                {
                    NPC.active = false;
                }
            }
            AITimer++;
            if (AITimer == 300)
                State = AIState.Dash;

            if (AITimer == 900)
                State = AIState.Beam;

            if (AITimer >= 1200)
            {
                State = AIState.Idle;
                AITimer = 0;
            }

            if (State == AIState.Idle)
            {
                var angle = target.Center - NPC.Center;
                NPC.rotation = angle.ToRotation() + MathHelper.PiOver2;
                angle.Normalize();
                angle.X *= 3f;
                angle.Y *= 3f;

                var position = boss.Center - new Vector2(-180, -70);
                float speed = Vector2.Distance(NPC.Center, position);
                speed = MathHelper.Clamp(speed, -10f, 10f);

                Move(position, speed);
            }
            if (State == AIState.Beam) // This is the same as the "Idle" state but it shoots beams.
            {
                BeamTimer++;
                var angle = target.Center - NPC.Center;
                NPC.rotation = angle.ToRotation() + MathHelper.PiOver2;
                angle.Normalize();
                angle.X *= 3f;
                angle.Y *= 3f;

                var position = boss.Center - new Vector2(-220, -70);
                float speed = Vector2.Distance(NPC.Center, position);
                speed = MathHelper.Clamp(speed, -14f, 14f);

                Move(position, speed);
                if (BeamTimer >= 120)
                {
                    Projectile.NewProjectile(default, NPC.Center, angle * 2.5f, ProjectileID.EyeBeam, 5, 0f, Main.myPlayer);
                    BeamTimer = 0;
                }
                if (NPC.life < NPC.lifeMax * 0.20f)
                {
                    if (BeamTimer >= 30)
                    {
                        Projectile.NewProjectile(default, NPC.Center, angle * 2.5f, ProjectileID.EyeBeam, 6, 0f, Main.myPlayer);
                        BeamTimer = 0;
                    }
                }
            }
            if (State == AIState.Dash)
            {
                var angle = target.Center - NPC.Center;
                NPC.rotation = angle.ToRotation() + MathHelper.PiOver2;
                angle.Normalize();
                angle.X *= 2f;
                angle.Y *= 2f;
                DashTimer++;
                if (DashTimer >= Main.rand.Next(70, 100))
                {
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                    Vector2 PlayerPosition = new Vector2(target.Center.X - NPC.Center.X, target.Center.Y - NPC.Center.Y);
                    PlayerPosition.Normalize();
                    NPC.velocity = PlayerPosition * 8f;
                    DashTimer = 0;
                }
                if (NPC.life < NPC.lifeMax * 0.20f)
                {
                    if (DashTimer >= 60)
                    {
                        NPC.TargetClosest();
                        NPC.netUpdate = true;
                        Vector2 PlayerPosition = new Vector2(target.Center.X - NPC.Center.X, target.Center.Y - NPC.Center.Y);
                        PlayerPosition.Normalize();
                        NPC.velocity = PlayerPosition * 10.5f;
                        DashTimer = 0;
                    }
                }
            }
        }
        public override void BossHeadRotation(ref float rotation) => rotation = NPC.rotation;
        private void Move(Vector2 position, float speed)
        {
            Vector2 direction = NPC.DirectionTo(position);

            Vector2 velocity = direction * speed;

            NPC.velocity = Vector2.SmoothStep(NPC.velocity, velocity, 0.2f);
        }
        public override bool CheckActive()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<WoodenWardenBoss>()))
                NPC.active = false;

            return base.CheckActive();
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int num2 = 0; num2 < 30; num2++)
                {
                    int num = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Dirt, 0f, 0f, 125, default, 0.65f);
                    Main.dust[num].velocity *= 3f;
                }
            }
        }
    }
}
