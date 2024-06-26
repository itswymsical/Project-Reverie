using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReverieMod.Common.Players;
using ReverieMod.Common;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace ReverieMod
{
    public class ReverieMod : Mod
    {
        public const string Abbreviation = "ReverieMod";

        public const string AbbreviationPrefix = Abbreviation + ":";
        public static ReverieMod Instance => ModContent.GetInstance<ReverieMod>();
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MessageType msgType = (MessageType)reader.ReadByte();

            switch (msgType)
            {
                case MessageType.AddExperience:
                    int playerID = reader.ReadInt32();
                    int experience = reader.ReadInt32();
                    if (playerID >= 0 && playerID < Main.maxPlayers)
                    {
                        Player player = Main.player[playerID];
                        ExperiencePlayer.AddExperience(player, experience);
                        CombatText.NewText(player.Hitbox, Color.LightGoldenrodYellow, $"+{experience} xp", true);
                    }
                    break;
            }
        }
    }
}