using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ReverieMod.Common.UI
{
    public class NPCData
    {
        public Asset<Texture2D> IconTexture { get; }
        public string NpcName { get; }
        public Color DialogueColor { get; }
        public SoundStyle CharacterSound { get; }
        public enum NPCDialogueID
        {
            Guide,
            Sophie,
            Fungore,
            Dalia,
            Mechanic,
            Goblin
        }
        public static class NPCDialogueIDHelper
        {
            public static NPCData GetNPCData(NPCDialogueID npcDialogueID)
            {
                var mechanic = NPC.FindFirstNPC(NPCID.Mechanic);
                var guide = NPC.FindFirstNPC(NPCID.Guide);
                var goblin = NPC.FindFirstNPC(NPCID.GoblinTinkerer);
                Player plr = Main.LocalPlayer;
                switch (npcDialogueID)
                {
                    case NPCDialogueID.Guide:
                        return new NPCData(
                            ModContent.Request<Texture2D>("ReverieMod/Assets/Textures/UI/DialoguePortraits/Guide"),
                            Main.npc[guide].GivenName,
                            new Color(64, 109, 164),
                            SoundID.MenuOpen
                        );
                    case NPCDialogueID.Sophie:
                        return new NPCData(
                            ModContent.Request<Texture2D>("ReverieMod/Assets/Textures/UI/DialoguePortraits/Sophie"),
                            "Sophie",
                            Color.Violet,
                            new SoundStyle($"{nameof(ReverieMod)}/Assets/SFX/Dialogue/Sophie")
                        );
                    case NPCDialogueID.Fungore:
                        return new NPCData(
                            ModContent.Request<Texture2D>("ReverieMod/Assets/Textures/UI/DialoguePortraits/Fungore"),
                            "Fungore",
                            Color.LavenderBlush,
                            new SoundStyle($"{nameof(ReverieMod)}/Assets/SFX/Dialogue/FungoreReborn")
                        );
                    case NPCDialogueID.Dalia:
                        return new NPCData(
                            ModContent.Request<Texture2D>("ReverieMod/Assets/Textures/UI/DialoguePortraits/Dalia"),
                            "Dalia",
                            Color.LightGoldenrodYellow,
                            new SoundStyle($"{nameof(ReverieMod)}/Assets/SFX/Dialogue/Dalia")
                        );
                    case NPCDialogueID.Mechanic:
                        return new NPCData(
                            ModContent.Request<Texture2D>("ReverieMod/Assets/Textures/UI/DialoguePortraits/Mechanic"),
                            Main.npc[mechanic].GivenName,
                            Color.Orange,
                            new SoundStyle($"{nameof(ReverieMod)}/Assets/SFX/Dialogue/Mechanic")
                        );
                    case NPCDialogueID.Goblin:
                        return new NPCData(
                            ModContent.Request<Texture2D>("ReverieMod/Assets/Textures/UI/DialoguePortraits/Goblin"),
                            Main.npc[goblin].GivenName,
                            Color.LightSlateGray,
                            new SoundStyle($"{nameof(ReverieMod)}/Assets/SFX/Dialogue/Goblin")
                        );
                    // Add more cases for other NPCs
                    default:
                        throw new ArgumentException($"NPCDialogueID '{npcDialogueID}' not recognized.");
                }
            }
        }
        public NPCData(Asset<Texture2D> iconTexture, string npcName, Color dialogueColor, SoundStyle characterSound)
        {
            IconTexture = iconTexture;
            NpcName = npcName;
            DialogueColor = dialogueColor;
            CharacterSound = characterSound;
        }
    }
}