using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReverieMod.Common.Players;
using System;
using System.Security.AccessControl;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static ReverieMod.Common.UI.NPCData;

namespace ReverieMod.Common.UI
{
    public class MissionStatusIndicator : IInGameNotification
	{
		public bool ShouldBeRemoved => timeLeft <= 0;
		private int timeLeft = 5 * 60;
		private float Scale
		{
			get
			{
				if (timeLeft < 30)
				{
					return MathHelper.Lerp(0f, 1f, timeLeft / 30f);
				}

				if (timeLeft > 285)
				{
					return MathHelper.Lerp(1f, 0f, (timeLeft - 285) / 15f);
				}

				return 1f;
			}
		}
		private float Opacity
		{
			get
			{
				if (Scale <= 0.5f)
				{
					return 0f;
				}

				return (Scale - 0.5f) / 0.5f;
			}
		}
		public void Update()
		{
			if (timeLeft == 5 * 60)
			{
				if (!MissionChecks.failed)
				{
                    SoundEngine.PlaySound(SoundID.AchievementComplete, Main.LocalPlayer.position);
                }
				else
				{
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(ReverieMod)}/Assets/SFX/MitigateDrop"), Main.LocalPlayer.position);
                }
                
            }
			timeLeft--;

			if (timeLeft < 0)
			{
				timeLeft = 0;
			}
			
		}
        private float panelWidth = 420f;
		public static int rewardItem = 0;
		public static int rewardStack = 0;
        public void DrawInGame(SpriteBatch spriteBatch, Vector2 bottomAnchorPosition)
		{
			if (Opacity <= 0f)
				return;
			
			Player player = Main.LocalPlayer;
			var reward = TextureAssets.Item[rewardItem];

			ExperiencePlayer modPlayer = player.GetModPlayer<ExperiencePlayer>();
			string title = $"Mission Complete! Rewards:";
			if (rewardStack > 1)
				title = $"Mission Complete! Rewards: {rewardStack}";
            
			if (MissionChecks.failed)
				title = "Mission Failed!";
			
            float effectiveScale = Scale * 1.1f;
            float panelHeight = ( new Vector2(35f, 20f).Y * 2f) * effectiveScale;
            Vector2 panelSize = new Vector2(panelWidth + 18, panelHeight);
            Vector2 panelPosition = bottomAnchorPosition + new Vector2(0f, (15f - panelSize.Y) * 0.5f);
            Rectangle panelRectangle = Utils.CenteredRectangle(panelPosition, panelSize);

			Color color = Color.LightCoral;
            Color colorText = new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor / 5, Main.mouseTextColor);

            if (MissionChecks.failed)
			{
				color = Color.Black;
				colorText = Color.Red;
            }

            Utils.DrawInvBG(spriteBatch, panelRectangle, default);
            spriteBatch.Draw(reward.Value, panelPosition + new Vector2(-30f + (panelWidth / 3), -52f + panelSize.Y), Color.White * Opacity);
            Utils.DrawBorderString(spriteBatch, title, panelPosition + new Vector2(panelWidth / 4, -44f + panelSize.Y), colorText * Opacity, effectiveScale * 0.9f, 1f, 0.4f);
		}


        public void PushAnchor(ref Vector2 positionAnchorBottom) => positionAnchorBottom.Y -= 50f * Opacity;
	}

	public class MissionPlayer : ModPlayer
	{
        public override void OnEnterWorld()
        {
            if (!MissionChecks.GUIDE_MISSIONS_WORLDSTART_ACTIVE && !MissionChecks.GUIDE_MISSIONS_WORLDSTART_COMPLETE)
            {
                Player player = Main.LocalPlayer;
                var guideData = NPCDialogueIDHelper.GetNPCData(NPCDialogueID.Guide);
                var guide = NPC.FindFirstNPC(NPCID.Guide);
                var dialogues = new (string Text, int Delay, int TimeLeft, NPCData NpcData)[]
                {
                    ($"Hey, {player.name}!", 3, 300, guideData),
                    ($"Nice to meet you, I'm {Main.npc[guide].GivenName}.", 2, 300, guideData),
                    ($"I'll be your guide.", 2, 300, guideData),
                    ("Although I'm only an apprentice guide, I'm more than qualified to help you learn everything about Terraria.", 2, 300, guideData),
                    ("You've been out for a while now, hehe.", 4, 300, guideData),
                    ("Anyways...", 2, 300, guideData),
                    ("Let's get started with our first task!", 3, 300, guideData),
                    ("Come speak with me.", 2, 300, guideData)
                };

                NPCDialogueBox dialogue = NPCDialogueBox.CreateNewDialogueSequence(dialogues);
                InGameNotificationsTracker.AddNotification(dialogue);

                MissionChecks.GUIDE_MISSIONS_WORLDSTART_ACTIVE = true;
            }
        }
        //public override void PostUpdate() { }
    }
    public class MissionInfoDisplay : InfoDisplay
    {
        public static Color RedInfoTextColor => new(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor);
        public override string Texture => Assets.TextureDirectory + Name;
        public override string HoverTexture => Texture + "_Hover";
        public override bool Active() => true; 
        public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
        {
			// Display Mission Name and Objective
			bool noInfo = MissionChecks.missionName == string.Empty && MissionChecks.missionObjective == string.Empty;

            if (noInfo)
				displayColor = InactiveInfoTextColor;
            else
				displayColor = RedInfoTextColor;

			//TODO: Checker for objective condition met (i.e. player placed item)
			// Do this by flagging the event and adding incrementing the objective value, or smth like that.
            return !noInfo ? $"{MissionChecks.missionName}: {MissionChecks.missionObjective}" : "No active missions.";
        }
    }
}