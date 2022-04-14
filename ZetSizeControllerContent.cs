using RoR2;
using RoR2.ContentManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPDespair.ZetSizeController
{
	public class ZetSizeControllerContent : IContentPackProvider
	{
		public ContentPack contentPack = new ContentPack();

		public string identifier
		{
			get { return "ZetSizeControllerContent"; }
		}

		public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
		{
			Artifacts.Create();
			Items.Create();

			ZetSplitifact.TrackerItemDef = Items.ZetSplitTracker;

			contentPack.artifactDefs.Add(Artifacts.artifactDefs.ToArray());
			contentPack.itemDefs.Add(Items.itemDefs.ToArray());

			args.ReportProgress(1f);
			yield break;
		}

		public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
		{
			ContentPack.Copy(contentPack, args.output);
			args.ReportProgress(1f);
			yield break;
		}

		public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
		{
			args.ReportProgress(1f);
			yield break;
		}



		public static class Artifacts
		{
			public static ArtifactDef ZetShrinkifact;
			public static ArtifactDef ZetSplitifact;

			public static List<ArtifactDef> artifactDefs = new List<ArtifactDef>();

			public static void Create()
			{
				if (Configuration.ShrinkifactEnable.Value == 1)
				{
					ZetShrinkifact = ScriptableObject.CreateInstance<ArtifactDef>();
					ZetShrinkifact.cachedName = "ARTIFACT_ZETSHRINKIFACT";
					ZetShrinkifact.nameToken = "ARTIFACT_ZETSHRINKIFACT_NAME";
					ZetShrinkifact.descriptionToken = "ARTIFACT_ZETSHRINKIFACT_DESC";
					ZetShrinkifact.smallIconSelectedSprite = ZetSizeControllerPlugin.CreateSprite(Properties.Resources.zetshrink_selected, Color.magenta);
					ZetShrinkifact.smallIconDeselectedSprite = ZetSizeControllerPlugin.CreateSprite(Properties.Resources.zetshrink_deselected, Color.gray);

					artifactDefs.Add(ZetShrinkifact);
				}

				if (Configuration.SplitifactEnable.Value == 1)
				{
					ZetSplitifact = ScriptableObject.CreateInstance<ArtifactDef>();
					ZetSplitifact.cachedName = "ARTIFACT_ZETSPLITIFACT";
					ZetSplitifact.nameToken = "ARTIFACT_ZETSPLITIFACT_NAME";
					ZetSplitifact.descriptionToken = "ARTIFACT_ZETSPLITIFACT_DESC";
					ZetSplitifact.smallIconSelectedSprite = ZetSizeControllerPlugin.CreateSprite(Properties.Resources.zetsplit_selected, Color.magenta);
					ZetSplitifact.smallIconDeselectedSprite = ZetSizeControllerPlugin.CreateSprite(Properties.Resources.zetsplit_deselected, Color.gray);

					artifactDefs.Add(ZetSplitifact);
				}
			}
		}

		public static class Items
		{
			public static ItemDef ZetSplitTracker;

			public static List<ItemDef> itemDefs = new List<ItemDef>();

			public static void Create()
			{
				ZetSplitTracker = ScriptableObject.CreateInstance<ItemDef>();
				ZetSplitTracker.name = "ZetSplitTracker";
				ZetSplitTracker.tier = ItemTier.NoTier;
				ZetSplitTracker.AutoPopulateTokens();
				ZetSplitTracker.hidden = true;
				ZetSplitTracker.canRemove = false;

				itemDefs.Add(ZetSplitTracker);
			}
		}
	}
}
