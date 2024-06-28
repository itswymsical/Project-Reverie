using ReverieMod.Common;
using ReverieMod.Core.Loaders;

using Microsoft.Xna.Framework;
using System.Linq;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReverieMod.Common.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using ReLogic.Content;
using static ReverieMod.Common.UI.NPCData;

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
                // Some code here
            }

            Player player = Main.LocalPlayer;
            var guideData = NPCDialogueIDHelper.GetNPCData(NPCDialogueID.Guide);
            var sophieData = NPCDialogueIDHelper.GetNPCData(NPCDialogueID.Sophie);

            var dialogues = new (string Text, int Delay, int TimeLeft, NPCData NpcData)[]
            {
                ($"Hey, {player.name}!", 3, 300, guideData),
                ($"Nice to meet you, I'm your guide.", 2, 300, guideData),
                ("Although I'm only an apprentice guide, I'm more than qualified to help you learn everything about Terraria.", 2, 300, guideData),
                ("You've been out for a while now, hehe.", 4, 300, guideData),
                ("Anyways...", 2, 300, guideData),
                ("I bet you're wondering what to do from here.", 3, 300, guideData),
                ("Screw that guy. Come talk to me, I'll answer any questions you have!", 2, 300, sophieData)
            };

            NPCDialogueBox dialogue = NPCDialogueBox.CreateNewDialogueSequence(dialogues);
            InGameNotificationsTracker.AddNotification(dialogue);

            // MissionChecks.GUIDE_MISSIONS_WORLDSTART = true;
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
