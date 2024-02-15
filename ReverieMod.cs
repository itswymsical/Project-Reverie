using Terraria.ModLoader;

namespace ReverieMod
{
	public class ReverieMod : Mod
	{
        public const string Abbreviation = "ReverieM";

        public const string AbbreviationPrefix = Abbreviation + ":";
        public static ReverieMod Instance => ModContent.GetInstance<ReverieMod>();
    }
}