using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Trelamium.Common.Players;

namespace Trelamium.Content.NPCs.Boss.Fungore
{
    [AutoloadBossHead]
    public class Fungore : ModNPC
    {
        public override string Texture => Trelamium.AssetPath + "NPCs/Boss/Fungore/Fungore";
        private enum States
        {
            Walking,
            Punching,
            Jumping,
            SuperJumping
        }

        private States State
        {
            get => (States)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        private float AttackCooldown
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        private float TimeGrounded
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }

        private Player player;

        private int frameY;
        private int frameX;

        private int punchDirection;

        private Vector2 scale = Vector2.One;

        private float alpha;
        private float alphaTimer;
        private int AITimer;
        private bool flag = false;
        public override void SetStaticDefaults(){
            Main.npcFrameCount[NPC.type] = 16;
        }
        public override void SetDefaults()
        {
            NPC.damage = 12;
            NPC.defense = 7;
            NPC.lifeMax = 1640;

            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.knockBackResist = 0f;

            NPC.aiStyle = -1;
            AIType = -1;

            NPC.value = Item.buyPrice(gold: 1);

            NPC.HitSound = SoundID.DD2_OgreHurt;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.width = NPC.height = 88;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = State == States.Punching ? punchDirection : NPC.direction;

            NPC.frame.Width = 138;
            NPC.frame.Height = 134;
            NPC.frameCounter++;

            int frameRate = 4;
            // Make the actual punch last slightly longer.
            if (State == States.Punching && (frameY == 5 || frameY == 6))
            {
                frameRate = 15;
            }
            if (State == States.Jumping && frameY == 6)
            {
                frameRate = 35;
            }
            if (State == States.SuperJumping && frameY == 2)
            {
                frameRate = 35;
            }
            if (State == States.SuperJumping && (frameY == 6 || frameY == 7 || frameY == 8))
            {
                frameRate = 25;
            }
            if (State == States.SuperJumping && frameY == 9)
            {
                frameRate = 30;
            }
            if (NPC.frameCounter > frameRate)
            {
                frameY++;

                NPC.frameCounter = 0;
            }

            frameX = State == States.Walking ? 0 : State == States.Punching ? 1 : State == States.Jumping ? 2 : 3;

            if (State == States.Walking && frameY > 7)
            {
                frameY = 0;
            }
            if (State == States.Jumping && frameY > 11)
            {
                frameY = 0;
            }
            if (State == States.SuperJumping && frameY > 15)
            {
                frameY = 0;
            }
            NPC.frame.Y = frameY * frameHeight;
            NPC.frame.X = frameX * NPC.frame.Width;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            //NPC.lifeMax = (int)(NPC.lifeMax * 0.525f * bossAdjustment);
            float damage = NPC.damage;
            NPC.damage += (int)(damage * .15f);
        }
        
        public override void AI() => HandleAll();
        private void HandleAll()
        {
            HandleStates();
            HandleAttacks();
            HandleDespawn();
            HandleCollision(60, 6f);
            HandleScale();
        }

        private void HandleStates()
        {
            NPC.TargetClosest();

            player = Main.player[NPC.target];

            switch (State)
            {
                case States.Walking:
                    Walk();
                    break;

                case States.Punching:
                    Punch();
                    break;

                case States.Jumping:
                    Jump();
                    break;

                case States.SuperJumping:
                    SuperJump();
                    break;
            }
        }
        private void CheckPlatform(Player player) // credits to Spirit Mod :sex:
        {
            bool onplatform = true;
            for (int i = (int)NPC.position.X; i < NPC.position.X + NPC.width; i += NPC.height / 2)
            {
                Tile tile = Framing.GetTileSafely(new Point((int)NPC.position.X / 16, (int)(NPC.position.Y + NPC.height + 8) / 16));
                if (!TileID.Sets.Platforms[tile.TileType])
                    onplatform = false;
            }
            if (onplatform && (NPC.Center.Y < player.position.Y - 20))
                NPC.noTileCollide = true;
            else
                NPC.noTileCollide = false;
        }
        private void HandleAttacks()
        {
            const int minAttackCooldown = 180;
            CheckPlatform(player);
            AttackCooldown++;
            // Made a attack cooldown to avoid stuff as immediately doing certain attack out of nowhere, etc...
            if (AttackCooldown > minAttackCooldown)
            {
                const float minPunchDistance = 120;
                // Punch if the player is close enough to Fungore.
                if (player.Distance(NPC.Center) < minPunchDistance && (Main.rand.NextBool(20)) || (Main.rand.NextBool(52)))
                {
                    punchDirection = Math.Sign(player.position.X - NPC.position.X);

                    frameY = 0; // Make sure to reset the frame. Will cause weird looks if you dont.

                    State = States.Punching;
                    AttackCooldown = 0;
                }
                if (player.Distance(NPC.Center) > 220f && (Main.rand.NextBool(40)) || (Main.rand.NextBool(70)))
                {
                    frameY = 0;

                    State = States.SuperJumping;
                    AttackCooldown = 0;
                }

                if (NPC.velocity.X == 0 && AttackCooldown > 60)
                {
                    State = States.SuperJumping;
                    AttackCooldown = 0;
                }
                if (Main.rand.NextBool(48))
                {
                    frameY = 0;

                    State = States.Jumping;
                    AttackCooldown = 0;
                }
            }
        }
        private void HandleDespawn()
        {
            if (!player.active || player.dead)
            {
                flag = true;
                State = States.SuperJumping;
                if (++AITimer == 220)
                {
                    NPC.active = false;
                    NPC.TargetClosest(false);
                }
            }
        }
        private void HandleCollision(int maxTime, float jumpHeight)
        {
            if (NPC.velocity.Y == 0f)
            {
                TimeGrounded++;
            }
            else
            {
                TimeGrounded = 0;
            }

            if (TimeGrounded > maxTime && NPC.collideX && NPC.position.X == NPC.oldPosition.X)
            {
                NPC.velocity.Y = -jumpHeight;
            }

            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
        }
        private void Walk()
        {
            const float maxSpeed = 2.7f;

            if (NPC.velocity.X < -maxSpeed || NPC.velocity.X > maxSpeed)
            {
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity *= 0.8f;
                }
            }
            else
            {
                if (NPC.velocity.X < maxSpeed && NPC.direction == 1)
                {
                    NPC.velocity.X += 0.07f;
                }

                if (NPC.velocity.X > -maxSpeed && NPC.direction == -1)
                {
                    NPC.velocity.X -= 0.07f;
                }

                NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -maxSpeed, maxSpeed);
            }
            Dust.NewDust(NPC.oldPosition, NPC.width, NPC.height, DustID.OrangeTorch, NPC.oldVelocity.X, NPC.oldVelocity.Y, 0, default, 1f);
        }
        private void Leap()
        {
            const float leapVelocity = 8.5f;
            NPC.noTileCollide = true;
            if (NPC.velocity.X < -leapVelocity || NPC.velocity.X > leapVelocity)
            {
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity *= 3f;
                }
            }
            else
            {
                if (!(frameY >= 6 && State == States.SuperJumping))
                {
                    if (NPC.velocity.X < leapVelocity && NPC.direction == 1)
                    {
                        NPC.velocity.X += 2f;
                    }

                    if (NPC.velocity.X > -leapVelocity && NPC.direction == -1)
                    {
                        NPC.velocity.X -= 2f;
                    }
                }
                NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -leapVelocity, leapVelocity);
            }
        }
        private void Punch()
        {
            if (frameY == 3)
                SoundEngine.PlaySound(SoundID.DD2_OgreAttack, NPC.position);

            if (frameY == 4 || frameY == 5)
            {
                NPC.velocity.X += punchDirection * 1.75f;
                NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -6f, 6f);
            }
            else
            {
                NPC.velocity.X *= 0.95f;
            }

            if (NPC.velocity.X != 0f)
            {
                var scaleMult = Math.Abs(NPC.velocity.X) * 0.05f;

                scale.X += scaleMult;

                if (scale.X > 1.75f)
                {
                    scale.X -= scaleMult;
                }
            }

            // Go back to walking after finishing punching or if it collides with a side tile.
            if (frameY > 10 || NPC.collideX)
            {
                frameY = 0; // Make sure to reset the frame. Will cause weird looks if you dont.
                State = States.Walking;
            }
        }
        private void Jump()
        {
            Walk();
            if (frameY > 2 && frameY < 4)
            {
                NPC.velocity.Y = Main.rand.NextFloat(-9f, -8f);
                NPC.TargetClosest();
                NPC.netUpdate = true;
                if (NPC.velocity.Y >= 0f)
                {
                    Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 1);
                }
            }
            if (frameY == 4)
                SoundEngine.PlaySound(SoundID.DD2_OgreAttack, NPC.position);

            if (frameY >= 8 && (NPC.collideY || NPC.collideX))
            {
                SoundEngine.PlaySound(SoundID.DD2_OgreGroundPound, NPC.position);
                Main.LocalPlayer.GetModPlayer<TPlayer>().ScreenShakeIntensity = 3f;

                //Projectile.NewProjectile(spawnSource: default, new Vector2(NPC.Center.X, NPC.Center.Y + 40), new Vector2(0), ModContent.ProjectileType<Projectiles.FungoreSmoke>(), NPC.damage / 2, 16f, Main.myPlayer);
                for (int i = 0; i < 6; ++i)
                {
                    //var index = Projectile.NewProjectile(spawnSource: default, NPC.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2) * 12f, ModContent.ProjectileType<Mushroom>(), (int)(NPC.damage * 0.25f), 0.5f);
                    //Main.projectile[index].hostile = true;
                }

                frameY = 0;
                State = States.Walking;
            }
            if (frameY > 15 && (NPC.collideY || NPC.collideX))
            {
                SoundEngine.PlaySound(SoundID.DD2_OgreGroundPound, NPC.position);
                frameY = 0;
                State = States.Walking;
            }
        }
        private void SuperJump()
        {
            if (frameY < 4)
            {
                NPC.velocity.X = 0;
            }
            else
            {
                Leap();
            }
            if (frameY > 2 && frameY < 4)
            {
                NPC.noTileCollide = false;
                NPC.velocity.Y = Main.rand.NextFloat(-13f, -12f);
                NPC.TargetClosest();
                NPC.netUpdate = true;
                if (NPC.velocity.Y >= 0f)
                {
                    Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 1);
                }
                if (flag)
                {
                    NPC.velocity.Y = Main.rand.NextFloat(-20f, -20f);
                }
            }

            if (frameY == 4)
                SoundEngine.PlaySound(SoundID.DD2_OgreAttack, NPC.position);

            if (frameY >= 8 && (NPC.collideY || NPC.collideX))
            {
                SoundEngine.PlaySound(SoundID.DD2_OgreGroundPound, NPC.position);
                Main.LocalPlayer.GetModPlayer<TPlayer>().ScreenShakeIntensity = 5f;

                //Projectile.NewProjectile(default, NPC.position, new Vector2(0), ModContent.ProjectileType<Projectiles.FungoreSlam>(), NPC.damage / 2, 16f, Main.myPlayer);
                for (int i = 0; i < 12; ++i)
                {
                    //var index = Projectile.NewProjectile(default, NPC.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2) * 12f, ModContent.ProjectileType<Mushroom>(), (int)(NPC.damage * 0.25f), 0.5f);
                    //Main.projectile[index].hostile = true;
                }
                if (Main.rand.NextBool(5))
                {
                    for (int i = 0; i < 2; ++i)
                    {
                        //NPC.NewNPC(default, (int)NPC.position.X + Main.rand.Next(-i * -20, i * 20), (int)NPC.oldPosition.Y, ModContent.NPCType<Enemies.Forest.MushroomSlime>(), 0, i);
                    }
                }

                frameY = 0;
                State = States.Walking;
            }
            if (frameY > 15 && (NPC.collideY || NPC.collideX))
            {
                SoundEngine.PlaySound(SoundID.DD2_OgreGroundPound, NPC.position);
                frameY = 0;
                State = States.Walking;
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;

            return null;
        }
        private void HandleScale()
        {
            Vector2 targetScale = Vector2.One;

            if (scale.Y != targetScale.Y)
            {
                scale.Y = MathHelper.Lerp(scale.Y, targetScale.Y, 0.33f);
            }

            if (scale.X != targetScale.X)
            {
                scale.X = MathHelper.Lerp(scale.X, targetScale.X, 0.33f);
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) {
                /*for (int i = 1; i <= 5; i++) {
                    Gore.NewGore(NPC.Center, NPC.velocity, Mod.GetGoreSlot("Gores/Fungore/FungoreGore" + i));
                }*/
            }

            for (int i = 0; i < 28; i++)
                Dust.NewDust(NPC.Center, NPC.width, NPC.height, DustID.MushroomTorch, hit.HitDirection, .5f);
        }
    }
}
