﻿using System;
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
		}



		private static void MovespeedHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(out _),
					x => x.MatchCallOrCallvirt<CharacterBody>("set_moveSpeed")
				);

				if (found)
				{
					c.Index += 2;

					c.Emit(OpCodes.Ldarg, 0);// set_moveSpeed
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldarg, 0);// get_moveSpeed
					c.Emit(OpCodes.Call, typeof(CharacterBody).GetMethod("get_moveSpeed"));
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
					c.Emit(OpCodes.Call, typeof(CharacterBody).GetMethod("set_moveSpeed", BindingFlags.Instance | BindingFlags.NonPublic));
				}
				else
				{
					ZetSizeControllerPlugin.LogWarn("MovespeedHook Failed");
				}
			};
		}

		private static void DamageHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				const int baseValue = 88;
				const int multValue = 89;

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(baseValue),
					x => x.MatchLdloc(multValue),
					x => x.MatchMul(),
					x => x.MatchStloc(baseValue)
				);

				if (found)
				{
					c.Index += 4;

					// multiplier
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, baseValue);
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
					c.Emit(OpCodes.Stloc, baseValue);
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

				const int baseValue = 70;
				const int multValue = 71;

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(baseValue),
					x => x.MatchLdloc(multValue),
					x => x.MatchMul(),
					x => x.MatchStloc(baseValue)
				);

				if (found)
				{
					c.Index += 4;

					// multiplier
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, baseValue);
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
					c.Emit(OpCodes.Stloc, baseValue);
				}
				else
				{
					ZetSizeControllerPlugin.LogWarn("HealthHook Failed");
				}
			};
		}
	}
}
