using Microsoft.Xna.Framework.Graphics;
using ReverieMod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using ReverieMod.Common.Players;
using Terraria.Audio;
using ReverieMod.Content.NPCs.Boss.Fungore.Projectiles;
using static ReverieMod.Content.NPCs.Boss.WoodenWarden.WoodenWardenBoss;

namespace ReverieMod.Content.NPCs.Boss.Shelledrake
{
    [AutoloadBossHead]
    public class Shelledrake : ModNPC
    {
        private enum AIState
        {
            Walk,
            Roll,
            MortarStrike,
            Stomp,
            Despawn
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
        private Player player;
        private bool stomped = false;

        private float textAlpha;
        private float textAlphaTimer;
        private int bossDefense = 17;
        public override string Texture => Assets.NPCs.Shelledrake + Name;
        public override void SetDefaults()
        {
            NPC.life = 2450;
            NPC.defense = bossDefense;
            NPC.damage = 26;

            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;       

            NPC.value = Item.buyPrice(gold: 5);
            NPC.width = 185;
            NPC.height = 98;
            NPC.HitSound = SoundID.NPCHit18;
            NPC.DeathSound = SoundID.NPCDeath12;
        }
        public override void AI()
        {
            NPC.TargetClosest();

            player = Main.player[NPC.target];
            
            HandleStates();
            Walk();
            AITimer++;
            if (AITimer == 600)
            {
                State = AIState.Stomp;
                AITimer = 0;
            }
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
        private void HandleStates()
        {
            NPC.TargetClosest();

            player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                State = AIState.Despawn;
                AITimer++;
                if (State == AIState.Despawn && AITimer >= 200)
                {
                    NPC.active = false;
                    NPC.TargetClosest(false);
                }
            }
            switch (State)
            {
                case AIState.Walk:
                    Walk();
                    break;             
            case AIState.Roll:
                Roll();
                break;
            case AIState.MortarStrike:
                MortarStrike();
                break;             
                case AIState.Stomp:
                    Stomp();
                    break;
                case AIState.Despawn:
                    break;
            }
        }
        private void Walk()
        {
            const float maxSpeed = 3.0125f;

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
        }
        private void Roll()
        { }
        private void Stomp()
        {
            for (int i = 5; i < 25; i++) {
                NPC.velocity.Y = Main.rand.NextFloat(-6f, -5f);
                NPC.TargetClosest();
                NPC.netUpdate = true;

                if (NPC.velocity.Y >= 0f) {
                    Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 1);
                }
                if (i == 25) {
                    stomped = true;
                }
            }

            if (!stomped) {
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.position);
                Main.LocalPlayer.GetModPlayer<ReveriePlayer>().ScreenShakeIntensity = .765f;
                Projectile.NewProjectile(default, NPC.position, new Vector2(0), ModContent.ProjectileType<FungoreSmoke>(), NPC.damage, 16f, Main.myPlayer);             
            }
        }
        private void MortarStrike()
        { }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;

            return null;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => HandleScreenText(spriteBatch);
        private void HandleScreenText(SpriteBatch spriteBatch)
        {
            var position = new Vector2(Main.screenWidth / 2f, 200f);
            var position2 = new Vector2(Main.screenWidth / 2f, -400f);
            Color color = Color.Orange * textAlpha;

            textAlphaTimer++;

            if (textAlphaTimer < 180)
            {
                textAlpha += 0.025f;
            }
            else
            {
                textAlpha -= 0.025f;
            }

            // do these actually disappear? could be an issue for memory usage.
            Helper.DrawText(spriteBatch, position, "- Shelledrake -", color);
            Helper.DrawText(spriteBatch, position2, "- Emberite Porcudillo -", color);
        }
    }
}