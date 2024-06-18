using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReverieMod.Common.Players;
using ReverieMod.Content.NPCs.Bosses.WoodenWarden;
using ReverieMod.Helpers;
using System;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace ReverieMod.Content.NPCs.Boss.WoodenWarden
{
    [AutoloadBossHead]
    public class WoodenWardenBoss : ModNPC
    {
        public enum AIState
        {
            Moving,
            SlamPhase,
            SpawnArms
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
        private Player target;
        private float alpha;
        private float alphaTimer;
        private int count;
        private int bossDefense = 17;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Sets the description of this NPC that is listed in the bestiary
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> { new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("The Protector of the Ligneous Temple. The Wooden Warden is a sentient golem ordered to protect the gateways that connect to otherworlds.")
            });
        }
        public override string Texture => Assets.NPCs.WoodenWarden + Name;
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
            {
                Music = MusicLoader.GetMusicSlot(Mod, Assets.Music + "LigneousWarden");
            }
            NPC.HitSound = SoundID.NPCHit18;
            NPC.DeathSound = SoundID.NPCDeath12;
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
            State = AIState.Moving;
            target = Main.player[NPC.target];
            NPC.TargetClosest(true);       
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
            if (!handsSpawned)
            {
                var Left = NPC.NewNPC(default, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WardenArmLeft>());
                Main.npc[Left].ai[0] = NPC.whoAmI;
                var Right = NPC.NewNPC(default, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WardenArmRight>());
                Main.npc[Right].ai[0] = NPC.whoAmI;
                handsSpawned = true;
            }
            if (NPC.AnyNPCs(ModContent.NPCType<WardenArmLeft>()) || NPC.AnyNPCs(ModContent.NPCType<WardenArmRight>()))
            {
                NPC.defense = 100;
            }
            else
            {
                NPC.defense = bossDefense;
                State = AIState.SlamPhase;
            }
            if (State == AIState.Moving)
            {
                Movement();
            }
            if (State == AIState.SlamPhase)
            {

                AITimer++;
                if (AITimer < 240)
                {
                    Movement();
                }
                if (AITimer >= 190 && AITimer <= 380)
                {
                    Slam();
                }
                if (AITimer >= 380)
                {
                    AITimer = 0;
                    count += 1;
                }
                if (count == 5)
                {
                    State = AIState.SpawnArms;
                }
            }
            if (State == AIState.SpawnArms)
            {
                var Left = NPC.NewNPC(default, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WardenArmLeft>());
                Main.npc[Left].ai[0] = NPC.whoAmI;
                var Right = NPC.NewNPC(default, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WardenArmRight>());
                Main.npc[Right].ai[0] = NPC.whoAmI;
                State = AIState.Moving;
            }
        }
        private void Move(Vector2 position, float speed)
        {
            Vector2 direction = NPC.DirectionTo(position);

            Vector2 velocity = direction * speed;

            NPC.velocity = Vector2.SmoothStep(NPC.velocity, velocity, 0.2f);
        }
        private void Movement()
        {
            NPC.noTileCollide = true;
            var position = new Vector2(target.Center.X, target.Center.Y - 290);
            if (NPC.AnyNPCs(ModContent.NPCType<WardenArmLeft>()) || NPC.AnyNPCs(ModContent.NPCType<WardenArmRight>()))
            {
                position = new Vector2(target.Center.X, target.Center.Y - 320);
            }
            float speed = Vector2.Distance(NPC.Center, position);
            speed = MathHelper.Clamp(speed, -6f, 6f);
            Move(position, speed);
            NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.01f, 0.07f);
        }
        private void Slam()
        {
            NPC.noTileCollide = false;

            if (!slamming)
            {
                NPC.damage = 0;
                NPC.ai[2]++;
                NPC.velocity.X = 0;
                if (NPC.position.Y > NPC.Center.Y)
                {
                    NPC.velocity.Y -= 0.1f;
                }
                else
                {
                    NPC.velocity.Y *= 0.8f;
                }
                slamming = true;
                NPC.ai[2] = 0;
            }
            else
            {
                NPC.damage = 100;
                NPC.ai[2] += 0.09f;

                NPC.velocity.Y += NPC.ai[2];
            }
            if (NPC.collideY && NPC.position.Y == NPC.oldPosition.Y)
            {
                if (slamming)
                {
                    SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                }
                CheckPlatform(target);
                Collision.HitTiles(NPC.position, NPC.velocity, NPC.width, NPC.height);
                NPC.ai[2] = 0;
                slamming = false;
                State = AIState.Moving;
                Main.player[NPC.target].GetModPlayer<ReveriePlayer>().ScreenShakeIntensity = Math.Abs(NPC.velocity.Y * 3.85f);
            }
        }
        private void CheckPlatform(Player player) // Spirit Mod :kek:
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
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => HandleScreenText(spriteBatch);
        private void HandleScreenText(SpriteBatch spriteBatch)
        {
            var position = new Vector2(Main.screenWidth / 2f, Main.screenHeight / 8f);
            var position2 = new Vector2(Main.screenWidth / 2f, Main.screenHeight / 1.5f);
            Color color = Color.White * alpha;

            alphaTimer++;

            if (alphaTimer < 180)
                alpha += 0.025f;
            
            else
                alpha -= 0.025f;
            

            Helper.DrawText(spriteBatch, position, "〈 Wooden Warden 〉", color);
        }
    }
}
