using EntityStates;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;
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

		private static BuffIndex HeadhunterBuff = BuffIndex.None;
		private static ItemTier LunarVoidTier = ItemTier.AssignedAtRuntime;
		private static ItemIndex RelicMassIndex = ItemIndex.None;



		internal static void Init()
		{
			BodyCatalog.availability.CallWhenAvailable(PopulateBodyIndexes);
			RoR2Application.onLoad += PopulateIndexes;

			if (Configuration.ModifyCamera.Value)
			{
				CameraDistanceHook();
				CameraVerticalOffsetHook();
				/*
				MethodInfo target = AccessTools.Method(typeof(InputBankTest), "get_aimOrigin");
				if (target != null)
				{
					ZetSizeControllerPlugin.harmony.Patch(target, postfix: new HarmonyMethod(typeof(SizeManager), nameof(InputBankAimOriginPostfix)));
				}
				*/
			}
			if (Configuration.ModifyInteraction.Value)
			{
				InteractionDriverHook();
				PickupPickerHook();
				TieredPickupPickerHook();
				DroneScrapperPickerHook();
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

			ZetSizeControllerPlugin.LogWarn("PopulateBodyIndexes : Lesser[" + LesserBodyIndexes.Count + "] Greater[" + GreaterBodyIndexes.Count + "] Champion[" + ChampionBodyIndexes.Count + "]");
		}

		private static void PopulateIndexes()
		{
			BuffIndex buffIndex = BuffCatalog.FindBuffIndex("ZetHeadHunter");
			if (buffIndex != BuffIndex.None)
			{
				HeadhunterBuff = buffIndex;
			}

			ItemIndex itemIndex = ItemCatalog.FindItemIndex("RelicOfMass");
			if (itemIndex != ItemIndex.None)
			{
				RelicMassIndex = itemIndex;
			}

			ItemTierDef itemTierDef = ItemTierCatalog.FindTierDef("VoidLunarTierDef");
			if (itemTierDef)
			{
				LunarVoidTier = itemTierDef.tier;
			}
		}



		public static float GetScaleMult(CharacterBody body)
		{
			float increase, multiplier, exponent, maximum, minimum;

			SizeClass sizeClass = GetSizeClass(body);
			switch (sizeClass)
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
			Inventory inventory = body.inventory;
			if (inventory)
			{
				float itemCatLimit = Configuration.CoreItemModifierLimit.Value;

				count = inventory.GetItemCountEffective(RoR2Content.Items.Knurl);
				if (count > 0)
				{
					increase += modifier * Configuration.KnurlSizeIncrease.Value * Mathf.Min(itemCatLimit, Mathf.Sqrt(count));
				}

				count = 10 * inventory.GetItemCountEffective(RoR2Content.Items.Pearl);
				count += 25 * inventory.GetItemCountEffective(RoR2Content.Items.ShinyPearl);
				if (count > 0)
				{
					increase += modifier * Configuration.PearlSizeIncrease.Value * Mathf.Min(itemCatLimit, Mathf.Sqrt(count / 10f));
				}

				count = inventory.GetItemCountEffective(DLC1Content.Items.HalfSpeedDoubleHealth);
				if (count > 0)
				{
					increase += modifier * Configuration.StoneFluxSizeIncrease.Value * Mathf.Min(itemCatLimit, Mathf.Sqrt(count));
				}

				count = inventory.GetItemCountEffective(DLC2Content.Items.BoostAllStats);
				if (count > 0)
				{
					increase += modifier * Configuration.NectarSizeIncrease.Value * Mathf.Min(itemCatLimit, Mathf.Sqrt(count));
				}

				if (RelicMassIndex != ItemIndex.None)
				{
					count = inventory.GetItemCountEffective(RelicMassIndex);
					if (count > 0)
					{
						increase += modifier * Configuration.StoneFluxSizeIncrease.Value * Mathf.Min(itemCatLimit, Mathf.Sqrt(count));
					}
				}

				count = inventory.GetItemCountEffective(ZetSizeControllerContent.Items.ZetSplitTracker);
				if (count > 1)
				{
					ignoreExponentMultiplier *= Mathf.Pow(Configuration.SplitifactMult.Value, count - 1);
				}

				count = inventory.GetItemCountEffective(RoR2Content.Items.LunarDagger);
				if (count > 0)
				{
					ignoreExponentMultiplier *= Mathf.Max(Configuration.GlassModifierLimit.Value, Mathf.Pow(Configuration.GlassSizeMult.Value, count));
				}

				increase += modifier * GetItemScoreScaling(inventory);
			}

			if (body.isBoss)
			{
				increase += modifier * Configuration.BossSizeIncrease.Value;
			}

			count = body.eliteBuffCount;
			if (count > 0)
			{
				increase += modifier * Configuration.EliteSizeIncrease.Value;

				increase += modifier * Configuration.EliteCountSizeIncrease.Value * Mathf.Pow(count, Configuration.EliteCountExponent.Value);
			}

			count = body.GetBuffCount(HeadhunterBuff);
			if (count > 0)
			{
				increase += modifier * Configuration.HeadhunterSizeIncrease.Value * Mathf.Pow(count, Configuration.HeadhunterExponent.Value);
			}

			if (body.HasBuff(RoR2Content.Buffs.TonicBuff))
			{
				ignoreExponentMultiplier *= Configuration.TonicSizeMult.Value;
			}

			increase += modifier * GetLevelScaling(sizeClass == SizeClass.Player, body.level);

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
				if (sizeClass == SizeClass.Player)
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
				if (sizeClass == SizeClass.Player)
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



		public static float GetInteractionRangeMult(CharacterBody body)
		{
			return 1f + Mathf.Max(0f, body.currentScale - 1f) / 2f;
		}

		public static SizeClass GetSizeClass(CharacterBody body)
		{
			if (body.isPlayerControlled)
			{
				return SizeClass.Player;
			}
			else
			{
				if (LesserBodyIndexes.Contains(body.bodyIndex))
				{
					return SizeClass.Lesser;
				}
				if (GreaterBodyIndexes.Contains(body.bodyIndex))
				{
					return SizeClass.Greater;
				}
				if (ChampionBodyIndexes.Contains(body.bodyIndex))
				{
					return SizeClass.Champion;
				}
			}

			if (body.hullClassification == HullClassification.BeetleQueen)
			{
				return SizeClass.Champion;
			}
			if (body.hullClassification == HullClassification.Golem)
			{
				return SizeClass.Greater;
			}
			if (body.hullClassification == HullClassification.Human)
			{
				return SizeClass.Lesser;
			}

			return SizeClass.None;
		}



		private static void CameraDistanceHook()
		{
			IL.RoR2.CameraModes.CameraModePlayerBasic.UpdateInternal += (il) =>
			{
				ILCursor c = new ILCursor(il);

				const int directionIndex = 13;

				bool found = c.TryGotoNext(
					x => x.MatchStloc(directionIndex)// Vector3
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Ldloc, directionIndex);
					c.Emit(OpCodes.Ldloc, 1);
					c.EmitDelegate<Func<Vector3, CameraRigController, Vector3>>((direction, camRig) =>
					{
						CharacterBody body = camRig.targetBody;
						if (body)
						{
							return direction * body.currentScale;
						}

						return direction;
					});
					c.Emit(OpCodes.Stloc, directionIndex);
				}
				else
				{
					ZetSizeControllerPlugin.LogWarn("CameraDistanceHook Failed");
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
							float currentScale = body.currentScale;

							float halfHeight = Mathf.Abs(body.corePosition.y - body.footPosition.y);
							float originalHalfHeight = halfHeight / currentScale;
							float heightVerticalOffset = originalHalfHeight * (currentScale - 1f);

							return (vertical * currentScale) + heightVerticalOffset;
						}

						return vertical;
					});
				}
				else
				{
					ZetSizeControllerPlugin.LogWarn("CameraVerticalOffsetHook Failed");
				}
			};
		}
		/*
		private static void InputBankAimOriginPostfix(InputBankTest __instance, ref Vector3 __result)
		{
			CharacterBody body = __instance.characterBody;
			if (body)
			{
				float currentScale = body.currentScale;

				float halfHeight = Mathf.Abs(body.corePosition.y - body.footPosition.y);
				float originalHalfHeight = halfHeight / currentScale;
				float heightVerticalOffset = originalHalfHeight * (currentScale - 1f);

				__result = new Vector3(__result.x, __result.y + heightVerticalOffset, __result.z);
			}
		}
		*/


		private static void InteractionDriverHook()
		{
			IL.RoR2.InteractionDriver.FindBestInteractableObject += (il) =>
			{
				ILCursor c = new ILCursor(il);

				int rangeIndex = -1;

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchCallOrCallvirt<InteractionDriver>("get_interactor"),
					x => x.MatchLdfld<Interactor>("maxInteractionDistance"),
					x => x.MatchStloc(out rangeIndex)
				);

				if (found)
				{
					c.Index += 4;

					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, rangeIndex);
					c.EmitDelegate<Func<InteractionDriver, float, float>>((driver, range) =>
					{
						CharacterBody body = driver.characterBody;
						if (body)
						{
							range *= GetInteractionRangeMult(body);
						}

						return range;
					});
					c.Emit(OpCodes.Stloc, rangeIndex);
				}
				else
				{
					ZetSizeControllerPlugin.LogWarn("InteractionHook Failed");
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
						if (body)
						{
							float value = GetInteractionRangeMult(body);
							cutoff *= value * value;
						}

						return cutoff;
					});
				}
				else
				{
					ZetSizeControllerPlugin.LogWarn("PickupPickerHook Failed");
				}
			};
		}

		private static void TieredPickupPickerHook()
		{
			IL.RoR2.TieredPickupPickerController.FixedUpdateServer += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<TieredPickupPickerController>("cutoffDistance"),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<TieredPickupPickerController>("cutoffDistance"),
					x => x.MatchMul()
				);

				if (found)
				{
					c.Index += 5;

					c.Emit(OpCodes.Ldloc, 1);
					c.EmitDelegate<Func<float, CharacterBody, float>>((cutoff, body) =>
					{
						if (body)
						{
							float value = GetInteractionRangeMult(body);
							cutoff *= value * value;
						}

						return cutoff;
					});
				}
				else
				{
					ZetSizeControllerPlugin.LogWarn("TieredPickupPickerHook Failed");
				}
			};
		}

		private static void DroneScrapperPickerHook()
		{
			IL.RoR2.DroneScrapperPickerController.FixedUpdateServer += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<DroneScrapperPickerController>("cutoffDistance"),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<DroneScrapperPickerController>("cutoffDistance"),
					x => x.MatchMul()
				);

				if (found)
				{
					c.Index += 5;

					c.Emit(OpCodes.Ldloc, 1);
					c.EmitDelegate<Func<float, CharacterBody, float>>((cutoff, body) =>
					{
						if (body)
						{
							float value = GetInteractionRangeMult(body);
							cutoff *= value * value;
						}

						return cutoff;
					});
				}
				else
				{
					ZetSizeControllerPlugin.LogWarn("DroneScrapperPickerHook Failed");
				}
			};
		}



		private static void OverlapAttackPositionHook()
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
					ZetSizeControllerPlugin.LogWarn("OverlapAttackPositionHook Failed");
				}
			};
		}

		private static void OverlapAttackScaleHook()
		{
			IL.RoR2.OverlapAttack.Fire += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchStloc(5)
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
					ZetSizeControllerPlugin.LogWarn("OverlapAttackScaleHook Failed");
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
						return Mathf.Clamp(1f / body.currentScale, 0.125f, 4f);
					}

					return 1f;
				});
				c.Emit(OpCodes.Stloc, playbackSpeed);


				/*
				bool found = c.TryGotoNext(
					x => x.MatchLdsfld(typeof(AnimationParameters).GetField("mainRootPlaybackRate")),
					x => x.MatchLdloc(out _)
				);

				if (found)
				{
					c.Index += 2;

					c.Emit(OpCodes.Ldloc, playbackSpeed);
					c.Emit(OpCodes.Mul);
				}
				else
				{
					ZetSizeControllerPlugin.LogWarn("AnimationHook:rootMotion Failed");
				}
				*/


				bool found = c.TryGotoNext(
					x => x.MatchLdsfld(typeof(AnimationParameters).GetField("walkSpeed"))
				);

				if (!found)
				{
					ZetSizeControllerPlugin.LogWarn("AnimationHook:walkSpeed Failed");
					return;
				}

				found = c.TryGotoNext(
					x => x.MatchCallOrCallvirt<EntityState>("get_characterBody"),
					x => x.MatchCallOrCallvirt<CharacterBody>("get_moveSpeed")
				);

				if (found)
				{
					c.Index += 2;

					c.Emit(OpCodes.Ldloc, playbackSpeed);
					c.Emit(OpCodes.Mul);
				}
				else
				{
					ZetSizeControllerPlugin.LogWarn("AnimationHook:Body.moveSpeed Failed");
				}

				found = c.TryGotoNext(
					x => x.MatchCallOrCallvirt<EntityState>("get_characterMotor"),
					x => x.MatchCallOrCallvirt<CharacterMotor>("get_walkSpeed")
				);

				if (found)
				{
					c.Index += 2;

					c.Emit(OpCodes.Ldloc, playbackSpeed);
					c.Emit(OpCodes.Mul);
				}
				else
				{
					ZetSizeControllerPlugin.LogWarn("AnimationHook:Motor.walkSpeed Failed");
				}
			};
		}
	}
}
