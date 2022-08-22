using EntityStates;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace TPDespair.ZetSizeController
{
	public enum SizeClass
	{
		None,
		Player,
		Lesser,
		Greater,
		Champion
	}

	public enum ExternalEffect
	{
		None = 0,
		TksatShrinkRay = 1
	}



	public class SizeData : MonoBehaviour
	{
		public NetworkInstanceId netId;
		public SizeClass sizeClass = SizeClass.None;

		public float heightVerticalOffset = 0f;
		public float interactionRange = 1f;
		public float playbackSpeed = 1f;

		public Vector3 size;
		public float height = 0f;
		public int validation = 0;
		public bool newData = true;

		public float scale = 1f;
		public float target = 1f;

		public float forceUpdate = 0f;
		public ExternalEffect externalEffect = ExternalEffect.None;

		public bool ready = false;
	}



	public static class SizeManager
	{
		private static readonly List<string> LesserBodyNames = new List<string> { "BackupDroneBody", "BeetleBody", "BeetleCrystalBody", "ClayBody", "Drone1Body", "Drone2Body", "EmergencyDroneBody", "EquipmentDroneBody", "FlameDroneBody", "FlyingVerminBody", "GipBody", "HermitCrabBody", "ImpBody", "JellyfishBody", "LemurianBody", "LunarExploderBody", "MiniMushroomBody", "MinorConstructAttachableBody", "MinorConstructBody", "MinorConstructOnKillBody", "MissileDroneBody", "MoffeinClayManBody", "SquidTurretBody", "Turret1Body", "UrchinTurretBody", "VerminBody", "VoidBarnacleBody", "VoidInfestorBody", "VultureBody", "WispBody", "WispSoulBody" };
		private static readonly List<BodyIndex> LesserBodyIndexes = new List<BodyIndex>();

		private static readonly List<string> GreaterBodyNames = new List<string> { "ArchWispBody", "BeetleGuardAllyBody", "BeetleGuardBody", "BeetleGuardCrystalBody", "BellBody", "BisonBody", "BomberBody", "ClayBruiserBody", "ClayGrenadierBody", "DroneCommanderBody", "EngiBeamTurretBody", "EngiTurretBody", "EngiWalkerTurretBody", "GeepBody", "GolemBody", "GreaterWispBody", "GupBody", "LemurianBruiserBody", "LunarGolemBody", "LunarKnightBody", "LunarWispBody", "MajorConstructBody", "MegaDroneBody", "NullifierBody", "ParentBody", "RoboBallGreenBuddyBody", "RoboBallMiniBody", "RoboBallRedBuddyBody", "ScavBody", "ShopkeeperBody", "VoidJailerBody" };
		private static readonly List<BodyIndex> GreaterBodyIndexes = new List<BodyIndex>();

		private static readonly List<string> ChampionBodyNames = new List<string> { "AncientWispBody", "BeetleQueen2Body", "BrotherBody", "BrotherGlassBody", "BrotherHurtBody", "ClayBossBody", "DireseekerBody", "ElectricWormBody", "GrandParentBody", "GravekeeperBody", "ImpBossBody", "MagmaWormBody", "MegaConstructBody", "MoffeinAncientWispBody", "RoboBallBossBody", "ScavLunar1Body", "ScavLunar2Body", "ScavLunar3Body", "ScavLunar4Body", "SuperRoboBallBossBody", "TitanBody", "TitanGoldBody", "VagrantBody", "VoidMegaCrabBody" };
		private static readonly List<BodyIndex> ChampionBodyIndexes = new List<BodyIndex>();

		private static BodyIndex BeetleBodyIndex = BodyIndex.None;
		private static BodyIndex BeetleCrystalBodyIndex = BodyIndex.None;
		private static BodyIndex ImpBodyIndex = BodyIndex.None;
		private static BodyIndex LemurianBodyIndex = BodyIndex.None;
		private static BodyIndex RexBodyIndex = BodyIndex.None;
		private static BodyIndex ClayableTemplarBodyIndex = BodyIndex.None;

		private static BuffIndex ShrinkRayBuff = BuffIndex.None;

		private static ItemTier LunarVoidTier = ItemTier.AssignedAtRuntime;

		public static Action<CharacterBody, SizeData> onSizeDataCreated;



		internal static void Init()
		{
			BodyCatalog.availability.CallWhenAvailable(PopulateBodyIndexes);
			RoR2Application.onLoad += PopulateIndexes;

			RecalculateStatsHook();
			FixedUpdateHook();
			OnDestroyHook();

			CharacterBody.onBodyStartGlobal += RecalcSizeData;
			VehicleSeat.onPassengerExitGlobal += ResizeOnVehicleExit;

			FixPrintController();

			if (Configuration.ModifyCamera.Value)
			{
				CameraDistanceHook();
				CameraVerticalOffsetHook();
			}
			if (Configuration.ModifyInteraction.Value)
			{
				InteractionDriverHook();
				PickupPickerHook();
			}
			if (Configuration.ModifyOverlap.Value)
			{
				OverlapAttackPositionHook();
				OverlapAttackScaleHook();
			}
			if (Configuration.ModifyAnimation.Value)
			{
				AnimationHook();
			}
		}

		

		private static void PopulateBodyIndexes()
		{
			BodyIndex bodyIndex;

			foreach (string lesserName in LesserBodyNames)
			{
				bodyIndex = BodyCatalog.FindBodyIndex(lesserName);
				if (bodyIndex != BodyIndex.None)
				{
					if (!LesserBodyIndexes.Contains(bodyIndex))
					{
						LesserBodyIndexes.Add(bodyIndex);
					}
				}
			}

			foreach (string greaterName in GreaterBodyNames)
			{
				bodyIndex = BodyCatalog.FindBodyIndex(greaterName);
				if (bodyIndex != BodyIndex.None)
				{
					if (!GreaterBodyIndexes.Contains(bodyIndex))
					{
						GreaterBodyIndexes.Add(bodyIndex);
					}
				}
			}

			foreach (string championName in ChampionBodyNames)
			{
				bodyIndex = BodyCatalog.FindBodyIndex(championName);
				if (bodyIndex != BodyIndex.None)
				{
					if (!ChampionBodyIndexes.Contains(bodyIndex))
					{
						ChampionBodyIndexes.Add(bodyIndex);
					}
				}
			}

			BeetleBodyIndex = BodyCatalog.FindBodyIndex("BeetleBody");
			BeetleCrystalBodyIndex = BodyCatalog.FindBodyIndex("BeetleCrystalBody");
			ImpBodyIndex = BodyCatalog.FindBodyIndex("ImpBody");
			LemurianBodyIndex = BodyCatalog.FindBodyIndex("LemurianBody");
			RexBodyIndex = BodyCatalog.FindBodyIndex("TreebotBody");
			ClayableTemplarBodyIndex = BodyCatalog.FindBodyIndex("Templar_Survivor");

			Debug.LogWarning("ZetSizeController - PopulateBodyIndexes : Lesser[" + LesserBodyIndexes.Count + "] Greater[" + GreaterBodyIndexes.Count + "] Champion[" + ChampionBodyIndexes.Count + "]");
		}

		private static void PopulateIndexes()
		{
			BuffIndex buffIndex = BuffCatalog.FindBuffIndex("TKSATShrink");
			if (buffIndex != BuffIndex.None)
			{
				ShrinkRayBuff = buffIndex;
				//Debug.LogWarning("ZetSplitifact - ShrinkRayBuff : " + ShrinkRayBuff);
			}

			ItemTierDef itemTierDef = ItemTierCatalog.FindTierDef("VoidLunarTierDef");
			if (itemTierDef)
			{
				LunarVoidTier = itemTierDef.tier;
				//Debug.LogWarning("ZetSplitifact - LunarVoidTier : " + LunarVoidTier);
			}
		}



		private static void RecalculateStatsHook()
		{
			On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
			{
				// create before stats calc for artifact stat effects
				GetSizeData(self);

				orig(self);

				RecalcSizeData(self);
			};
		}

		private static void RecalcSizeData(CharacterBody self)
		{
			SizeData sizeData = GetSizeData(self);
			if (sizeData)
			{
				float value = GetCharacterScale(self, sizeData);

				if (sizeData.target != value)
				{
					if (sizeData.sizeClass == SizeClass.Player)
					{
						Debug.LogWarning("Player Size : " + sizeData.netId + " - " + $"{sizeData.target:0.###}" + " => " + $"{value:0.###}");
					}

					sizeData.target = value;
				}

				if (sizeData.newData)
				{
					sizeData.newData = false;

					UpdateSize(self, true);
				}
			}
		}

		private static SizeData GetSizeData(CharacterBody body)
		{
			// force masterObject assignment by reading it
			if (body && body.masterObject)
			{
				SizeData sizeData = body.gameObject.GetComponent<SizeData>();

				if (!sizeData)
				{
					if (!HasTransform(body)) return null;
					if (!ValidateScale(body)) return null;

					SizeClass sizeClass = GetSizeClass(body);
					if (sizeClass == SizeClass.None) return null;

					sizeData = body.gameObject.AddComponent<SizeData>();
					sizeData.netId = body.netId;
					sizeData.size = body.modelLocator.modelTransform.localScale;
					sizeData.height = Mathf.Abs(body.corePosition.y - body.footPosition.y) * 2f;
					sizeData.sizeClass = sizeClass;

					if (sizeClass != SizeClass.Player && Configuration.ValidateMonsterSize.Value)
					{
						// 2 fixedupdate of waiting , 1 fixedupdate instant update size
						sizeData.validation = 3;
					}

					sizeData.newData = true;

					Action<CharacterBody, SizeData> action = onSizeDataCreated;
					if (action != null)
					{
						#pragma warning disable IDE1005 // Delegate invocation can be simplified.
						action(body, sizeData);
						#pragma warning restore IDE1005 // Delegate invocation can be simplified.
					}

					sizeData.ready = true;

					if (sizeClass == SizeClass.Player)
					{
						body.SetBuffCount(ZetSizeControllerContent.Buffs.ZetPlayerSizeClass.buffIndex, 1);

						Debug.LogWarning("Created Player SizeData : " + sizeData.netId);
						Debug.LogWarning("-- Height : " + sizeData.height);
					}
					else
					{
						body.SetBuffCount(ZetSizeControllerContent.Buffs.ZetMonsterSizeClass.buffIndex, 1);
					}
				}

				// wait until onSizeDataCreated is finished
				// onSizeDataCreated : splitifact rollcount updates inventory which triggers this function again
				if (!sizeData.ready) return null;

				return sizeData;
			}

			return null;
		}

		private static bool HasTransform(CharacterBody self)
		{
			return self.modelLocator && self.modelLocator.modelTransform;
		}

		private static bool ValidateScale(CharacterBody self)
		{
			if (self.bodyIndex == RexBodyIndex)
			{
				if (Mathf.Abs(self.modelLocator.modelTransform.localScale.x - 0.75f) >= 0.01f) return false;
			}

			if (self.bodyIndex == ClayableTemplarBodyIndex)
			{
				if (Mathf.Abs(self.modelLocator.modelTransform.localScale.x - 0.9f) >= 0.01f) return false;
			}

			return true;
		}

		private static SizeClass GetSizeClass(CharacterBody self)
		{
			if (self.isPlayerControlled)
			{
				return SizeClass.Player;
			}
			else
			{
				if (LesserBodyIndexes.Contains(self.bodyIndex))
				{
					return SizeClass.Lesser;
				}
				if (GreaterBodyIndexes.Contains(self.bodyIndex))
				{
					return SizeClass.Greater;
				}
				if (ChampionBodyIndexes.Contains(self.bodyIndex))
				{
					return SizeClass.Champion;
				}
			}

			return SizeClass.None;
		}

		private static float GetCharacterScale(CharacterBody self, SizeData sizeData)
		{
			float increase, multiplier, exponent, maximum, minimum;

			switch (sizeData.sizeClass)
			{
				case SizeClass.Player:
					{
						increase = Configuration.PlayerSizeIncrease.Value;
						multiplier = Configuration.PlayerSizeMult.Value;
						exponent = Configuration.PlayerSizeExponent.Value;
						maximum = Configuration.PlayerSizeLimit.Value;
						minimum = 0.5f;
					}
					break;
				case SizeClass.Lesser:
					{
						increase = Configuration.LesserSizeIncrease.Value;
						multiplier = Configuration.LesserSizeMult.Value;
						exponent = Configuration.LesserSizeExponent.Value;
						maximum = Configuration.LesserSizeLimit.Value;
						minimum = 0.5f;
					}
					break;
				case SizeClass.Greater:
					{
						increase = Configuration.GreaterSizeIncrease.Value;
						multiplier = Configuration.GreaterSizeMult.Value;
						exponent = Configuration.GreaterSizeExponent.Value;
						maximum = Configuration.GreaterSizeLimit.Value;
						minimum = 0.35f;
					}
					break;
				case SizeClass.Champion:
					{
						increase = Configuration.ChampionSizeIncrease.Value;
						multiplier = Configuration.ChampionSizeMult.Value;
						exponent = Configuration.ChampionSizeExponent.Value;
						maximum = Configuration.ChampionSizeLimit.Value;
						minimum = 0.25f;
					}
					break;
				default:
					{
						return 1f;
					}
			}

			maximum = Mathf.Min(Configuration.AbsoluteSizeLimit.Value, maximum);

			if (Configuration.IgnoreSizeLimit.Value)
			{
				maximum = 999f;
				minimum = 0.1f;
			}

			if (Configuration.IgnoreSizeExponent.Value)
			{
				exponent = 1f;
			}

			float ignoreExponentMultiplier = 1f;
			float modifier = Configuration.ModifierMult.Value;

			int count;
			Inventory inventory = self.inventory;
			if (inventory)
			{
				float itemCatLimit = Configuration.CoreItemModifierLimit.Value;

				count = inventory.GetItemCount(RoR2Content.Items.Knurl);
				if (count > 0)
				{
					increase += modifier * Configuration.KnurlSizeIncrease.Value * Mathf.Min(itemCatLimit, Mathf.Sqrt(count));
				}

				count = 10 * self.inventory.GetItemCount(RoR2Content.Items.Pearl);
				count += 25 * self.inventory.GetItemCount(RoR2Content.Items.ShinyPearl);
				if (count > 0)
				{
					increase += modifier * Configuration.PearlSizeIncrease.Value * Mathf.Min(itemCatLimit, Mathf.Sqrt(count / 10f));
				}

				count = inventory.GetItemCount(DLC1Content.Items.HalfSpeedDoubleHealth);
				if (count > 0)
				{
					increase += modifier * Configuration.StoneFluxSizeIncrease.Value * Mathf.Min(itemCatLimit, Mathf.Sqrt(count));
				}

				count = inventory.GetItemCount(ZetSizeControllerContent.Items.ZetSplitTracker);
				if (count > 1)
				{
					ignoreExponentMultiplier *= Mathf.Pow(Configuration.SplitifactMult.Value, count - 1);
				}

				count = inventory.GetItemCount(RoR2Content.Items.LunarDagger);
				if (count > 0)
				{
					ignoreExponentMultiplier *= Mathf.Max(Configuration.GlassModifierLimit.Value, Mathf.Pow(Configuration.GlassSizeMult.Value, count));
				}

				increase += modifier * GetItemScoreScaling(inventory);
			}

			if (self.isBoss)
			{
				increase += modifier * Configuration.BossSizeIncrease.Value;
			}

			count = self.eliteBuffCount;
			if (count > 0)
			{
				increase += modifier * Configuration.EliteSizeIncrease.Value;
				increase += modifier * Configuration.EliteCountSizeIncrease.Value * Mathf.Min(5f, Mathf.Sqrt(count));
			}

			if (self.HasBuff(RoR2Content.Buffs.TonicBuff))
			{
				ignoreExponentMultiplier *= Configuration.TonicSizeMult.Value;
			}

			if (self.HasBuff(ShrinkRayBuff))
			{
				ignoreExponentMultiplier *= 0.65f;
			}

			increase += modifier * GetLevelScaling(sizeData.sizeClass == SizeClass.Player, self.level);

			if (increase < 0f)
			{
				increase = -0.01f * Util.ConvertAmplificationPercentageIntoReductionPercentage(Mathf.Abs(increase) * 100f);
			}

			float value = Mathf.Max(0.1f, multiplier * (1f + increase));

			if (value > 1f)
			{
				value = Mathf.Pow(value, exponent);
			}

			if (ZetShrinkifact.Enabled)
			{
				if (sizeData.sizeClass == SizeClass.Player)
				{
					if (Configuration.ShrinkifactPlayer.Value)
					{
						ignoreExponentMultiplier *= Configuration.ShrinkifactMult.Value;
						if (Configuration.ShrinkifactExtend.Value)
						{
							minimum *= Configuration.ShrinkifactMult.Value;
						}
					}
				}
				else
				{
					if (Configuration.ShrinkifactMonster.Value)
					{
						ignoreExponentMultiplier *= Configuration.ShrinkifactMult.Value;
						if (Configuration.ShrinkifactExtend.Value)
						{
							minimum *= Configuration.ShrinkifactMult.Value;
						}
					}
				}
			}

			if (ZetTitanifact.Enabled)
			{
				if (sizeData.sizeClass == SizeClass.Player)
				{
					if (Configuration.TitanifactPlayer.Value)
					{
						ignoreExponentMultiplier *= Configuration.TitanifactMult.Value;
						if (Configuration.TitanifactExtend.Value && Configuration.PlayerSizeLimitExtendable.Value)
						{
							maximum *= Configuration.TitanifactMult.Value;
						}
					}
				}
				else
				{
					if (Configuration.TitanifactMonster.Value)
					{
						ignoreExponentMultiplier *= Configuration.TitanifactMult.Value;
						if (Configuration.TitanifactExtend.Value)
						{
							maximum *= Configuration.TitanifactMult.Value;
						}
					}
				}
			}

			minimum = Mathf.Max(minimum, 0.1f);

			value *= ignoreExponentMultiplier;

			value = Mathf.Clamp(value, minimum, Mathf.Max(minimum, maximum));

			return value;
		}

		private static float GetItemScoreScaling(Inventory inventory)
		{
			float itemScoreEffect = Configuration.ItemScoreSizeIncrease.Value;
			if (itemScoreEffect > 0f)
			{
				float itemScore = 0f;

				float t1 = Configuration.ItemScoreT1Effect.Value;
				float t2 = Configuration.ItemScoreT2Effect.Value;
				float t3 = Configuration.ItemScoreT3Effect.Value;
				float boss = Configuration.ItemScoreBossEffect.Value;
				float lunar = Configuration.ItemScoreLunarEffect.Value;

				itemScore += t1 * inventory.GetTotalItemCountOfTier(ItemTier.Tier1);
				itemScore += t1 * inventory.GetTotalItemCountOfTier(ItemTier.VoidTier1);

				itemScore += t2 * inventory.GetTotalItemCountOfTier(ItemTier.Tier2);
				itemScore += t2 * inventory.GetTotalItemCountOfTier(ItemTier.VoidTier2);

				itemScore += t3 * inventory.GetTotalItemCountOfTier(ItemTier.Tier3);
				itemScore += t3 * inventory.GetTotalItemCountOfTier(ItemTier.VoidTier3);

				itemScore += boss * inventory.GetTotalItemCountOfTier(ItemTier.Boss);
				itemScore += boss * inventory.GetTotalItemCountOfTier(ItemTier.VoidBoss);

				itemScore += lunar * inventory.GetTotalItemCountOfTier(ItemTier.Lunar);
				if (LunarVoidTier != ItemTier.AssignedAtRuntime)
				{
					itemScore += lunar * inventory.GetTotalItemCountOfTier(LunarVoidTier);
				}

				if (itemScore > 0f)
				{
					itemScoreEffect *= Mathf.Pow(itemScore, Configuration.ItemScoreSizeExponent.Value);

					return Mathf.Min(itemScoreEffect, Configuration.ItemScoreSizeLimit.Value);
				}
			}

			return 0f;
		}

		private static float GetLevelScaling(bool isPlayer, float level)
		{
			if (level > 1.99f)
			{
				float levelSizeIncrease, levelSizeExponent, levelSizeLimit;

				if (isPlayer)
				{
					levelSizeIncrease = Mathf.Abs(Configuration.PlayerLevelSizeIncrease.Value);
					if (levelSizeIncrease == 0f) return 0f;
					levelSizeExponent = Configuration.PlayerLevelSizeExponent.Value;
					levelSizeLimit = Configuration.PlayerLevelSizeLimit.Value;
				}
				else
				{
					levelSizeIncrease = Mathf.Abs(Configuration.MonsterLevelSizeIncrease.Value);
					if (levelSizeIncrease == 0f) return 0f;
					levelSizeExponent = Configuration.MonsterLevelSizeExponent.Value;
					levelSizeLimit = Configuration.MonsterLevelSizeLimit.Value;
				}

				levelSizeIncrease *= Mathf.Pow(level - 1f, levelSizeExponent);

				return Mathf.Min(levelSizeIncrease, levelSizeLimit);
			}

			return 0f;
		}



		private static void FixedUpdateHook()
		{
			On.RoR2.CharacterBody.FixedUpdate += (orig, self) =>
			{
				orig(self);

				UpdateSize(self, false);
			};
		}

		private static void UpdateSize(CharacterBody self, bool instant)
		{
			if (self)
			{
				SizeData sizeData = self.gameObject.GetComponent<SizeData>();
				if (sizeData)
				{
					float deltaTime = Time.fixedDeltaTime;
					bool validating = Configuration.ValidateMonsterSize.Value;
					bool isPlayer = sizeData.sizeClass == SizeClass.Player;

					if (!validating || isPlayer || sizeData.validation <= 1)
					{
						if (validating && !isPlayer && sizeData.validation == 1)
						{
							sizeData.size = self.modelLocator.modelTransform.localScale;

							instant = true;
						}

						TrackExternalEffects(self, sizeData);

						bool update = false;
						bool forcedUpdate = sizeData.forceUpdate > 0f;

						if (forcedUpdate || sizeData.scale != sizeData.target)
						{
							update = true;

							float rate = Configuration.SizeChangeRate.Value;

							if (instant || forcedUpdate || rate <= 0f)
							{
								sizeData.scale = sizeData.target;

								//if (sizeData.sizeClass == SizeClass.Player) Debug.LogWarning("Player Size : " + sizeData.netId + " - " + sizeData.scale);
							}
							else
							{
								float delta = deltaTime * sizeData.scale * rate;

								if (sizeData.scale < sizeData.target)
								{
									sizeData.scale = Mathf.Min(sizeData.scale + delta, sizeData.target);
								}
								else
								{
									sizeData.scale = Mathf.Max(sizeData.scale - delta, sizeData.target);
								}

								//if (sizeData.sizeClass == SizeClass.Player) Debug.LogWarning("Player Size : " + sizeData.netId + " - " + sizeData.scale + " => " + sizeData.target);
							}
						}

						//if (sizeData.sizeClass == SizeClass.Player) Debug.LogWarning(">> Current Scale - " + self.modelLocator.modelTransform.localScale.x);

						if (update && HasTransform(self))
						{
							SetSize(self, sizeData);
						}

						if (sizeData.forceUpdate > 0f)
						{
							sizeData.forceUpdate = Mathf.Max(0f, sizeData.forceUpdate - deltaTime);
						}
					}

					if (sizeData.validation > 0)
					{
						sizeData.validation--;
					}
				}
			}
		}

		private static void TrackExternalEffects(CharacterBody self, SizeData sizeData)
		{
			ExternalEffect external = ExternalEffect.None;

			if (self.HasBuff(ShrinkRayBuff))
			{
				external |= ExternalEffect.TksatShrinkRay;
			}

			if (sizeData.externalEffect != external)
			{
				sizeData.externalEffect = external;

				sizeData.forceUpdate = 0.1f;
			}
		}

		private static void SetSize(CharacterBody body, SizeData sizeData)
		{
			bool isRex = body.bodyIndex == RexBodyIndex;

			Vector3 size = sizeData.size;
			float scale = sizeData.scale;

			body.modelLocator.modelTransform.localScale = new Vector3(size.x * scale, size.y * scale, size.z * scale);

			float vho = (sizeData.height / 2f) * (scale - 1f);
			if (isRex) vho *= 1.75f;
			sizeData.heightVerticalOffset = vho;

			sizeData.playbackSpeed = Mathf.Clamp(1f / scale, 0.125f, 4f);

			float divisor = isRex ? 0.4f : 2f;
			sizeData.interactionRange = 1f + Mathf.Max(0f, scale - 1f) / divisor;
		}



		private static void OnDestroyHook()
		{
			On.RoR2.CharacterBody.OnDestroy += (orig, self) =>
			{
				DestroySizeData(self);

				orig(self);
			};
		}

		private static void DestroySizeData(CharacterBody self)
		{
			if (self)
			{
				SizeData sizeData = self.gameObject.GetComponent<SizeData>();
				if (sizeData)
				{
					if (sizeData.sizeClass == SizeClass.Player)
					{
						Debug.LogWarning("Destroying Player SizeData : " + sizeData.netId);
					}

					UnityEngine.Object.Destroy(sizeData);
				}
			}
		}



		private static void ResizeOnVehicleExit(VehicleSeat seat, GameObject gameObject)
		{
			CharacterBody body = gameObject.GetComponent<CharacterBody>();
			if (body)
			{
				SizeData sizeData = gameObject.GetComponent<SizeData>();
				if (sizeData)
				{
					sizeData.scale = sizeData.target - 0.01f;

					UpdateSize(body, true);
				}
			}
		}



		private static void FixPrintController()
		{
			IL.RoR2.PrintController.SetPrintThreshold += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdfld<PrintController>("maxPrintHeight")
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldarg, 1);
					c.EmitDelegate<Func<PrintController, float, float>>((printController, sample) =>
					{
						CharacterModel model = printController.characterModel;
						if (model)
						{
							CharacterBody body = model.body;
							if (body)
							{
								if (body.bodyIndex == ClayableTemplarBodyIndex)
								{
									return 1f;
								}

								if (sample >= 1f) return 100f;

								SizeData sizeData = body.gameObject.GetComponent<SizeData>();
								if (sizeData)
								{
									return Mathf.Max(1f, sizeData.scale);
								}
							}
						}

						return 1f;
					});
					c.Emit(OpCodes.Mul);
				}
				else
				{
					Debug.LogWarning("ZetSizeController - FixPrintController Failed");
				}
			};
		}



		private static void CameraDistanceHook()
		{
			IL.RoR2.CameraModes.CameraModePlayerBasic.UpdateInternal += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchStloc(13)
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Ldloc, 13);
					c.Emit(OpCodes.Ldloc, 1);
					c.EmitDelegate<Func<Vector3, CameraRigController, Vector3>>((direction, camRig) =>
					{
						CharacterBody body = camRig.targetBody;
						if (body)
						{
							SizeData sizeData = body.gameObject.GetComponent<SizeData>();
							if (sizeData)
							{
								return direction * sizeData.scale;
							}
						}

						return direction;
					});
					c.Emit(OpCodes.Stloc, 13);
				}
				else
				{
					Debug.LogWarning("ZetSizeController - CameraDistanceHook Failed");
				}
			};
		}

		private static void CameraVerticalOffsetHook()
		{
			IL.RoR2.CameraModes.CameraModePlayerBasic.CalculateTargetPivotPosition += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdflda<CharacterCameraParamsData>("pivotVerticalOffset"),
					x => x.MatchLdfld<HG.BlendableTypes.BlendableFloat>("value")
				);

				if (found)
				{
					c.Index += 2;

					c.Emit(OpCodes.Ldloc, 0);
					c.EmitDelegate<Func<float, CameraRigController, float>>((vertical, camRig) =>
					{
						CharacterBody body = camRig.targetBody;
						if (body)
						{
							SizeData sizeData = body.gameObject.GetComponent<SizeData>();
							if (sizeData)
							{
								return (vertical * sizeData.scale) + sizeData.heightVerticalOffset;
							}
						}

						return vertical;
					});
				}
				else
				{
					Debug.LogWarning("ZetSizeController - CameraVerticalOffsetHook Failed");
				}
			};
		}



		private static void InteractionDriverHook()
		{
			IL.RoR2.InteractionDriver.FindBestInteractableObject += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchCallOrCallvirt<InteractionDriver>("get_interactor"),
					x => x.MatchLdfld<Interactor>("maxInteractionDistance"),
					x => x.MatchStloc(3)
				);

				if (found)
				{
					c.Index += 4;

					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 3);
					c.EmitDelegate<Func<InteractionDriver, float, float>>((driver, range) =>
					{
						CharacterBody body = driver.characterBody;
						if (body)
						{
							SizeData sizeData = body.gameObject.GetComponent<SizeData>();
							if (sizeData)
							{
								range *= sizeData.interactionRange;
							}
						}

						return range;
					});
					c.Emit(OpCodes.Stloc, 3);
				}
				else
				{
					Debug.LogWarning("ZetSizeController - InteractionHook Failed");
				}
			};
		}

		private static void PickupPickerHook()
		{
			IL.RoR2.PickupPickerController.FixedUpdateServer += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<PickupPickerController>("cutoffDistance"),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<PickupPickerController>("cutoffDistance"),
					x => x.MatchMul()
				);

				if (found)
				{
					c.Index += 5;

					c.Emit(OpCodes.Ldloc, 1);
					c.EmitDelegate<Func<float, CharacterBody, float>>((cutoff, body) =>
					{
						SizeData sizeData = body.gameObject.GetComponent<SizeData>();
						if (sizeData)
						{
							float value = sizeData.interactionRange;
							cutoff *= value * value;
						}

						return cutoff;
					});
				}
				else
				{
					Debug.LogWarning("ZetSizeController - PickupPickerHook Failed");
				}
			};
		}



		private static void OverlapAttackPositionHook()
		{
			IL.RoR2.OverlapAttack.Fire += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchStloc(3)
				);

				if (found)
				{
					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<Vector3, OverlapAttack, Vector3>>((position, attack) =>
					{
						GameObject atkObject = attack.attacker;
						if (atkObject)
						{
							CharacterBody body = atkObject.GetComponent<CharacterBody>();
							if (body)
							{
								Vector3 offset = position - body.corePosition;

								if (body.bodyIndex == BeetleBodyIndex || body.bodyIndex == BeetleCrystalBodyIndex)
								{
									offset = new Vector3(offset.x, offset.y * 0.5f, offset.z);
								}

								position = body.corePosition + offset;
							}
						}

						return position;
					});
				}
				else
				{
					Debug.LogWarning("ZetSizeController - OverlapAttackPositionHook Failed");
				}
			};
		}

		private static void OverlapAttackScaleHook()
		{
			IL.RoR2.OverlapAttack.Fire += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchStloc(4)
				);

				if (found)
				{
					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<Vector3, OverlapAttack, Vector3>>((scale, attack) =>
					{
						GameObject atkObject = attack.attacker;
						if (atkObject)
						{
							CharacterBody body = atkObject.GetComponent<CharacterBody>();
							if (body)
							{
								if (body.bodyIndex == BeetleBodyIndex || body.bodyIndex == BeetleCrystalBodyIndex)
								{
									scale = new Vector3(scale.x * 1.25f, scale.y * 2.5f, scale.z * 1.25f);
								}
								if (body.bodyIndex == ImpBodyIndex || body.bodyIndex == LemurianBodyIndex)
								{
									scale *= 1.75f;
								}
							}
						}

						return scale;
					});
				}
				else
				{
					Debug.LogWarning("ZetSizeController - OverlapAttackScaleHook Failed");
				}
			};
		}



		private static void AnimationHook()
		{
			IL.EntityStates.BaseCharacterMain.UpdateAnimationParameters += (il) =>
			{
				ILCursor c = new ILCursor(il);

				VariableDefinition playbackSpeed = new VariableDefinition(il.Body.Method.Module.TypeSystem.Single);
				il.Body.Variables.Add(playbackSpeed);

				c.Index = 0;

				c.Emit(OpCodes.Ldarg, 0);
				c.EmitDelegate<Func<EntityState, float>>((entityState) =>
				{
					CharacterBody body = entityState.characterBody;
					if (body)
					{
						SizeData sizeData = body.GetComponent<SizeData>();
						if (sizeData)
						{
							return sizeData.playbackSpeed;
						}
					}

					return 1f;
				});
				c.Emit(OpCodes.Stloc, playbackSpeed);

				bool found = c.TryGotoNext(
					x => x.MatchLdsfld(typeof(AnimationParameters).GetField("mainRootPlaybackRate")),
					x => x.MatchLdloc(3)
				);

				if (found)
				{
					c.Index += 2;

					c.Emit(OpCodes.Ldloc, playbackSpeed);
					c.Emit(OpCodes.Mul);
				}
				else
				{
					Debug.LogWarning("ZetSizeController - AnimationHook:rootMotion Failed");
				}

				found = c.TryGotoNext(
					x => x.MatchLdsfld(typeof(AnimationParameters).GetField("walkSpeed")),
					x => x.MatchLdarg(0),
					x => x.MatchCall<EntityState>("get_characterBody"),
					x => x.MatchCallvirt<CharacterBody>("get_moveSpeed")
				);

				if (found)
				{
					c.Index += 4;

					c.Emit(OpCodes.Ldloc, playbackSpeed);
					c.Emit(OpCodes.Mul);
				}
				else
				{
					Debug.LogWarning("ZetSizeController - AnimationHook:walkSpeed Failed");
				}
			};
		}
	}
}
