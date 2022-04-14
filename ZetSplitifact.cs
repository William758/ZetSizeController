using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace TPDespair.ZetSizeController
{
	public static class ZetSplitifact
	{
		private static readonly List<string> SplitableBodyNames = new List<string> { "BeetleBody", "BeetleCrystalBody", "ClayBody", "FlyingVerminBody", "HermitCrabBody", "ImpBody", "JellyfishBody", "LemurianBody", "LunarExploderBody", "MiniMushroomBody", "MinorConstructBody", "MoffeinClayManBody", "VerminBody", "VultureBody", "WispBody", "AncientWispBody", "BeetleGuardBody", "BeetleGuardCrystalBody", "BellBody", "BisonBody", "BomberBody", "ClayBruiserBody", "ClayGrenadierBody", "GolemBody", "GreaterWispBody", "LemurianBruiserBody", "LunarGolemBody", "LunarKnightBody", "LunarWispBody", "MajorConstructBody", "NullifierBody", "ParentBody", "RoboBallMiniBody", "VoidJailerBody", "ArchWispBody", "BeetleQueen2Body", "ClayBossBody", "DireseekerBody", "ElectricWormBody", "GrandParentBody", "GravekeeperBody", "ImpBossBody", "MagmaWormBody", "MegaConstructBody", "MoffeinAncientWispBody", "RoboBallBossBody", "SuperRoboBallBossBody", "TitanBody", "VagrantBody", "VoidMegaCrabBody" };
		private static readonly List<BodyIndex> SplitableBodyIndexes = new List<BodyIndex>();

		private static readonly List<string> SmallFlyingBodyNames = new List<string> { "FlyingVerminBody", "VultureBody", "WispBody", "RoboBallMiniBody" };
		private static readonly List<BodyIndex> SmallFlyingBodyIndexes = new List<BodyIndex>();

		private static readonly List<NetworkInstanceId> SplitBodies = new List<NetworkInstanceId>();

		// count : 0 is uninitialized , 1 is 0 remaining
		internal static ItemDef TrackerItemDef;



		private static int state = 0;

		public static bool Enabled
		{
			get
			{
				if (state < 1) return false;
				else if (state > 1) return true;
				else
				{
					if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(ZetSizeControllerContent.Artifacts.ZetSplitifact)) return true;

					return false;
				}
			}
		}



		private static void EnableEffects()
		{
			On.RoR2.CharacterAI.BaseAI.OnBodyDeath += SplitOnBodyDeath;
			SizeManager.onSizeDataCreated += RollSplitCount;
		}

		private static void DisableEffects()
		{
			On.RoR2.CharacterAI.BaseAI.OnBodyDeath -= SplitOnBodyDeath;
			SizeManager.onSizeDataCreated -= RollSplitCount;
		}



		private static void OnArtifactEnabled(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
		{
			if (artifactDef == ZetSizeControllerContent.Artifacts.ZetSplitifact)
			{
				EnableEffects();
			}
		}

		private static void OnArtifactDisabled(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
		{
			if (artifactDef == ZetSizeControllerContent.Artifacts.ZetSplitifact)
			{
				DisableEffects();
			}
		}



		internal static void Init()
		{
			state = Configuration.ShrinkifactEnable.Value;
			if (state < 1) return;

			ZetSizeControllerPlugin.RegisterLanguageToken("ARTIFACT_ZETSPLITIFACT_NAME", "Artifact of Fragmentation");
			ZetSizeControllerPlugin.RegisterLanguageToken("ARTIFACT_ZETSPLITIFACT_DESC", "Monsters can appear in larger sizes and split into smaller versions of themselves on death.");

			BodyCatalog.availability.CallWhenAvailable(PopulateBodyIndexes);

			if (state == 1)
			{
				RunArtifactManager.onArtifactEnabledGlobal += OnArtifactEnabled;
				RunArtifactManager.onArtifactDisabledGlobal += OnArtifactDisabled;
			}
			else
			{
				EnableEffects();
			}
		}



		private static void PopulateBodyIndexes()
		{
			BodyIndex bodyIndex;

			foreach (string splitName in SplitableBodyNames)
			{
				bodyIndex = BodyCatalog.FindBodyIndex(splitName);
				if (bodyIndex != BodyIndex.None)
				{
					if (!SplitableBodyIndexes.Contains(bodyIndex))
					{
						SplitableBodyIndexes.Add(bodyIndex);
					}
				}
			}

			foreach (string flyName in SmallFlyingBodyNames)
			{
				bodyIndex = BodyCatalog.FindBodyIndex(flyName);
				if (bodyIndex != BodyIndex.None)
				{
					if (!SmallFlyingBodyIndexes.Contains(bodyIndex))
					{
						SmallFlyingBodyIndexes.Add(bodyIndex);
					}
				}
			}

			Debug.LogWarning("ZetSplitifact - PopulateBodyIndexes : " + SplitableBodyIndexes.Count);
		}



		private static void SplitOnBodyDeath(On.RoR2.CharacterAI.BaseAI.orig_OnBodyDeath orig, RoR2.CharacterAI.BaseAI self, CharacterBody body)
		{
			if (NetworkServer.active && Enabled)
			{
				if (IsValidBodyForSpliting(body) && DeathAllowsSplitting(body))
				{
					SplitBody(body, 2);
				}
			}

			orig(self, body);
		}

		private static bool IsValidBodyForSpliting(CharacterBody body)
		{
			CharacterMaster master = body.master;
			if (master)
			{
				Inventory inventory = master.inventory;
				if (inventory)
				{
					int count = inventory.GetItemCount(TrackerItemDef);
					if (count > 1)
					{
						if (master.IsDeadAndOutOfLivesServer())
						{
							TeamIndex teamIndex = master.teamIndex;

							if (teamIndex == TeamIndex.Monster || teamIndex == TeamIndex.Lunar) return true;
						}
					}
				}
			}

			return false;
		}

		private static bool DeathAllowsSplitting(CharacterBody body)
		{
			HealthComponent healthComponent = body.healthComponent;
			if (healthComponent)
			{
				if ((body.healthComponent.killingDamageType & DamageType.OutOfBounds) != DamageType.Generic) return false;
				if ((body.healthComponent.killingDamageType & DamageType.VoidDeath) != DamageType.Generic) return false;

				return true;
			}

			return false;
		}

		private static void SplitBody(CharacterBody body, int count)
		{
			GameObject masterPrefab = MasterCatalog.FindMasterPrefab(body.master.name.Replace("(Clone)", ""));
			if (masterPrefab)
			{
				Vector3 position = body.transform.position;
				float y = Quaternion.LookRotation(body.inputBank.aimDirection).eulerAngles.y;

				float bodyHorizontalRadius = GetBodyHorizontalRadius(body.master.bodyPrefab);
				float spawnRadius = (count > 1) ? bodyHorizontalRadius / Mathf.Sin(3.14159274f / count) : 0f;
				spawnRadius = Mathf.Max(spawnRadius, body.radius * 0.5f);

				Inventory inventory = body.inventory;
				if (inventory)
				{
					int splitCount = inventory.GetItemCount(TrackerItemDef);
					if (splitCount > 1)
					{
						inventory.RemoveItem(TrackerItemDef);
					}
				}
				else
				{
					Debug.LogWarning("ZetSplitifact - Could not find inventory for body while splitting!");
				}

				// should have been cleared in RollSplitCount
				if (SplitBodies.Contains(body.netId))
				{
					SplitBodies.Remove(body.netId);
				}

				MasterSummon summon = new MasterSummon
				{
					masterPrefab = masterPrefab,
					summonerBodyObject = body.gameObject,
					inventoryToCopy = body.inventory ? body.inventory : null,
					ignoreTeamMemberLimit = false,
					useAmbientLevel = true,
				};

				foreach (float sliceFraction in new DegreeSlices(count, 0.5f))
				{
					Quaternion rotation = Quaternion.Euler(0f, y + sliceFraction + 180f, 0f);
					Vector3 vector = rotation * Vector3.forward;
					float distance = spawnRadius;

					RaycastHit raycastHit;
					if (Physics.Raycast(new Ray(position, vector), out raycastHit, spawnRadius + bodyHorizontalRadius, LayerIndex.world.intVal, QueryTriggerInteraction.Ignore))
					{
						distance = raycastHit.distance - bodyHorizontalRadius;
					}

					Vector3 summonPosition = position + vector * distance;

					summon.position = summonPosition;
					summon.rotation = rotation;

					CharacterMaster spawnMaster = summon.Perform();
					if (spawnMaster)
					{
						CharacterBody spawnBody = spawnMaster.GetBody();
						if (spawnBody)
						{
							if (!SplitBodies.Contains(spawnBody.netId))
							{
								SplitBodies.Add(spawnBody.netId);
							}
							else
							{
								Debug.LogWarning("ZetSplitifact - SplitBodies already contains netId : " + spawnBody.netId);
							}

							spawnBody.AddTimedBuff(RoR2Content.Buffs.Immune, 1f);

							float yForce = 0f;

							CharacterMotor charMotor = body.characterMotor;
							if (charMotor)
							{
								yForce = charMotor.isGrounded ? 10f : 5f;
							}

							Vector3 additionalVelocity = rotation * new Vector3(0f, yForce, 10f);
							AddBodyVelocity(spawnBody, additionalVelocity);
							spawnMaster.money = (uint)Mathf.CeilToInt(body.master.money * 0.65f);
						}
					}
				}
			}
		}

		private static float GetBodyHorizontalRadius(GameObject bodyPrefab)
		{
			Collider collider = bodyPrefab.GetComponent<Collider>();
			if (collider)
			{
				Vector3 position = bodyPrefab.transform.position;
				Bounds bounds = collider.bounds;
				Vector3 min = bounds.min;
				Vector3 max = bounds.max;
				return Mathf.Max(Mathf.Max(Mathf.Max(Mathf.Max(0f, position.x - min.x), position.z - min.z), max.x - position.x), max.z - position.z);
			}

			return 0f;
		}

		private static void AddBodyVelocity(CharacterBody body, Vector3 additionalVelocity)
		{
			IPhysMotor physMotor = body.GetComponent<IPhysMotor>();
			if (physMotor != null)
			{
				PhysForceInfo physForceInfo = PhysForceInfo.Create();
				physForceInfo.force = additionalVelocity;
				physForceInfo.massIsOne = true;
				physForceInfo.ignoreGroundStick = true;
				physForceInfo.disableAirControlUntilCollision = false;

				physMotor.ApplyForceImpulse(physForceInfo);
			}
		}



		private static void RollSplitCount(CharacterBody body, SizeData sizeData)
		{
			Inventory inventory = body.inventory;
			if (inventory)
			{
				bool isSplitBody = SplitBodies.Contains(body.netId);
				int count = inventory.GetItemCount(TrackerItemDef);

				if (!isSplitBody)
				{
					if (count > 1)
					{
						inventory.RemoveItem(TrackerItemDef, count - 1);
					}
					else if (count == 0 && IsBodyEligableForSpliting(body))
					{
						int splitCount = 0;

						switch (sizeData.sizeClass)
						{
							case SizeClass.Lesser:
								splitCount = Configuration.SplitifactMaxLesser.Value;
								break;
							case SizeClass.Greater:
								splitCount = Configuration.SplitifactMaxGreater.Value;
								break;
							case SizeClass.Champion:
								splitCount = Configuration.SplitifactMaxChampion.Value;
								break;
						}

						if (splitCount > 0)
						{
							if (SmallFlyingBodyIndexes.Contains(body.bodyIndex))
							{
								splitCount = Mathf.Min(splitCount, Configuration.SplitifactMaxSmallFly.Value);
							}
						}

						if (splitCount > 0)
						{
							splitCount = Random.Range(0, splitCount + 1);
							inventory.GiveItem(TrackerItemDef, splitCount + 1);
						}
					}
				}

				if (isSplitBody)
				{
					SplitBodies.Remove(body.netId);
				}
			}
		}

		private static bool IsBodyEligableForSpliting(CharacterBody body)
		{
			CharacterMaster master = body.master;
			if (master)
			{
				TeamIndex teamIndex = master.teamIndex;

				if (teamIndex == TeamIndex.Monster || teamIndex == TeamIndex.Lunar)
				{
					if (SplitableBodyIndexes.Contains(body.bodyIndex)) return true;
				}
			}

			return false;
		}
	}
}