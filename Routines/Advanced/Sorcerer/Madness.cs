﻿// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using pCombat.Core;
using pCombat.Helpers;

namespace pCombat.Routines
{
	internal class Madness : RotationBase
	{
		public override string Name
		{
			get { return "Sorcerer Madness"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Mark of Power")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new LockSelector(
					Spell.Buff("Force Barrier", ret => Me.HealthPercent <= 20),
					Spell.Buff("Unnatural Preservation", ret => Me.HealthPercent <= 70),
					Spell.Buff("Cloud Mind"),
					Spell.Buff("Polarity Shift"),
					Spell.Buff("Recklessness")
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new LockSelector(
					//Movement
					CombatMovement.CloseDistance(Distance.Ranged),

					//Rotation
					Spell.Cast("Jolt", ret => Me.CurrentTarget.IsCasting && !pCombat.MovementDisabled),
					Spell.Cast("Electrocute", ret => Me.CurrentTarget.IsCasting && !pCombat.MovementDisabled),
					Spell.CastOnGround("Death Field"),
					Spell.Cast("Crushing Darkness", ret => Me.HasBuff("Wrath")),
					Spell.Cast("Affliction",
						ret =>
							!Me.CurrentTarget.HasDebuff("Affliction (Force)") || Me.CurrentTarget.DebuffTimeLeft("Affliction (Force)") <= 3),
					Spell.DoT("Creeping Terror", "", 15000),
					Spell.Cast("Creeping Terror"),
					Spell.Cast("Force Lightning"),
					Spell.Cast("Shock"));
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new LockSelector(
						Spell.CastOnGround("Death Field"),
						Spell.DoT("Affliction", "Affliction (Force)"),
						Spell.CastOnGround("Force Storm")
						));
			}
		}
	}
}