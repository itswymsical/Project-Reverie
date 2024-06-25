using Microsoft.Xna.Framework;
using ReverieMod.Common.Players;
using ReverieMod.Content.NPCs.Boss.Warden;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ReverieMod.Content.NPCs.Bosses.Warden
{
    [AutoloadBossHead]
	public class WardenArm : ModNPC
	{
        public override string Texture => Assets.NPCs.Warden + Name;
        public enum AIState
        {
            Idle,
            Grabbing,
            Dashing,
            Laser,
            Deterred,
            Grabbed
        }
        private enum ArmType
        {
            Left,
            Right
        }
        private AIState State
        {
            get => (AIState)NPC.ai[0];
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
        private int BeamTimer;
        private int DashTimer; //attack intervals
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

			NPC.damage = 18;
			NPC.defense = 12;
			NPC.lifeMax = 520;

			NPC.noTileCollide = true;
			NPC.noGravity = true;
			NPC.lavaImmune = true;
			NPC.netUpdate = true;

			NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle($"{nameof(ReverieMod)}/Assets/SFX/WoodenHit")
            {
                Volume = 1.4f,
                PitchVariance = 0.2f,
                MaxInstances = 3,
            };
            NPC.DeathSound = SoundID.Item14;
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
            if (!NPC.AnyNPCs(ModContent.NPCType<WoodenWarden>()))
                NPC.active = false;

            return base.CheckActive();
        }
        public override void FindFrame(int frameHeight)
		{
            NPC.spriteDirection = (Arm == ArmType.Left) ? -1 : 1;
            
			if (State == AIState.Laser)
			{
				NPC.frame.Y = 1 * frameHeight;
			}
			else if (State == AIState.Grabbing)
			{
				NPC.frame.Y = 0 * frameHeight;
			}
            else
            {
                NPC.frame.Y = 2 * frameHeight;
            }
        } 
        public override void AI()
        {
            NPC boss = Main.npc[(int)NPC.ai[3]];
            Player target = Main.player[NPC.target];
            NPC.TargetClosest(true); 
            switch (State)
            {
                case AIState.Idle:
                    IdleHover();
                    break;
                case AIState.Grabbing:
                    Grabbing();
                    break;
                case AIState.Dashing:
                    Dashing();
                    break;
                case AIState.Laser:
                    LaserAttack(target);
                    break;
                case AIState.Deterred:
                    Deterred();
                    break;
                case AIState.Grabbed:
                    CrushPlayer(target);
                    break;
            }

            AITimer++;
            if (AITimer >= 300) // Adjust timing as needed
            {
                if (State == AIState.Idle)
                {
                    if (Arm == ArmType.Left)
                    {
                        if (Main.rand.NextBool(2))
                            State = AIState.Laser;

                        else
                            State = AIState.Grabbing;
                    }
                    else if (Arm == ArmType.Right)
                    {
                        if (Main.rand.NextBool(2))
                            State = AIState.Laser;
                        
                        else
                            State = AIState.Dashing;                       
                    }
                }
                else if (State == AIState.Grabbing || State == AIState.Laser || State == AIState.Dashing || State == AIState.Grabbed)
                {
                    State = AIState.Idle;
                }
                AITimer = 0;
            }
        }

        private void IdleHover()
        {
            Player target = Main.player[NPC.target];
            var angle = target.Center - NPC.Center;
            NPC.rotation = angle.ToRotation() + MathHelper.PiOver2;
            angle.Normalize();
            angle.X *= 3f;
            angle.Y *= 3f;

            NPC boss = Main.npc[(int)NPC.ai[3]];

            var position = boss.Center - new Vector2(160, -80);
            if (Arm == ArmType.Right)
            {
                position = boss.Center - new Vector2(-160, -80);
            }
            float speed = Vector2.Distance(NPC.Center, position);
            speed = MathHelper.Clamp(speed, -14f, 14f);
            Move(position, speed);
        }

        private void Grabbing()
        {
            NPC.damage = 0;
            Player target = Main.player[NPC.target];
            var angle = target.Center - NPC.Center;
            Vector2 point = new Vector2(target.Center.X, target.Center.Y);
            NPC.rotation = angle.ToRotation() + MathHelper.PiOver2;
            angle.Normalize();
            angle.X *= 3f;
            angle.Y *= 3f;

            float speed = Vector2.Distance(NPC.Center, point);
            speed = MathHelper.Clamp(speed, -10f, 10f);
            Move(point, speed);

            if (AITimer >= 180)
            {
                NPC.knockBackResist = 0f;
                State = AIState.Deterred;
                ShakeEffect(5);
                AITimer = 0;
            }
            if (NPC.Hitbox.Intersects(target.Hitbox))
            {
                State = AIState.Grabbed;
                AITimer = 0;
            }
        }

        private void Deterred()
        {
            NPC.position += new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-3, 4)); // Shake effect
            NPC.velocity.Y = 0f;
            NPC.velocity.X = 0;
            NPC boss = Main.npc[(int)NPC.ai[3]];

            if (AITimer >= 180) // 3 seconds
            {
                SoundEngine.PlaySound(SoundID.Item32, NPC.position);
                State = AIState.Idle;
                AITimer = 0;
            }
        }
        public static string CrushQuotes(Player target)
        {
            string gender = "";
            if (target.Male)
                gender = "his";
            
            else if (!target.Male)
                gender = "her";
            
            else
                gender = "their";
            

            string[] OverloadQuotes = new string[]
            {
                " was crushed into a pool of blood.",
                " was turned into a ball of bramble.",
                " was brutally squeezed to death.",
                " couldn't break free.",
                " couldn't wiggle " + gender + " way out this one."
            };
            
            int randomQuote = Main.rand.Next(OverloadQuotes.Length);

            return OverloadQuotes[randomQuote];
        }
        private void CrushPlayer(Player target)
        {
            NPC.Center = target.Center;
            target.velocity.Y -= 0.4f;
            target.velocity.X *= 0f;

            if (NPC.justHit)
            {
                hitsTaken += 1;               
                CombatText.NewText(NPC.Hitbox, Color.Red, $"Hits: {hitsTaken}!", true);
            }
            if (!Main.masterMode)
            {
                if (hitsTaken >= 5)
                {
                    State = AIState.Deterred;
                    hitsTaken = 0;
                }
            }
            else
            {
                if (hitsTaken >= 7)
                {
                    State = AIState.Deterred;
                    hitsTaken = 0;
                }
            }

            Main.player[NPC.target].GetModPlayer<ReveriePlayer>().ScreenShakeIntensity = Math.Abs(AITimer / 48f);
            if (AITimer > 240)
            {              
                if (!Main.masterMode)
                {
                    target.Hurt(PlayerDeathReason.ByCustomReason(target.name + CrushQuotes(target)), 100, 0);
                }
                else
                {
                    target.Hurt(PlayerDeathReason.ByCustomReason(target.name + CrushQuotes(target)), 150, 0);
                }
                for (int num = 0; num < 12; num++)
                {
                    Dust.NewDust(target.Center, NPC.width, NPC.height, DustID.Blood, newColor: Color.DarkRed, Scale: 2f);
                }
                SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                AITimer = 0;
                State = AIState.Idle;
            }
        }
        private void ShakeEffect(int value)
        {
            Vector2 shakeOffset = new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)) * value;
            NPC.position += shakeOffset;
        }
        private void Dashing()
        {
            Player target = Main.player[NPC.target];
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
                NPC.velocity = PlayerPosition * 8.5f;
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
        private void LaserAttack(Player target)
        {
            BeamTimer++;
            var angle = target.Center - NPC.Center;
            NPC.rotation = angle.ToRotation() + MathHelper.PiOver2;
            angle.Normalize();
            angle.X *= 3f;
            angle.Y *= 3f;

            var position = target.Center - new Vector2(-200, 70);
            if (Arm == ArmType.Right)
            {
                position = target.Center - new Vector2(200, 70);
            }
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

    }
}
