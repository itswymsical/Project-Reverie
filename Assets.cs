namespace ReverieMod
{
	/// <summary>
	/// Contains directories for all asset paths.
	/// </summary>
	internal static class Assets
	{
		public const string Directory = "ReverieMod/Assets/";

        public const string TextureDirectory = "ReverieMod/Assets/Textures/";

        public const string SFX = "Assets/SFX/";

        public const string Music = "Assets/Music/";

        public const string PlaceholderTexture = TextureDirectory + "Unloaded";

		public const string Icon = "ReverieMod/icon";

		public const string Dusts = TextureDirectory + "Dusts/";
		internal class Items
		{
			public const string Directory = TextureDirectory + "Items/";

			public const string Misc = Directory + "Misc/";

            public const string Tiles = Directory + "Tiles/";

            public const string Accessory = Directory + "Accessory/";

			public const string Consumable = Directory + "Consumable/";

			public const string Food = Consumable + "Food/";

			public const string Materials = Directory + "Materials/";

			public const string Tools = Directory + "Tools/";

			public const string Shovels = Tools + "Shovels/";

			#region Boss
			public const string Boss = Directory + "Boss/";

			public const string Fungore = Boss + "Fungore/";
            public const string FG = Boss + "ForestGuardian/";
            public const string Legion = Boss + "FoodLegion/";
            public const string Shelledrake = Boss + "Shelledrake/";
            #endregion
        }
		internal class Armor
		{
			public const string Directory = TextureDirectory + "Items/Armor/";

			public const string Sandcrawler = Directory + "Sandcrawler/";

			public const string Wildlife = Directory + "Wildlife/";

			public const string Frostbark = Directory + "Frostbark/";
		}
		internal class Vanity
		{
			public const string Directory = TextureDirectory + "Items/Vanity/";

			public const string Peepo = Directory + "Peepo/";
		}
		internal class Weapons
		{
			public const string Directory = TextureDirectory + "Items/Weapons/";

			public const string Sandcrawler = Directory + "Sandcrawler/";

			public const string Frostbark = Directory + "Frostbark/";

			public const string Melee = Directory + "Melee/";

			public const string Magic = Directory + "Magic/";

			public const string Summon = Directory + "Summon/";
		}
		internal class Buffs
		{
			public const string Directory = TextureDirectory + "Buffs/";

			public const string Minions = Directory + "Minions/";

			public const string Potions = Directory + "Potions/";

			public const string Debuffs = Directory + "Debuffs/";

		}
		internal class NPCs
		{
			public const string Directory = TextureDirectory + "NPCs/";

            public const string Boss = Directory + "Boss/";

			public const string Fungore = Boss + "Fungore/";

            public const string WoodenWarden = Boss + "WoodenWarden/";

            public const string FoodLegion = Boss + "FoodLegion/";

			//public const string Cumulor = Boss + "Cumulor/";

			public const string Critters = Directory + "Critters/";

			public const string Enemies = Directory + "Enemies/";

			public const string Forest = Enemies + "Forest/";

			public const string Desert = Enemies + "Desert/";

			public const string Underground = Enemies + "Underground/";
		}
		internal class Projectiles
		{
			public const string Directory = TextureDirectory + "Projectiles/";

			public const string Typeless = Directory + "Typeless/";

			public const string Melee = Directory + "Melee/";

            public const string Frostbark = Directory + "Frostbark/";

            public const string Ranged = Directory + "Ranged/";

			public const string Magic = Directory + "Magic/";

			public const string Summon = Directory + "Summon/";
		}
		internal class Tiles
		{
			public const string Directory = TextureDirectory + "Tiles/";

			public const string Bars = Directory + "Bars/";

			public const string Ores = Directory + "Ores/";

			public const string WoodlandCanopy = Directory + "WoodlandCanopy/";

            public const string Furniture = Directory + "Furniture/";

			public const string Paintings = Furniture + "Paintings/";
		}
	}
}
