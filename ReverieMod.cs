using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace ReverieMod
{
    public class ReverieMod : Mod
    {
        public const string Abbreviation = "ReverieMod";

        public const string AbbreviationPrefix = Abbreviation + ":";
        public static ReverieMod Instance => ModContent.GetInstance<ReverieMod>();
    }
}