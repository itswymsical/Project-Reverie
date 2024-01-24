using Terraria.ModLoader;

namespace EmpyreanDreamscape
{
	public class EmpyreanDreamscape : Mod
	{
        public const string Abbreviation = "TM";

        public const string AbbreviationPrefix = Abbreviation + ":";
        public static EmpyreanDreamscape Instance => ModContent.GetInstance<EmpyreanDreamscape>();
    }
}