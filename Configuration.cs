using BepInEx.Configuration;

namespace TPDespair.ZetSizeController
{
    public static class Configuration
	{
		public static ConfigEntry<float> SizeChangeRate { get; set; }
		public static ConfigEntry<float> AbsoluteSizeLimit { get; set; }

		public static ConfigEntry<bool> ModifyCamera { get; set; }
		public static ConfigEntry<bool> ModifyInteraction { get; set; }
		public static ConfigEntry<bool> ModifyOverlap { get; set; }
		public static ConfigEntry<bool> ModifyAnimation { get; set; }

		public static ConfigEntry<int> ShrinkifactEnable { get; set; }
		public static ConfigEntry<float> ShrinkifactMult { get; set; }
		public static ConfigEntry<int> SplitifactEnable { get; set; }
		public static ConfigEntry<float> SplitifactMult { get; set; }
		public static ConfigEntry<int> SplitifactMaxSmallFly { get; set; }
		public static ConfigEntry<int> SplitifactMaxLesser { get; set; }
		public static ConfigEntry<int> SplitifactMaxGreater { get; set; }
		public static ConfigEntry<int> SplitifactMaxChampion { get; set; }

		public static ConfigEntry<float> PlayerSizeIncrease { get; set; }
		public static ConfigEntry<float> PlayerSizeMult { get; set; }
		public static ConfigEntry<float> PlayerSizeExponent { get; set; }
		public static ConfigEntry<float> PlayerSizeLimit { get; set; }

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
		public static ConfigEntry<float> KnurlSizeIncrease { get; set; }
		public static ConfigEntry<float> PearlSizeIncrease { get; set; }
		public static ConfigEntry<float> StoneFluxSizeIncrease { get; set; }
		public static ConfigEntry<float> BossSizeIncrease { get; set; }
		public static ConfigEntry<float> EliteSizeIncrease { get; set; }
		public static ConfigEntry<float> EliteCountSizeIncrease { get; set; }
		public static ConfigEntry<float> TonicSizeMult { get; set; }



		internal static void Init(ConfigFile Config)
		{
			SizeChangeRate = Config.Bind(
				"1a-General", "sizeChangeRate", 0.25f,
				"Rate of size change per second. 0 is instant."
			);
			AbsoluteSizeLimit = Config.Bind(
				"1a-General", "absoluteSizeLimit", 4f,
				"Prevent anything from scaling past value."
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
				"2a-Artifacts", "shrinkifactEnable", 1,
				"Artifact of Miniaturization. 0 = Disabled, 1 = Artifact Available, 2 = Always Active"
			);
			ShrinkifactMult = Config.Bind(
				"2a-Artifacts", "shrinkifactMult", 0.65f,
				"Size multiplier of Artifact of Miniaturization. Unaffected by sizeclass exponent."
			);
			SplitifactEnable = Config.Bind(
				"2a-Artifacts", "splitifactEnable", 1,
				"Artifact of Fragmentation. 0 = Disabled, 1 = Artifact Available, 2 = Always Active"
			);
			SplitifactMult = Config.Bind(
				"2a-Artifacts", "splitifactMult", 1.5f,
				"Size multiplier per remaining split for Artifact of Fragmentation. Unaffected by sizeclass exponent."
			);
			SplitifactMaxSmallFly = Config.Bind(
				"2a-Artifacts", "splitifactMaxSmallFly", 1,
				"Maximum amount of splits for small flying monsters. Lowest between this value and sizeclass value is chosen."
			);
			SplitifactMaxLesser = Config.Bind(
				"2a-Artifacts", "splitifactMaxLesser", 2,
				"Maximum amount of splits for lesser monsters."
			);
			SplitifactMaxGreater = Config.Bind(
				"2a-Artifacts", "splitifactMaxGreater", 1,
				"Maximum amount of splits for greater monsters."
			);
			SplitifactMaxChampion = Config.Bind(
				"2a-Artifacts", "splitifactMaxChampion", 0,
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
				"Maximum size of players."
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
				"3d-Size - Champion", "greaterSizeLimit", 2.0f,
				"Maximum size of champion monsters."
			);



			ModifierMult = Config.Bind(
				"4a-Modifiers", "modifierMult", 1f,
				"Multiply increase modifier effects in this section."
			);
			KnurlSizeIncrease = Config.Bind(
				"4a-Modifiers", "knurlSizeIncrease", 0.1f,
				"Size increase from Titanic Knurl. Value multiplied by SQRT of item count."
			);
			PearlSizeIncrease = Config.Bind(
				"4a-Modifiers", "pearlSizeIncrease", 0.1f,
				"Size increase from Pearl(x1) and Irradiant Pearl(x2.5). Value multiplied by SQRT of item count."
			);
			StoneFluxSizeIncrease = Config.Bind(
				"4a-Modifiers", "stoneFluxSizeIncrease", 0.2f,
				"Size increase from Stone Flux Pauldron. Value multiplied by SQRT of item count."
			);
			BossSizeIncrease = Config.Bind(
				"4a-Modifiers", "bossSizeIncrease", 0.2f,
				"Size increase from being in a boss group."
			);
			EliteSizeIncrease = Config.Bind(
				"4a-Modifiers", "eliteSizeIncrease", 0.1f,
				"Size increase from having any elite buff."
			);
			EliteCountSizeIncrease = Config.Bind(
				"4a-Modifiers", "eliteCountSizeIncrease", 0.1f,
				"Size increase per elite buff. Value multiplied by SQRT of buff count."
			);
			TonicSizeMult = Config.Bind(
				"4a-Modifiers", "tonicSizeMult", 1.25f,
				"Size multiplier from Spinel Tonic. Unaffected by modifiermult."
			);
		}
	}
}
