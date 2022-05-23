using UnityEngine;
using BepInEx;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Linq;
using System.Collections.Generic;

using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete

namespace TPDespair.ZetSizeController
{
	[BepInPlugin(ModGuid, ModName, ModVer)]

	public class ZetSizeControllerPlugin : BaseUnityPlugin
	{
		public const string ModVer = "1.0.3";
		public const string ModName = "ZetSizeController";
		public const string ModGuid = "com.TPDespair.ZetSizeController";

		public static Dictionary<string, string> LangTokens = new Dictionary<string, string>();



		public void Awake()
		{
			RoR2Application.isModded = true;
			NetworkModCompatibilityHelper.networkModList = NetworkModCompatibilityHelper.networkModList.Append(ModGuid + ":" + ModVer);

			Configuration.Init(Config);

			ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;

			SizeManager.Init();

			ZetShrinkifact.Init();
			ZetSplitifact.Init();
			ZetTitanifact.Init();

			if (Configuration.AllowStatHook.Value)
			{
				StatHooks.Init();
			}

			LanguageOverride();
		}
		/*
		public void Update()
		{
			DebugDrops();
		}
		//*/



		private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
		{
			addContentPackProvider(new ZetSizeControllerContent());
		}



		private static void LanguageOverride()
		{
			On.RoR2.Language.TokenIsRegistered += (orig, self, token) =>
			{
				if (token != null)
				{
					if (LangTokens.ContainsKey(token)) return true;
				}

				return orig(self, token);
			};

			On.RoR2.Language.GetString_string += (orig, token) =>
			{
				if (token != null)
				{
					if (LangTokens.ContainsKey(token)) return LangTokens[token];
				}

				return orig(token);
			};
		}

		public static void RegisterLanguageToken(string token, string text)
		{
			if (!LangTokens.ContainsKey(token)) LangTokens.Add(token, text);
			else LangTokens[token] = text;
		}



		public static Sprite CreateSprite(byte[] resourceBytes, Color fallbackColor)
		{
			// Create a temporary texture, then load the texture onto it.
			var tex = new Texture2D(32, 32, TextureFormat.RGBA32, false);
			try
			{
				if (resourceBytes == null)
				{
					FillTexture(tex, fallbackColor);
				}
				else
				{
					tex.LoadImage(resourceBytes, false);
					tex.Apply();
					CleanAlpha(tex);
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.ToString());
				FillTexture(tex, fallbackColor);
			}

			return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(31, 31));
		}

		public static Texture2D FillTexture(Texture2D tex, Color color)
		{
			var pixels = tex.GetPixels();
			for (var i = 0; i < pixels.Length; ++i)
			{
				pixels[i] = color;
			}

			tex.SetPixels(pixels);
			tex.Apply();

			return tex;
		}

		public static Texture2D CleanAlpha(Texture2D tex)
		{
			var pixels = tex.GetPixels();
			for (var i = 0; i < pixels.Length; ++i)
			{
				if (pixels[i].a < 0.05f)
				{
					pixels[i] = Color.clear;
				}
			}

			tex.SetPixels(pixels);
			tex.Apply();

			return tex;
		}


		/*
		private static void DebugDrops()
		{
			if (Input.GetKeyDown(KeyCode.F2))
			{
				var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

				CreateDroplet(RoR2Content.Equipment.AffixWhite, transform.position + new Vector3(-5f, 5f, 5f));
				CreateDroplet(RoR2Content.Items.Knurl, transform.position + new Vector3(0f, 5f, 7.5f));
				CreateDroplet(DLC1Content.Items.HalfSpeedDoubleHealth, transform.position + new Vector3(5f, 5f, 5f));
				//CreateDroplet(Catalog.Equip.AffixHaunted, transform.position + new Vector3(-5f, 5f, -5f));
				//CreateDroplet(Catalog.Equip.AffixPoison, transform.position + new Vector3(0f, 5f, -7.5f));
				//CreateDroplet(Catalog.Equip.AffixLunar, transform.position + new Vector3(5f, 5f, -5f));
			}
			if (Input.GetKeyDown(KeyCode.F3))
			{
				var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

				ItemIndex itemIndex = ItemCatalog.FindItemIndex("TKSATShrinkRay");
				if (itemIndex != ItemIndex.None)
				{
					CreateDroplet(ItemCatalog.GetItemDef(itemIndex), transform.position + new Vector3(-5f, 5f, 5f));
				}
			}
		}

		private static void CreateDroplet(EquipmentDef def, Vector3 pos)
		{
			if (!def) return;

			PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(def.equipmentIndex), pos, Vector3.zero);
		}

		private static void CreateDroplet(ItemDef def, Vector3 pos)
		{
			if (!def) return;

			PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(def.itemIndex), pos, Vector3.zero);
		}
		//*/
	}
}
