using ReverieMod.Common.UI;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static ReverieMod.Common.UI.NPCData;
using Terraria.UI;

namespace ReverieMod.Content.NPCs.WorldNPCs
{
    public class Sophie : ModNPC
    {
        private static Profiles.StackedNPCProfile NPCProfile;
        public override string Texture => Assets.NPCs.WorldNPCs + Name;
        public override string HeadTexture => Assets.NPCs.WorldNPCs + (Name + "_Head");
        public override void SetStaticDefaults()
        {
            //Main.npcFrameCount[Type] = 1;
            NPCID.Sets.HasNoPartyText[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            //NPCID.Sets.FaceEmote[Type] = ModContent.EmoteBubbleType<ExampleTravellingMerchantEmote>();

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 2f, // Draws the NPC in the bestiary as if its walking +2 tiles in the x direction
                Direction = -1 // -1 is left and 1 is right.
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 38;
            NPC.height = 50;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.immortal = true;
            NPC.knockBackResist = 0.5f;
            AnimationType = -1;
            TownNPCStayingHomeless = true;
        }
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("Chat");
            button2 = Language.GetTextValue("Mission(s) [!]");
        }
        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            Player player = Main.LocalPlayer;
            if (!firstButton)
            {
                var sophieData = NPCDialogueIDHelper.GetNPCData(NPCDialogueID.Dalia);
                var dialogues = new (string Text, int Delay, int TimeLeft, NPCData NpcData)[]
                {
                ($"I don't have any missions for ya at the moment, but check back in later.", 3, 300, sophieData)
                };

                NPCDialogueBox dialogue = NPCDialogueBox.CreateNewDialogueSequence(dialogues);
                InGameNotificationsTracker.AddNotification(dialogue);
            }
            if (firstButton)
            {
                var sophieData = NPCDialogueIDHelper.GetNPCData(NPCDialogueID.Sophie);
                var dialogues = new (string Text, int Delay, int TimeLeft, NPCData NpcData)[]
                {
                ($"Hey, {player.name}. How's it going?", 2, 300, sophieData),
                ($"I appreciate you taking care of those slimes for me. They've been quite the hassle.", 2, 300, sophieData),
                };

                NPCDialogueBox dialogue = NPCDialogueBox.CreateNewDialogueSequence(dialogues);
                InGameNotificationsTracker.AddNotification(dialogue);
            }
        }
        public override bool CanTownNPCSpawn(int numTownNPCs) => false;
        // This should always be false, because we spawn in the Traveling Merchant manually
        public override void AI() => NPC.homeless = true;
        
    }
}