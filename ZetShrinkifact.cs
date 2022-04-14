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
			ZetSizeControllerPlugin.RegisterLanguageToken("ARTIFACT_ZETSHRINKIFACT_DESC", "Reduce the size of monsters and players.");
		}
	}
}
