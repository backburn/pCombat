﻿// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using pCombat.Core;
using pCombat.Helpers;

namespace pCombat.Routines
{
	internal class Annihilation : RotationBase
	{
		public override string Name
		{
			get { return "Marauder Annihilation"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Juyo Form"),
					Spell.Buff("Unnatural Might")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new LockSelector(
					Spell.Buff("Cloak of Pain", ret => Me.HealthPercent <= 90),
					Spell.Buff("Undying Rage", ret => Me.HealthPercent <= 20),
					Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 50),
					Spell.Buff("Deadly Saber", ret => !Me.HasBuff("Deadly Saber")),
					Spell.Buff("Frenzy", ret => Me.BuffCount("Fury") < 5),
					Spell.Buff("Berserk", ret => Me.CurrentTarget.DebuffCount("Bleeding (Deadly Saber)") == 3)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new LockSelector(
					Spell.Cast("Dual Saber Throw",
						ret => !pCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

                    Spell.Cast("Force Charge", 
                        ret => !pCombat.MovementDisabled && Me.CurrentTarget.Distance >= Distance.Melee && Me.CurrentTarget.Distance <= 3f),

					//Movement
					CombatMovement.CloseDistance(Distance.Melee),

					//Rotation
					Spell.Cast("Disruption", ret => Me.CurrentTarget.IsCasting && !pCombat.MovementDisabled),
                    Spell.DoT("Force Rend", "Force Rend"),
					Spell.DoT("Rupture", "Bleeding (Rupture)"),
                    Spell.Cast("Dual Saber Throw", ret => Me.HasBuff("Pulverize")),
					Spell.Cast("Annihilate"),
					Spell.Cast("Vicious Throw", ret => Me.CurrentTarget.HealthPercent <= 30),
					Spell.Cast("Ravage"),
					Spell.Cast("Vicious Slash", ret => Me.ActionPoints >= 9),
					Spell.Cast("Battering Assault", ret => Me.ActionPoints <= 6),
					Spell.Cast("Force Charge", ret => Me.ActionPoints <= 8),
					Spell.Cast("Assault", ret => Me.ActionPoints < 9)
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldPbaoe,
					new LockSelector(
						Spell.Cast("Dual Saber Throw",
							ret => !pCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),
						Spell.Cast("Smash", ret => Me.CurrentTarget.HasDebuff("Bleeding (Rupture)") && Me.CurrentTarget.HasDebuff("Force Rend")),
                        Spell.DoT("Force Rend", "Force Rend"),
                        Spell.DoT("Rupture", "Bleeding (Rupture)"),
                        Spell.Cast("Sweeping Slash")
						));
			}
		}
	}
}