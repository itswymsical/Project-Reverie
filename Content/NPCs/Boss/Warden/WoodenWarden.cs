using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReverieMod.Common.Players;
using ReverieMod.Content.NPCs.Bosses.Warden;
using ReverieMod.Helpers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace ReverieMod.Content.NPCs.Boss.Warden
{
    [AutoloadBossHead]
    public class WoodenWarden : ModNPC
    {
        public enum AIState
        {
            Hovering,
            Slamming,
            Recovering
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
        bool handsSpawned;

        private bool slamming;
        private float hoverTimer;
        private float slamTimer;
        private float recoveryTimer;

        private Player target;

        private float alpha;
        private float alphaTimer;

        private int count;
        private int bossDefense = 17;
        public override void SetStaticDefaults() => NPCID.Sets.BossBestiaryPriority.Add(Type);
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Sets the description of this NPC that is listed in the bestiary
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> { new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("The Protector of the Ligneous Temple. The Wooden Warden is a sentient golem ordered to protect the gateways that connect to otherworlds.")
            });
        }
        public override string Texture => Assets.NPCs.Warden + Name;
        public override void SetDefaults()
        {
            NPC.damage = 1;
            NPC.defense = bossDefense;
            NPC.lifeMax = 3765;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.width = 200;
            NPC.height = 184;
            NPC.aiStyle = -1;
            AIType = -1;

            NPC.value = Item.buyPrice(gold: 4);
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.knockBackResist = 0f;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, Assets.Music + "LigneousWarden");
            
            NPC.HitSound = new SoundStyle($"{nameof(ReverieMod)}/Assets/SFX/WoodenHit")
            {
                Volume = 1.4f,
                PitchVariance = 0.2f,
                MaxInstances = 3,
            };
            NPC.DeathSound = new SoundStyle($"{nameof(ReverieMod)}/Assets/SFX/WardenDeath_" + Main.rand.Next(1, 3))
            {
                Volume = 0.9f,
                PitchVariance = 0.2f,
                MaxInstances = 3,
            };
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            float damage = NPC.damage;

            if (Main.expertMode)
            {
                NPC.damage += (int)(damage * .2f);
                bossAdjustment = NPC.life;
                NPC.life += (int)(bossAdjustment * .12f);
            }
            if (Main.masterMode)
            {
                NPC.damage += (int)(damage * .35f);

                NPC.life += (int)(bossAdjustment * .25f);
                bossDefense = 20;
            }
        }
        public override void AI()
        {
            Player target = Main.player[NPC.target];
            NPC.TargetClosest(true);

            switch (State)
            {
                case AIState.Hovering:
                    HoverAbovePlayer(target);
                    break;
                case AIState.Slamming:
                    ChargeSlam(target);
                    break;
                case AIState.Recovering:
                    Recover();
                    break;
            }

            AITimer++;
            if (AITimer >= 600) // Adjust timing as needed
            {
                if (State == AIState.Hovering)
                {
                    State = AIState.Slamming;
                }
                else if (State == AIState.Recovering)
                {
                    State = AIState.Hovering;
                }
                AITimer = 0;
            }
        }
        private void HoverAbovePlayer(Player target)
        {
            Vector2 point = new Vector2(target.Center.X, target.Center.Y - 290);
            float speed = Vector2.Distance(NPC.Center, point);
            speed = MathHelper.Clamp(speed, -6f, 6f);
            Move(point, speed);

            NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.01f, 0.07f);
        }
        private void Move(Vector2 position, float speed)
        {
            Vector2 direction = NPC.DirectionTo(position);

            Vector2 velocity = direction * speed;

            NPC.velocity = Vector2.SmoothStep(NPC.velocity, velocity, 0.2f);
        }
        private void ChargeSlam(Player target)
        {
            
            slamming = true;
            slamTimer++;

            if (slamTimer < 60) // Shake and rise for the first 60 ticks
            {
                NPC.position += new Vector2(Main.rand.Next(-2, 2), Main.rand.Next(-2, 2)); // Shake effect
                NPC.velocity.Y = -0.5f; // Slowly rise
            }
            else if (slamTimer < 120) // Rapidly descend
            {
                NPC.velocity.Y = 20f;
                if (NPC.collideY) // Check if NPC hits the ground
                {
                    SpawnShockwaveProjectiles();
                    slamming = false;
                    slamTimer = 0;
                    State = AIState.Recovering; // New state for recovery period
                }
            }
        }
        private void SpawnShockwaveProjectiles()
        {
            Vector2 position = NPC.Center;
            Vector2 velocityLeft = new Vector2(-10f, 0f); // Adjust speed as needed
            Vector2 velocityRight = new Vector2(10f, 0f);

            //Projectile.NewProjectile(default, position, velocityLeft, ModContent.ProjectileType<ShockwaveProjectile>(), 20, 1f, Main.myPlayer);
            //Projectile.NewProjectile(default, position, velocityRight, ModContent.ProjectileType<ShockwaveProjectile>(), 20, 1f, Main.myPlayer);
        }
        private void Recover()
        {
            if (recoveryTimer < 60) // Recovery period
            {
                NPC.velocity = Vector2.Zero; // Stop movement
            }
            else if (recoveryTimer >= 60 && recoveryTimer < 120) // Slowly return to hovering position
            {
                Vector2 hoverPosition = target.Center - new Vector2(0, 200); // Adjust hover height as needed
                NPC.velocity = (hoverPosition - NPC.Center) * 0.05f; // Smoothly move to hover position
            }
            else
            {
                recoveryTimer = 0;
                State = AIState.Hovering;
            }
            recoveryTimer++;
        }
        private void SpawnArms()
        {
            if (NPC.localAI[3] == 1f) // Check if a new arm needs to be spawned
            {
                int armType = Main.rand.Next(2); // 0 for left, 1 for right
                int armIndex = NPC.NewNPC(default, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WardenArm>());
                NPC arm = Main.npc[armIndex];
                arm.ai[1] = NPC.whoAmI; // Store Warden NPC ID
                arm.localAI[0] = armType; // Set arm type

                if (armType == 0)
                {
                    arm.localAI[1] = 1f; // Flag to prevent looping
                }
                else
                {
                    arm.localAI[1] = 2f; // Flag to prevent looping
                }

                NPC.localAI[3] = 0f; // Reset spawn flag
            }
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => HandleScreenText(spriteBatch);
        private void HandleScreenText(SpriteBatch spriteBatch)
        {
            var position = new Vector2(Main.screenWidth / 2f, Main.screenHeight / 8f);
            // var position2 = new Vector2(Main.screenWidth / 2f, Main.screenHeight / 1.5f);
            Color color = Color.White * alpha;

            alphaTimer++;

            if (alphaTimer < 180)
                alpha += 0.025f;
            
            else
                alpha -= 0.025f;
            

            Helper.DrawText(spriteBatch, position, "〈 Wooden Warden 〉", color, Color.Black);
        }
    }
}
