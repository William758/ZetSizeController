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
			Buffs.Create();

			ZetSplitifact.TrackerItemDef = Items.ZetSplitTracker;

			contentPack.artifactDefs.Add(Artifacts.artifactDefs.ToArray());
			contentPack.itemDefs.Add(Items.itemDefs.ToArray());
			contentPack.buffDefs.Add(Buffs.buffDefs.ToArray());

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
			public static ArtifactDef ZetTitanifact;

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

				if (Configuration.TitanifactEnable.Value == 1)
				{
					ZetTitanifact = ScriptableObject.CreateInstance<ArtifactDef>();
					ZetTitanifact.cachedName = "ARTIFACT_ZETTITANIFACT";
					ZetTitanifact.nameToken = "ARTIFACT_ZETTITANIFACT_NAME";
					ZetTitanifact.descriptionToken = "ARTIFACT_ZETTITANIFACT_DESC";
					ZetTitanifact.smallIconSelectedSprite = ZetSizeControllerPlugin.CreateSprite(Properties.Resources.zettitan_selected, Color.magenta);
					ZetTitanifact.smallIconDeselectedSprite = ZetSizeControllerPlugin.CreateSprite(Properties.Resources.zettitan_deselected, Color.gray);

					artifactDefs.Add(ZetTitanifact);
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
				#pragma warning disable CS0618 // Type or member is obsolete
				ZetSplitTracker.deprecatedTier = ItemTier.NoTier;
				#pragma warning restore CS0618 // Type or member is obsolete
				ZetSplitTracker.AutoPopulateTokens();
				ZetSplitTracker.hidden = true;
				ZetSplitTracker.canRemove = false;

				itemDefs.Add(ZetSplitTracker);
			}
		}

		public static class Buffs
		{
			public static BuffDef ZetPlayerSizeClass;
			public static BuffDef ZetMonsterSizeClass;

			public static List<BuffDef> buffDefs = new List<BuffDef>();

			public static void Create()
			{
				ZetPlayerSizeClass = ScriptableObject.CreateInstance<BuffDef>();
				ZetPlayerSizeClass.name = "ZetPlayerSizeClass";
				ZetPlayerSizeClass.canStack = false;
				ZetPlayerSizeClass.isDebuff = false;
				ZetPlayerSizeClass.isHidden = true;
				buffDefs.Add(ZetPlayerSizeClass);

				ZetMonsterSizeClass = ScriptableObject.CreateInstance<BuffDef>();
				ZetMonsterSizeClass.name = "ZetMonsterSizeClass";
				ZetMonsterSizeClass.canStack = false;
				ZetMonsterSizeClass.isDebuff = false;
				ZetMonsterSizeClass.isHidden = true;
				buffDefs.Add(ZetMonsterSizeClass);
			}
		}
	}
}
