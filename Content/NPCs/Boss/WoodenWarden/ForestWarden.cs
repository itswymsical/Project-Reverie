using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReverieMod.Common.Players;
using ReverieMod.Content.NPCs.Bosses.WoodenWarden;
using ReverieMod.Helpers;
using System;
using System.Threading;
using Terraria;
using Terraria.Audio;
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
        private bool slamming;
        private bool firstSpawn;

        private Player player;

        private float alpha;
        private float alphaTimer;

        private int bossDefense = 17;
        public override string Texture => Assets.NPCs.WoodenWarden + Name;
        public override void SetDefaults()
        {
            NPC.damage = 0;
            NPC.defense = bossDefense;
            NPC.lifeMax = 3765;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.width = 200;
            NPC.height = 184;
            NPC.aiStyle = -1;

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
            player = Main.player[NPC.target];
            NPC.TargetClosest();
            if (player.dead || !player.active)
            {
                NPC.TargetClosest(false);
                NPC.velocity = new Vector2(0f, -10f);
                NPC.timeLeft--;
                if (NPC.timeLeft <= 0)
                {
                    NPC.active = false;
                }
            }
            SpawnLimbs();
            var position = new Vector2(player.Center.X, player.Center.Y - 320);

            float speed = Vector2.Distance(NPC.Center, position);
            speed = MathHelper.Clamp(speed, -9f, 9f);
            Move(position, speed);
            NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.01f, 0.1f);
        }
     
        private void SpawnLimbs()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<WardenArmLeft>()) && !NPC.AnyNPCs(ModContent.NPCType<WardenArmRight>()) && !firstSpawn)
            {
                firstSpawn = true;
                var Left = NPC.NewNPC(default, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WardenArmLeft>());
                Main.npc[Left].ai[0] = NPC.whoAmI;

                var Right = NPC.NewNPC(default, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WardenArmRight>());
                Main.npc[Right].ai[0] = NPC.whoAmI;
            }
        }

        private void Move(Vector2 position, float speed)
        {
            Vector2 direction = NPC.DirectionTo(position);

            Vector2 velocity = direction * speed;

            NPC.velocity = Vector2.SmoothStep(NPC.velocity, velocity, 0.2f);
        }
        private void CheckPlatform(Player player) // Spirit Mod, :sex: - (naka, 2021)
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
            var position = new Vector2(Main.screenWidth / 2f, 200f);
            var position2 = new Vector2(Main.screenWidth / 2f, -400f);
            Color color = Color.White * alpha;

            alphaTimer++;

            if (alphaTimer < 180)
            {
                alpha += 0.025f;
            }
            else
            {
                alpha -= 0.025f;
            }

            // do these actually disappear? could be an issue for memory usage.
            Helper.DrawText(spriteBatch, position, "- Wooden Warden -", color);
            Helper.DrawText(spriteBatch, position2, "- Protector of the Canopy -", color);
        }
    }
}
