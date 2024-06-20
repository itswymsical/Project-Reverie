using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReverieMod.Helpers;
using ReverieMod.Content.NPCs.Boss.Warden;

namespace ReverieMod.Content.NPCs.Bosses.Warden
{
	[AutoloadBossHead]
	public class WardenArm : ModNPC
	{
        public override string Texture => Assets.NPCs.Warden + Name;
        public enum ArmAIState
        {
            Idle,
            Grabbing,
            Dashing,
            Laser,
            Grabbed
        }
        private enum ArmType
        {
            Left,
            Right
        }
        private ArmAIState State
        {
            get => (ArmAIState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public float AITimer
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        private ArmType Arm
        {
            get => (ArmType)NPC.localAI[0];
            set => NPC.localAI[0] = (float)value;
        }

        private bool flagged;
        private int dashCount;
        private int laserShots;
        private Vector2[] bezierPoints; 
        private int hitsTaken;
        private Player grabbedPlayer;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 4;

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
		}

		public override void FindFrame(int frameHeight)
		{
            NPC.spriteDirection = (Arm == ArmType.Left) ? -1 : 1;
            /*
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
			}*/
        }
        public override void AI()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<WoodenWarden>()))
            {
                NPC.active = false;
                return;
            }

            Player target = Main.player[NPC.target];

            // Flip sprite based on arm type
            NPC.spriteDirection = (Arm == ArmType.Left) ? -1 : 1;

            switch (State)
            {
                case ArmAIState.Idle:
                    IdleHover();
                    break;
                case ArmAIState.Grabbing:
                    Grabbing(target);
                    break;
                case ArmAIState.Dashing:
                    Dashing(target);
                    break;
                case ArmAIState.Laser:
                    LaserAttack(target);
                    break;
                case ArmAIState.Grabbed:
                    Grabbed(target);
                    break;
            }

            AITimer++;
            if (AITimer >= 300) // Adjust timing as needed
            {
                if (State == ArmAIState.Idle)
                {
                    if (Arm == ArmType.Left)
                    {
                        State = ArmAIState.Grabbing;
                    }
                    else if (Arm == ArmType.Right && !NPC.AnyNPCs(ModContent.NPCType<WardenArm>())) // Ensure the left arm has finished grabbing
                    {
                        State = ArmAIState.Dashing;
                    }
                }
                else if (State == ArmAIState.Grabbing || State == ArmAIState.Laser || State == ArmAIState.Dashing)
                {
                    State = ArmAIState.Idle;
                }
                AITimer = 0;
            }
        }

        private void IdleHover()
        {
            Vector2 hoverPosition = Main.npc[NPC.target].Center + new Vector2((Arm == ArmType.Left ? -1 : 1) * 100, 0); // Hover 100 pixels to the left or right
            NPC.velocity = (hoverPosition - NPC.Center) * 0.1f; // Smoothly move to hover position
        }

        private void Grabbing(Player target)
        {
            if (AITimer == 0)
            {
                // Set bezier points for grabbing path
                bezierPoints = new Vector2[4];
                bezierPoints[0] = NPC.Center;
                bezierPoints[1] = NPC.Center + new Vector2(-50, -50);
                bezierPoints[2] = target.Center + new Vector2(50, -50);
                bezierPoints[3] = target.Center;
            }

            float t = AITimer / 60f; // Adjust speed as needed
            NPC.Center = BezierCurve(t, bezierPoints);

            if (AITimer >= 60)
            {
                grabbedPlayer = target;
                grabbedPlayer.velocity = Vector2.Zero;
                grabbedPlayer.position = NPC.Center;
                State = ArmAIState.Grabbed;
                AITimer = 0;
            }
        }

        private void Grabbed(Player target)
        {
            target.position = NPC.Center;

            if (hitsTaken >= 3)
            {
                ReleasePlayer();
                State = ArmAIState.Idle;
                hitsTaken = 0;
            }
            else
            {
                AITimer++;
                if (AITimer >= 180) // 3 seconds
                {
                    target.Hurt(Terraria.DataStructures.PlayerDeathReason.ByNPC(NPC.whoAmI), 150, 0);
                    State = ArmAIState.Idle;
                    hitsTaken = 0;
                }
            }
        }

        public override bool CheckActive()
        {
            return false; // Prevent despawning
        }

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return State == ArmAIState.Grabbed;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return State == ArmAIState.Grabbed;
        }
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            hitsTaken++;
            ShakeEffect();
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            hitsTaken++;
            ShakeEffect();
        }

        private void ShakeEffect()
        {
            Vector2 shakeOffset = new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-3, 4)) * hitsTaken;
            NPC.position += shakeOffset;
        }

        private void ReleasePlayer()
        {
            grabbedPlayer.velocity = Vector2.Zero;
            grabbedPlayer = null;
        }

        private void Dashing(Player target)
        {
            if (AITimer < 60)
            {
                // Charge up
                NPC.velocity = Vector2.Zero;
            }
            else if (AITimer == 60)
            {
                // Dash towards player
                Vector2 dashDirection = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                NPC.velocity = dashDirection * 20f;
            }
            else if (AITimer > 60 && NPC.collideX)
            {
                // Explode into splinters on tile collide
                Explode();
                NPC.active = false;
                Main.npc[ModContent.NPCType<WoodenWarden>()].localAI[3] = 1f; // Flag to spawn new arm
            }
        }

        private void LaserAttack(Player target)
        {
            if (AITimer == 0)
            {
                // Initial tracking
                laserShots = 0;
            }

            if (AITimer < 60)
            {
                // Track player's Y position
                NPC.Center = new Vector2(NPC.Center.X, target.Center.Y);
            }
            else if (AITimer % 20 == 0 && laserShots < 5)
            {
                // Fire laser
                Vector2 direction = Vector2.UnitX * NPC.spriteDirection;
                Projectile.NewProjectile(default, NPC.Center, direction * 10f, ProjectileID.EyeLaser, 20, 1f, Main.myPlayer);
                laserShots++;
            }

            if (laserShots >= 5)
            {
                State = ArmAIState.Idle;
                AITimer = 0;
            }
        }

        private void Explode()
        {
            // Implement explosion effect
        }

        private Vector2 BezierCurve(float t, Vector2[] points)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector2 p = uuu * points[0];
            p += 3 * uu * t * points[1];
            p += 3 * u * tt * points[2];
            p += ttt * points[3];

            return p;
        }
    }
}
