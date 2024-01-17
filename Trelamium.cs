using Terraria.ModLoader;

namespace Trelamium
{
	public class Trelamium : Mod
	{
        public const string Abbreviation = "TM";

        public const string AbbreviationPrefix = Abbreviation + ":";
        public static Trelamium Instance => ModContent.GetInstance<Trelamium>();
    }
}