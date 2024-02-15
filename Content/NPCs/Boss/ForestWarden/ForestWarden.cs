using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ReverieMod.Content.NPCs.Boss.ForestWarden
{
    public class ForestWarden : ModNPC
    {
        private enum States
        {
            Follow,
            Slam,
            SummonFist,
            SummonFoot
        }
        private States State
        {
            get => (States)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        private Player player;
        public override string Texture => Assets.NPCs.ForestWarden + Name;
        public override void SetDefaults()
        {
            NPC.damage = 1;
            NPC.defense = 17;
            NPC.lifeMax = 5765;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.width = 200;
            NPC.height = 194;
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
                NPC.life += (int)(bossAdjustment * .2f);
            }
            if (Main.masterMode)
            {
                NPC.damage += (int)(damage * .35f);

                NPC.life += (int)(bossAdjustment * .35f);
                NPC.defense = 17;
            }
        }
        public override void AI()
        {
            HandleStates();
        }
        private void HandleStates()
        {
            NPC.TargetClosest();

            player = Main.player[NPC.target];

            switch (State)
            {
                case States.Follow:
                    Follow();
                    break;

                case States.Slam:
                    //Punch();
                    break;

                case States.SummonFist:
                    //Jump();
                    break;

                case States.SummonFoot:
                    //SuperJump();
                    break;
            }
        }
        private void Follow()
        {
            const float maxSpeed = 5.125f;

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
    }
}
