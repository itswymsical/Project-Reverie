using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria.Audio;
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
                switch (npcDialogueID)
                {
                    case NPCDialogueID.Guide:
                        return new NPCData(
                            ModContent.Request<Texture2D>("ReverieMod/Assets/Textures/UI/DialoguePortraits/Guide"),
                            "Guide",
                            Color.LightBlue,
                            SoundID.MenuOpen
                        );
                    case NPCDialogueID.Sophie:
                        return new NPCData(
                            ModContent.Request<Texture2D>("ReverieMod/Assets/Textures/UI/DialoguePortraits/Sophie"),
                            "Sophie",
                            Color.Violet,
                            SoundID.MenuClose
                        );
                    case NPCDialogueID.Fungore:
                        return new NPCData(
                            ModContent.Request<Texture2D>("ReverieMod/Assets/Textures/UI/DialoguePortraits/Fungore"),
                            "Fungpre",
                            Color.LavenderBlush,
                            new SoundStyle($"{nameof(ReverieMod)}/Assets/SFX/Dialogue/FungoreReborn")
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