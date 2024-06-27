using ReverieMod.Common;
using ReverieMod.Core.Loaders;

using Microsoft.Xna.Framework;
using System.Linq;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReverieMod.Common.UI;
using ReverieMod.Common.Cutscenes;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace ReverieMod.Common.Players
{
	public class CutscenePlayer : ModPlayer
    {
        public Vector2[] CreditsScreenPosition = new Vector2[4];
        public bool RegisteredCreditsPositions;

        public bool PlayCredits;

        public override void ModifyScreenPosition()
        {
            foreach (var cutscene in CutsceneLoader.Cutscenes.Where(c => c.Visible))
                cutscene.ModifyScreenPosition();
        }

        public override void PostUpdateMiscEffects()
        {
            if (!RegisteredCreditsPositions)
            {
                RegisterCreditsPositions();
                RegisteredCreditsPositions = true;
            }
        }

        public override void UpdateLifeRegen()
        {
      
        }

        public override void OnEnterWorld()
        {
            if (Main.netMode == NetmodeID.Server && !MissionChecks.GUIDE_MISSIONS_WORLDSTART && !MissionChecks.GUIDE_MISSIONS_WORLDSTART_COMPLETE)
            {

            }
            Player player = Main.LocalPlayer;

            var dialogues = new (string Text, int Delay)[]
            {
                ($"Hey, {player.name}!", 3),
                ($"Nice to meet you, I'm you're guide.", 2),
                ("Although i'm only an apprentice guide, I'm more than qualified to help you learn everything about Terraria.", 2),
                ("You've been out for a while now, hehe.", 4),
                ("Anyways...", 2),
                ("I bet you're wondering what to do from here.", 3),
                ("Come talk to me, I'll answer any questions you have.", 2)
            };

            NPCDialogueBox dialogue = NPCDialogueBox.CreateNewDialogueSequence(dialogues);
            dialogue.npcName = "Guide";
            dialogue.currentDialogue = "???"; // currentDialogue is ALWAYS the intial dialogue.
            dialogue.iconTexture = ModContent.Request<Texture2D>("ReverieMod/Assets/Textures/UI/DialoguePortraits/Guide");
            dialogue.color = Color.LightBlue;
            dialogue.characterSound = SoundID.MenuOpen;
            InGameNotificationsTracker.AddNotification(dialogue);
            //MissionChecks.GUIDE_MISSIONS_WORLDSTART = true;
        }

        private void RegisterCreditsPositions()
        {
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile tile = Framing.GetTileSafely(i, j);

                    if (tile.TileType == TileID.Emerald || tile.TileType == TileID.Sapphire)
                        CreditsScreenPosition[0] = new Vector2(i, j) * 16f;

                    if (tile.TileType == TileID.Cactus)
                        CreditsScreenPosition[1] = new Vector2(i, j) * 16f;

                    CreditsScreenPosition[2] = new Vector2(Main.dungeonX, Main.dungeonY) * 16f;

                    if (tile.TileType == TileID.JunglePlants)
                        CreditsScreenPosition[3] = new Vector2(i, j) * 16f;
                }
            }
        }
    }
}
