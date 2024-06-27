using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;

namespace ReverieMod.Common.UI
{
    public class NPCData
    {
        public Asset<Texture2D> IconTexture { get; }
        public string NpcName { get; }
        public Color DialogueColor { get; }
        public SoundStyle CharacterSound { get; }

        public NPCData(Asset<Texture2D> iconTexture, string npcName, Color dialogueColor, SoundStyle characterSound)
        {
            IconTexture = iconTexture;
            NpcName = npcName;
            DialogueColor = dialogueColor;
            CharacterSound = characterSound;
        }
    }
}