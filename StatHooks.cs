using System;
using System.Reflection;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;

namespace TPDespair.ZetSizeController
{
	internal static class StatHooks
	{
		internal static void Init()
		{
			MovespeedHook();
			DamageHook();
			HealthHook();

			DirectStatHook();
		}



		private static void MovespeedHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				int BaseSpeedLocIndex = -1;

				if (c.TryGotoNext(
					x => x.MatchLdfld<CharacterBody>("baseMoveSpeed"),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<CharacterBody>("levelMoveSpeed")
				))
				{
					if (!c.TryGotoNext(x => x.MatchStloc(out BaseSpeedLocIndex)))
					{
						ZetSizeControllerPlugin.LogWarn("MovementSpeedHook Failed - Could not find BaseSpeedLocIndex");
						return;
					}
				}



				int MultSpeedLocIndex = -1;
				int DivSpeedLocIndex = -1;

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(BaseSpeedLocIndex),
					x => x.MatchLdloc(out MultSpeedLocIndex),
					x => x.MatchLdloc(out DivSpeedLocIndex),
					x => x.MatchDiv(),
					x => x.MatchMul(),
					x => x.MatchStloc(BaseSpeedLocIndex)
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Pop);

					// multiplier
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, BaseSpeedLocIndex);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						bool shrink = ZetShrinkifact.Enabled;
						bool titan = ZetTitanifact.Enabled;

						if (shrink || titan)
						{
							if (self.HasBuff(ZetSizeControllerContent.Buffs.ZetMonsterSizeClass))
							{
								if (shrink && Configuration.ShrinkifactMonster.Value)
								{
									value *= Configuration.ShrinkifactMovement.Value;
								}
								if (titan && Configuration.TitanifactMonster.Value)
								{
									value *= Configuration.TitanifactMovement.Value;
								}
							}
							else if (self.HasBuff(ZetSizeControllerContent.Buffs.ZetPlayerSizeClass))
							{
								if (shrink && Configuration.ShrinkifactPlayer.Value)
								{
									value *= Configuration.ShrinkifactMovement.Value;
								}
								if (titan && Configuration.TitanifactPlayer.Value)
								{
									value *= Configuration.TitanifactMovement.Value;
								}
							}
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, BaseSpeedLocIndex);

					c.Emit(OpCodes.Ldloc, BaseSpeedLocIndex);
				}
				else
				{
					ZetSizeControllerPlugin.LogWarn("MovementSpeedHook Failed");
				}
			};
		}

		private static void DamageHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				int BaseDamageLocIndex = -1;

				if (c.TryGotoNext(
					x => x.MatchLdfld<CharacterBody>("baseDamage"),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<CharacterBody>("levelDamage")
				))
				{
					if (!c.TryGotoNext(x => x.MatchStloc(out BaseDamageLocIndex)))
					{
						ZetSizeControllerPlugin.LogWarn("DamageHook Failed - Could not find BaseDamageLocIndex");
						return;
					}
				}



				int MultDamageLocIndex = -1;

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(BaseDamageLocIndex),
					x => x.MatchLdloc(out MultDamageLocIndex),
					x => x.MatchMul(),
					x => x.MatchStloc(BaseDamageLocIndex)
				);

				if (found)
				{
					c.Index += 4;

					// multiplier
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, BaseDamageLocIndex);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						bool shrink = ZetShrinkifact.Enabled;
						bool titan = ZetTitanifact.Enabled;

						if (shrink || titan)
						{
							if (self.HasBuff(ZetSizeControllerContent.Buffs.ZetMonsterSizeClass))
							{
								if (shrink && Configuration.ShrinkifactMonster.Value)
								{
									value *= Configuration.ShrinkifactDamage.Value;
								}
								if (titan && Configuration.TitanifactMonster.Value)
								{
									value *= Configuration.TitanifactDamage.Value;
								}
							}
							else if (self.HasBuff(ZetSizeControllerContent.Buffs.ZetPlayerSizeClass))
							{
								if (shrink && Configuration.ShrinkifactPlayer.Value)
								{
									value *= Configuration.ShrinkifactDamage.Value;
								}
								if (titan && Configuration.TitanifactPlayer.Value)
								{
									value *= Configuration.TitanifactDamage.Value;
								}
							}
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, BaseDamageLocIndex);
				}
				else
				{
					ZetSizeControllerPlugin.LogWarn("DamageHook Failed");
				}
			};
		}

		private static void HealthHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				int BaseHealthLocIndex = -1;

				if (c.TryGotoNext(
					x => x.MatchLdfld<CharacterBody>("baseMaxHealth"),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<CharacterBody>("levelMaxHealth")
				))
				{
					if (!c.TryGotoNext(x => x.MatchStloc(out BaseHealthLocIndex)))
					{
						ZetSizeControllerPlugin.LogWarn("HealthHook Failed - Could not find BaseHealthLocIndex");
						return;
					}
				}



				int MultHealthLocIndex = -1;

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(BaseHealthLocIndex),
					x => x.MatchLdloc(out MultHealthLocIndex),
					x => x.MatchMul(),
					x => x.MatchStloc(BaseHealthLocIndex)
				);

				if (found)
				{
					c.Index += 4;

					// multiplier
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, BaseHealthLocIndex);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						bool shrink = ZetShrinkifact.Enabled;
						bool titan = ZetTitanifact.Enabled;

						if (shrink || titan)
						{
							if (self.HasBuff(ZetSizeControllerContent.Buffs.ZetMonsterSizeClass))
							{
								if (shrink && Configuration.ShrinkifactMonster.Value)
								{
									value *= Configuration.ShrinkifactHealth.Value;
								}
								if (titan && Configuration.TitanifactMonster.Value)
								{
									value *= Configuration.TitanifactHealth.Value;
								}
							}
							else if (self.HasBuff(ZetSizeControllerContent.Buffs.ZetPlayerSizeClass))
							{
								if (shrink && Configuration.ShrinkifactPlayer.Value)
								{
									value *= Configuration.ShrinkifactHealth.Value;
								}
								if (titan && Configuration.TitanifactPlayer.Value)
								{
									value *= Configuration.TitanifactHealth.Value;
								}
							}
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, BaseHealthLocIndex);
				}
				else
				{
					ZetSizeControllerPlugin.LogWarn("HealthHook Failed");
				}
			};
		}



		private static void DirectStatHook()
		{
			On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
			{
				orig(self);

				if (self)
				{
					int count = self.GetBuffCount(ZetSizeControllerContent.Buffs.ZetSplitDefense);
					if (count > 0)
					{
						self.armor += 100f * ((1 << count) - 1);
					}
				}
			};
		}
	}
}
