using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Audio;

namespace ReverieMod.Common.Players
{
    public class ExperiencePlayer : ModPlayer
    {
        public int experienceLevel;
        public int experienceValue;
        public int skillPoints;

        public override void Initialize()
        {
            experienceLevel = 1;
            experienceValue = 0;
            skillPoints = 0;
        }
        public override void SaveData(TagCompound tag)
        {
            tag["experienceLevel"] = experienceLevel;
            tag["experienceValue"] = experienceValue;
            tag["skillPoints"] = skillPoints;
        }
        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("experienceLevel"))
            {
                experienceLevel = tag.GetInt("experienceLevel");
            }
            if (tag.ContainsKey("experienceValue"))
            {
                experienceValue = tag.GetInt("experienceValue");
            }
            if (tag.ContainsKey("skillPoints"))
            {
                skillPoints = tag.GetInt("skillPoints");
            }
        }

        public static void AddExperience(Player player, int value)
        {
            //Player player = Main.LocalPlayer;
            ExperiencePlayer modPlayer = player.GetModPlayer<ExperiencePlayer>();
            if (modPlayer.experienceLevel <= 40) // Max Level.
            {
                modPlayer.experienceValue += value;

                while (modPlayer.experienceValue >= GetNextExperienceThreshold(modPlayer.experienceLevel))
                {
                    modPlayer.experienceValue -= GetNextExperienceThreshold(modPlayer.experienceLevel);
                    modPlayer.experienceLevel++;
                    modPlayer.skillPoints++;
                    SoundEngine.PlaySound(SoundID.AchievementComplete, player.position);
                    Main.NewText($"{player.name} Reached Level {modPlayer.experienceLevel} [i:{ItemID.FallenStar}] , Skill Points: {modPlayer.skillPoints}");
                }
            }
        }
        public static int GetNextExperienceThreshold(int level)
        {
            if (level <= 0)
            {
                return 50; // Initial experience requirement for the first skill point
            }
            return 100 * level;
        }
    }
    public class ExperienceGlobalNPC : GlobalNPC
    {
        public Dictionary<int, int> playerDamage = new Dictionary<int, int>();
        public override bool InstancePerEntity => true;
        private float alpha;
        private float alphaTimer;
        public override void AI(NPC npc)
        {
            // Reset damage tracking for dead NPCs
            if (npc.life <= 0)
            {
                //playerDamage.Clear();
            }
        }
        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (!playerDamage.ContainsKey(player.whoAmI))
            {
                playerDamage[player.whoAmI] = 0;
            }
            playerDamage[player.whoAmI] += hit.Damage;
            if (hit.Damage > npc.life)
            {
                playerDamage[player.whoAmI] += player.HeldItem.damage;
            }
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.owner >= 0 && projectile.owner < Main.maxPlayers)
            {
                Player player = Main.player[projectile.owner];
                if (!playerDamage.ContainsKey(player.whoAmI))
                {
                    playerDamage[player.whoAmI] = 0;
                }
                playerDamage[player.whoAmI] += hit.Damage;
                if (hit.Damage > npc.life)
                {
                    playerDamage[player.whoAmI] += player.HeldItem.damage;
                }
            }
        }
        public override void OnKill(NPC npc)
        {
            int totalDamage = npc.lifeMax;

            if (npc.friendly || npc.CountsAsACritter || npc.SpawnedFromStatue || npc.isLikeATownNPC)
            {
                return;
            }

            foreach (var entry in playerDamage)
            {
                int playerID = entry.Key;
                int damageDealt = entry.Value;
                Player player = Main.player[playerID];
                
                if (player.active && !player.dead)
                {
                    float damageRatio = (float)damageDealt / totalDamage;
                    int experiencePoints = (int)(npc.lifeMax * damageRatio / 10); // Example calculation
                    ExperiencePlayer.AddExperience(player, experiencePoints);

                    CombatText.NewText(player.Hitbox, Color.LightGoldenrodYellow, $"+{experiencePoints} xp", true);
                    

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        ModPacket packet = Mod.GetPacket();
                        packet.Write((byte)MessageType.AddExperience);
                        packet.Write(playerID);
                        packet.Write(experiencePoints);
                        packet.Send();
                    }
                }              
            }
        }
    }
}