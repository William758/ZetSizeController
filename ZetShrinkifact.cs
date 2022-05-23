using RoR2;

namespace TPDespair.ZetSizeController
{
	public static class ZetShrinkifact
	{
		private static int state = 0;

		public static bool Enabled
		{
			get
			{
				if (state < 1) return false;
				else if (state > 1) return true;
				else
				{
					if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(ZetSizeControllerContent.Artifacts.ZetShrinkifact)) return true;

					return false;
				}
			}
		}



		internal static void Init()
		{
			state = Configuration.ShrinkifactEnable.Value;
			if (state < 1) return;

			ZetSizeControllerPlugin.RegisterLanguageToken("ARTIFACT_ZETSHRINKIFACT_NAME", "Artifact of Miniaturization");

			string desc;
			if (Configuration.ShrinkifactPlayer.Value)
			{
				if (Configuration.ShrinkifactMonster.Value)
				{
					desc = "Reduce the size of monsters and players.";
				}
				else
				{
					desc = "Reduce the size of players.";
				}
			}
			else
			{
				if (Configuration.ShrinkifactMonster.Value)
				{
					desc = "Reduce the size of monsters.";
				}
				else
				{
					desc = "Reduce the size of nothing?";
				}
			}

			desc += "\n\n<style=cStack>>Size Multiplier: </style>x" + (Configuration.ShrinkifactMult.Value).ToString("0.##");
			if (Configuration.AllowStatHook.Value)
			{
				if (Configuration.ShrinkifactHealth.Value != 1f)
				{
					desc += "\n<style=cStack>>Health Multiplier: </style>x" + (Configuration.ShrinkifactHealth.Value).ToString("0.##");
				}
				if (Configuration.ShrinkifactDamage.Value != 1f)
				{
					desc += "\n<style=cStack>>Damage Multiplier: </style>x" + (Configuration.ShrinkifactDamage.Value).ToString("0.##");
				}
				if (Configuration.ShrinkifactMovement.Value != 1f)
				{
					desc += "\n<style=cStack>>Movement Speed Multiplier: </style>x" + (Configuration.ShrinkifactMovement.Value).ToString("0.##");
				}
			}

			ZetSizeControllerPlugin.RegisterLanguageToken("ARTIFACT_ZETSHRINKIFACT_DESC", desc);
		}
	}
}
