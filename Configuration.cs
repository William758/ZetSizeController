using BepInEx.Configuration;

namespace TPDespair.ZetSizeController
{
	public static class Configuration
	{
		public static ConfigEntry<float> SizeChangeRate { get; set; }
		public static ConfigEntry<bool> IgnoreSizeLimit { get; set; }
		public static ConfigEntry<bool> IgnoreSizeExponent { get; set; }
		public static ConfigEntry<float> AbsoluteSizeLimit { get; set; }
		public static ConfigEntry<bool> ValidateMonsterSize { get; set; }

		public static ConfigEntry<bool> AllowStatHook { get; set; }
		public static ConfigEntry<bool> ModifyCamera { get; set; }
		public static ConfigEntry<bool> ModifyInteraction { get; set; }
		public static ConfigEntry<bool> ModifyOverlap { get; set; }
		public static ConfigEntry<bool> ModifyAnimation { get; set; }

		public static ConfigEntry<int> ShrinkifactEnable { get; set; }
		public static ConfigEntry<float> ShrinkifactMult { get; set; }
		public static ConfigEntry<bool> ShrinkifactExtend { get; set; }
		public static ConfigEntry<bool> ShrinkifactPlayer { get; set; }
		public static ConfigEntry<bool> ShrinkifactMonster { get; set; }
		public static ConfigEntry<float> ShrinkifactHealth { get; set; }
		public static ConfigEntry<float> ShrinkifactDamage { get; set; }
		public static ConfigEntry<float> ShrinkifactMovement { get; set; }

		public static ConfigEntry<int> TitanifactEnable { get; set; }
		public static ConfigEntry<float> TitanifactMult { get; set; }
		public static ConfigEntry<bool> TitanifactExtend { get; set; }
		public static ConfigEntry<bool> TitanifactPlayer { get; set; }
		public static ConfigEntry<bool> TitanifactMonster { get; set; }
		public static ConfigEntry<float> TitanifactHealth { get; set; }
		public static ConfigEntry<float> TitanifactDamage { get; set; }
		public static ConfigEntry<float> TitanifactMovement { get; set; }

		public static ConfigEntry<int> SplitifactEnable { get; set; }
		public static ConfigEntry<float> SplitifactChance { get; set; }
		public static ConfigEntry<float> SplitifactMult { get; set; }
		public static ConfigEntry<int> SplitifactMaxSmallFly { get; set; }
		public static ConfigEntry<int> SplitifactMaxLesser { get; set; }
		public static ConfigEntry<int> SplitifactMaxGreater { get; set; }
		public static ConfigEntry<int> SplitifactMaxChampion { get; set; }

		public static ConfigEntry<float> PlayerSizeIncrease { get; set; }
		public static ConfigEntry<float> PlayerSizeMult { get; set; }
		public static ConfigEntry<float> PlayerSizeExponent { get; set; }
		public static ConfigEntry<float> PlayerSizeLimit { get; set; }
		public static ConfigEntry<bool> PlayerSizeLimitExtendable { get; set; }

		public static ConfigEntry<float> LesserSizeIncrease { get; set; }
		public static ConfigEntry<float> LesserSizeMult { get; set; }
		public static ConfigEntry<float> LesserSizeExponent { get; set; }
		public static ConfigEntry<float> LesserSizeLimit { get; set; }

		public static ConfigEntry<float> GreaterSizeIncrease { get; set; }
		public static ConfigEntry<float> GreaterSizeMult { get; set; }
		public static ConfigEntry<float> GreaterSizeExponent { get; set; }
		public static ConfigEntry<float> GreaterSizeLimit { get; set; }

		public static ConfigEntry<float> ChampionSizeIncrease { get; set; }
		public static ConfigEntry<float> ChampionSizeMult { get; set; }
		public static ConfigEntry<float> ChampionSizeExponent { get; set; }
		public static ConfigEntry<float> ChampionSizeLimit { get; set; }

		public static ConfigEntry<float> ModifierMult { get; set; }
		public static ConfigEntry<float> CoreItemModifierLimit { get; set; }
		public static ConfigEntry<float> KnurlSizeIncrease { get; set; }
		public static ConfigEntry<float> PearlSizeIncrease { get; set; }
		public static ConfigEntry<float> StoneFluxSizeIncrease { get; set; }
		public static ConfigEntry<float> BossSizeIncrease { get; set; }
		public static ConfigEntry<float> EliteSizeIncrease { get; set; }
		public static ConfigEntry<float> EliteCountSizeIncrease { get; set; }
		public static ConfigEntry<float> GlassModifierLimit { get; set; }
		public static ConfigEntry<float> GlassSizeMult { get; set; }
		public static ConfigEntry<float> TonicSizeMult { get; set; }

		public static ConfigEntry<float> PlayerLevelSizeIncrease { get; set; }
		public static ConfigEntry<float> PlayerLevelSizeExponent { get; set; }
		public static ConfigEntry<float> PlayerLevelSizeLimit { get; set; }
		public static ConfigEntry<float> MonsterLevelSizeIncrease { get; set; }
		public static ConfigEntry<float> MonsterLevelSizeExponent { get; set; }
		public static ConfigEntry<float> MonsterLevelSizeLimit { get; set; }

		public static ConfigEntry<float> ItemScoreSizeIncrease { get; set; }
		public static ConfigEntry<float> ItemScoreSizeExponent { get; set; }
		public static ConfigEntry<float> ItemScoreSizeLimit { get; set; }
		public static ConfigEntry<float> ItemScoreT1Effect { get; set; }
		public static ConfigEntry<float> ItemScoreT2Effect { get; set; }
		public static ConfigEntry<float> ItemScoreT3Effect { get; set; }
		public static ConfigEntry<float> ItemScoreBossEffect { get; set; }
		public static ConfigEntry<float> ItemScoreLunarEffect { get; set; }



		internal static void Init(ConfigFile Config)
		{
			SizeChangeRate = Config.Bind(
				"1a-General", "sizeChangeRate", 0.25f,
				"Rate of size change per second. Higher values = faster. 0 is instant."
			);
			IgnoreSizeLimit = Config.Bind(
				"1a-General", "ignoreSizeLimit", false,
				"Set all sizeclass limits to extreme values."
			);
			IgnoreSizeExponent = Config.Bind(
				"1a-General", "ignoreSizeExponent", false,
				"Set all sizeclass exponents to 1."
			);
			AbsoluteSizeLimit = Config.Bind(
				"1a-General", "absoluteSizeLimit", 4f,
				"Prevent anything from scaling past value. Overwritten by ignoreSizeLimit."
			);
			ValidateMonsterSize = Config.Bind(
				"1a-General", "validateMonsterSize", true,
				"Wait a bit until size changes are made for monsters after spawning."
			);



			AllowStatHook = Config.Bind(
				"1c-Effects", "allowStatHook", true,
				"Allows changing stat values. Used for artifact effects."
			);
			ModifyCamera = Config.Bind(
				"1c-Effects", "modifyCamera", true,
				"Modify camera to scale with size."
			);
			ModifyInteraction = Config.Bind(
				"1c-Effects", "modifyInteraction", true,
				"Modify interaction range to scale with size."
			);
			ModifyOverlap = Config.Bind(
				"1c-Effects", "modifyOverlap", true,
				"Modify overlap attacks for some monsters to scale better with size."
			);
			ModifyAnimation = Config.Bind(
				"1c-Effects", "modifyAnimation", true,
				"Modify movement animation speed to scale with size."
			);



			ShrinkifactEnable = Config.Bind(
				"2a-Artifacts - Shrink", "shrinkifactEnable", 1,
				"Artifact of Miniaturization. 0 = Disabled, 1 = Artifact Available, 2 = Always Active"
			);
			ShrinkifactMult = Config.Bind(
				"2a-Artifacts - Shrink", "shrinkifactMult", 0.65f,
				"Size multiplier of Artifact of Miniaturization. Unaffected by sizeclass exponent."
			);
			ShrinkifactExtend = Config.Bind(
				"2a-Artifacts - Shrink", "shrinkifactExtend", true,
				"Artifact of Miniaturization modifies minimum size limits."
			);
			ShrinkifactPlayer = Config.Bind(
				"2a-Artifacts - Shrink", "shrinkifactPlayer", true,
				"Artifact of Miniaturization applies to players."
			);
			ShrinkifactMonster = Config.Bind(
				"2a-Artifacts - Shrink", "shrinkifactMonster", true,
				"Artifact of Miniaturization applies to monsters."
			);
			ShrinkifactHealth = Config.Bind(
				"2a-Artifacts - Shrink", "shrinkifactHealth", 1f,
				"Artifact of Miniaturization health multiplier."
			);
			ShrinkifactDamage = Config.Bind(
				"2a-Artifacts - Shrink", "shrinkifactDamage", 1f,
				"Artifact of Miniaturization damage multiplier."
			);
			ShrinkifactMovement = Config.Bind(
				"2a-Artifacts - Shrink", "shrinkifactMovement", 1f,
				"Artifact of Miniaturization movement speed multiplier."
			);



			TitanifactEnable = Config.Bind(
				"2b-Artifacts - Grow", "titanifactEnable", 1,
				"Artifact of the Giants. 0 = Disabled, 1 = Artifact Available, 2 = Always Active"
			);
			TitanifactMult = Config.Bind(
				"2b-Artifacts - Grow", "titanifactMult", 1.5f,
				"Size multiplier of Artifact of the Giants. Unaffected by sizeclass exponent."
			);
			TitanifactExtend = Config.Bind(
				"2b-Artifacts - Grow", "titanifactExtend", true,
				"Artifact of the Giants modifies maximum size limits."
			);
			TitanifactPlayer = Config.Bind(
				"2b-Artifacts - Grow", "titanifactPlayer", false,
				"Artifact of the Giants applies to players."
			);
			TitanifactMonster = Config.Bind(
				"2b-Artifacts - Grow", "titanifactMonster", true,
				"Artifact of the Giants applies to monsters."
			);
			TitanifactHealth = Config.Bind(
				"2b-Artifacts - Grow", "titanifactHealth", 1.5f,
				"Artifact of the Giants health multiplier."
			);
			TitanifactDamage = Config.Bind(
				"2b-Artifacts - Grow", "titanifactDamage", 1.5f,
				"Artifact of the Giants damage multiplier."
			);
			TitanifactMovement = Config.Bind(
				"2b-Artifacts - Grow", "titanifactMovement", 1f,
				"Artifact of the Giants movement speed multiplier."
			);



			SplitifactEnable = Config.Bind(
				"2c-Artifacts - Split", "splitifactEnable", 1,
				"Artifact of Fragmentation. 0 = Disabled, 1 = Artifact Available, 2 = Always Active"
			);
			SplitifactChance = Config.Bind(
				"2c-Artifacts - Split", "splitifactChance", 0.5f,
				"Chance that any monster will be selected for splitting. 0.5 = 50% chance that a monster will be made larger and split on death."
			);
			SplitifactMult = Config.Bind(
				"2c-Artifacts - Split", "splitifactMult", 1.5f,
				"Size multiplier per remaining split for Artifact of Fragmentation. Unaffected by sizeclass exponent."
			);
			SplitifactMaxSmallFly = Config.Bind(
				"2c-Artifacts - Split", "splitifactMaxSmallFly", 1,
				"Maximum amount of splits for small flying monsters. Lowest between this value and sizeclass value is chosen."
			);
			SplitifactMaxLesser = Config.Bind(
				"2c-Artifacts - Split", "splitifactMaxLesser", 2,
				"Maximum amount of splits for lesser monsters."
			);
			SplitifactMaxGreater = Config.Bind(
				"2c-Artifacts - Split", "splitifactMaxGreater", 1,
				"Maximum amount of splits for greater monsters."
			);
			SplitifactMaxChampion = Config.Bind(
				"2c-Artifacts - Split", "splitifactMaxChampion", 0,
				"Maximum amount of splits for champion monsters."
			);



			PlayerSizeIncrease = Config.Bind(
				"3a-Size - Player", "playerSizeIncrease", 0f,
				"Size increase of players."
			);
			PlayerSizeMult = Config.Bind(
				"3a-Size - Player", "playerSizeMult", 1f,
				"Size multiplier on players."
			);
			PlayerSizeExponent = Config.Bind(
				"3a-Size - Player", "playerSizeExponent", 0.65f,
				"Apply exponent to final player size."
			);
			PlayerSizeLimit = Config.Bind(
				"3a-Size - Player", "playerSizeLimit", 2.0f,
				"Maximum size of players. Higher values can make skill usage on some characters awkward."
			);
			PlayerSizeLimitExtendable = Config.Bind(
				"3a-Size - Player", "playerSizeLimitExtendable", false,
				"Whether player size limit can be extended by other effects."
			);



			LesserSizeIncrease = Config.Bind(
				"3b-Size - Lesser", "lesserSizeIncrease", 0f,
				"Size increase of lesser monsters."
			);
			LesserSizeMult = Config.Bind(
				"3b-Size - Lesser", "lesserSizeMult", 1f,
				"Size multiplier on lesser monsters."
			);
			LesserSizeExponent = Config.Bind(
				"3b-Size - Lesser", "lesserSizeExponent", 0.65f,
				"Apply exponent to final lesser monster size."
			);
			LesserSizeLimit = Config.Bind(
				"3b-Size - Lesser", "lesserSizeLimit", 3.0f,
				"Maximum size of lesser monsters."
			);



			GreaterSizeIncrease = Config.Bind(
				"3c-Size - Greater", "greaterSizeIncrease", 0f,
				"Size increase of greater monsters."
			);
			GreaterSizeMult = Config.Bind(
				"3c-Size - Greater", "greaterSizeMult", 1f,
				"Size multiplier on greater monsters."
			);
			GreaterSizeExponent = Config.Bind(
				"3c-Size - Greater", "greaterSizeExponent", 0.5f,
				"Apply exponent to final greater monster size."
			);
			GreaterSizeLimit = Config.Bind(
				"3c-Size - Greater", "greaterSizeLimit", 2.0f,
				"Maximum size of greater monsters."
			);



			ChampionSizeIncrease = Config.Bind(
				"3d-Size - Champion", "championSizeIncrease", 0f,
				"Size increase of champion monsters."
			);
			ChampionSizeMult = Config.Bind(
				"3d-Size - Champion", "championSizeMult", 1f,
				"Size multiplier on champion monsters."
			);
			ChampionSizeExponent = Config.Bind(
				"3d-Size - Champion", "championSizeExponent", 0.5f,
				"Apply exponent to final champion monster size."
			);
			ChampionSizeLimit = Config.Bind(
				"3d-Size - Champion", "championSizeLimit", 2.0f,
				"Maximum size of champion monsters."
			);



			ModifierMult = Config.Bind(
				"4a-Modifiers - General", "modifierMult", 1f,
				"Multiply increase modifier effects in modifiers sections. Multiplies item effects after limits."
			);



			CoreItemModifierLimit = Config.Bind(
				"4b-Modifiers - Core", "coreItemEffectLimit", 5f,
				"Maximum modifier effect multiplier for : Knurl, Pearls, StoneFlux. 5 = SQRT(25), item effect caps out at 25 items for each core item."
			);
			KnurlSizeIncrease = Config.Bind(
				"4b-Modifiers - Core", "knurlSizeIncrease", 0.1f,
				"Size increase from Titanic Knurl. Value multiplied by SQRT of item count."
			);
			PearlSizeIncrease = Config.Bind(
				"4b-Modifiers - Core", "pearlSizeIncrease", 0.1f,
				"Size increase from Pearl(x1) and Irradiant Pearl(x2.5). Value multiplied by SQRT of item count."
			);
			StoneFluxSizeIncrease = Config.Bind(
				"4b-Modifiers - Core", "stoneFluxSizeIncrease", 0.2f,
				"Size increase from Stone Flux Pauldron. Value multiplied by SQRT of item count."
			);
			BossSizeIncrease = Config.Bind(
				"4b-Modifiers - Core", "bossSizeIncrease", 0.2f,
				"Size increase from being in a boss group."
			);
			EliteSizeIncrease = Config.Bind(
				"4b-Modifiers - Core", "eliteSizeIncrease", 0.1f,
				"Size increase from having any elite buff."
			);
			EliteCountSizeIncrease = Config.Bind(
				"4b-Modifiers - Core", "eliteCountSizeIncrease", 0.1f,
				"Size increase per elite buff. Value multiplied by SQRT of buff count."
			);
			GlassModifierLimit = Config.Bind(
				"4b-Modifiers - Core", "glassEffectLimit", 0.25f,
				"Minimum modifier effect for : ShapedGlass."
			);
			GlassSizeMult = Config.Bind(
				"4b-Modifiers - Core", "glassSizeMult", 0.9f,
				"Size multiplier per Shaped Glass. Unaffected by modifierMult. Ignores sizeclass exponent."
			);
			TonicSizeMult = Config.Bind(
				"4b-Modifiers - Core", "tonicSizeMult", 1.25f,
				"Size multiplier from Spinel Tonic. Unaffected by modifierMult. Ignores sizeclass exponent."
			);



			PlayerLevelSizeIncrease = Config.Bind(
				"4c-Modifiers - LevelScaling", "playerLevelSizeIncrease", 0f,
				"Size increase from levels for players. 0 to disable. 0.01 would be the default if enabled."
			);
			PlayerLevelSizeExponent = Config.Bind(
				"4c-Modifiers - LevelScaling", "playerLevelSizeExponent", 1f,
				"Exponent applied to player level for size increase."
			);
			PlayerLevelSizeLimit = Config.Bind(
				"4c-Modifiers - LevelScaling", "playerLevelSizeLimit", 0.5f,
				"Limit for size increase from player level."
			);
			MonsterLevelSizeIncrease = Config.Bind(
				"4c-Modifiers - LevelScaling", "monsterLevelSizeIncrease", 0f,
				"Size increase from levels for monsters. 0 to disable. 0.02 would be the default if enabled."
			);
			MonsterLevelSizeExponent = Config.Bind(
				"4c-Modifiers - LevelScaling", "monsterLevelSizeExponent", 0.65f,
				"Exponent applied to monster level for size increase."
			);
			MonsterLevelSizeLimit = Config.Bind(
				"4c-Modifiers - LevelScaling", "monsterLevelSizeLimit", 0.5f,
				"Limit for size increase from monster level."
			);



			ItemScoreSizeIncrease = Config.Bind(
				"4d-Modifiers - ItemScoreScaling", "itemScoreSizeIncrease", 0f,
				"Size increase from itemscore. 0 to disable. 0.01 would be the default if enabled."
			);
			ItemScoreSizeExponent = Config.Bind(
				"4d-Modifiers - ItemScoreScaling", "itemScoreSizeExponent", 0.65f,
				"Exponent applied to itemscore for size increase."
			);
			ItemScoreSizeLimit = Config.Bind(
				"4d-Modifiers - ItemScoreScaling", "itemScoreSizeLimit", 0.5f,
				"Limit for size increase from itemscore."
			);
			ItemScoreT1Effect = Config.Bind(
				"4d-Modifiers - ItemScoreScaling", "itemScoreT1Effect", 1f,
				"Amount of added itemscore per T1 item."
			);
			ItemScoreT2Effect = Config.Bind(
				"4d-Modifiers - ItemScoreScaling", "itemScoreT2Effect", 2f,
				"Amount of added itemscore per T2 item."
			);
			ItemScoreT3Effect = Config.Bind(
				"4d-Modifiers - ItemScoreScaling", "itemScoreT3Effect", 6f,
				"Amount of added itemscore per T3 item."
			);
			ItemScoreBossEffect = Config.Bind(
				"4d-Modifiers - ItemScoreScaling", "itemScoreBossEffect", 4f,
				"Amount of added itemscore per Boss item."
			);
			ItemScoreLunarEffect = Config.Bind(
				"4d-Modifiers - ItemScoreScaling", "itemScoreLunarEffect", 4f,
				"Amount of added itemscore per Lunar item."
			);
		}
	}
}
