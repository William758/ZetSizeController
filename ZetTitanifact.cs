using RoR2;

namespace TPDespair.ZetSizeController
{
	public static class ZetTitanifact
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
					if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(ZetSizeControllerContent.Artifacts.ZetTitanifact)) return true;

					return false;
				}
			}
		}



		internal static void Init()
		{
			state = Configuration.TitanifactEnable.Value;
			if (state < 1) return;

			ZetSizeControllerPlugin.RegisterLanguageToken("ARTIFACT_ZETTITANIFACT_NAME", "Artifact of the Giants");

			string desc;
			if (Configuration.TitanifactPlayer.Value)
			{
				if (Configuration.TitanifactMonster.Value)
				{
					desc = "Increase the size of monsters and players.";
				}
				else
				{
					desc = "Increase the size of players.";
				}
			}
			else
			{
				if (Configuration.TitanifactMonster.Value)
				{
					desc = "Increase the size of monsters.";
				}
				else
				{
					desc = "Increase the size of nothing?";
				}
			}

			desc += "\n\n<style=cStack>>Size Multiplier: </style>x" + (Configuration.TitanifactMult.Value).ToString("0.##");

			if (Configuration.AllowStatHook.Value)
			{
				if (Configuration.TitanifactHealth.Value != 1f)
				{
					desc += "\n<style=cStack>>Health Multiplier: </style>x" + (Configuration.TitanifactHealth.Value).ToString("0.##");
				}
				if (Configuration.TitanifactDamage.Value != 1f)
				{
					desc += "\n<style=cStack>>Damage Multiplier: </style>x" + (Configuration.TitanifactDamage.Value).ToString("0.##");
				}
				if (Configuration.TitanifactMovement.Value != 1f)
				{
					desc += "\n<style=cStack>>Movement Speed Multiplier: </style>x" + (Configuration.TitanifactMovement.Value).ToString("0.##");
				}
			}

			ZetSizeControllerPlugin.RegisterLanguageToken("ARTIFACT_ZETTITANIFACT_DESC", desc);
		}
	}
}
