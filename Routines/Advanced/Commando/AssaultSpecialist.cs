﻿// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using pCombat.Core;
using pCombat.Helpers;

namespace pCombat.Routines
{
	internal class AssaultSpecialist : RotationBase
	{
		public override string Name
		{
			get { return "Commando Assault Specialist"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Plasma Cell"),
					Spell.Buff("Fortification")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new LockSelector(
					Spell.Buff("Tenacity", ret => Me.IsStunned),
					Spell.Buff("Recharge Cells", ret => Me.ResourceStat <= 40),
					Spell.Buff("Reactive Shield", ret => Me.HealthPercent <= 70),
					Spell.Buff("Adrenaline Rush", ret => Me.HealthPercent <= 30),
					Spell.Buff("Supercharged Cell", ret => Me.BuffCount("Supercharge") == 10),
					Spell.Buff("Reserve Powercell", ret => Me.ResourceStat <= 60)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new LockSelector(
					new Decorator(ret => Me.ResourcePercent() < 60,
						new LockSelector(
							Spell.Cast("Mag Bolt", ret => Me.HasBuff("Ionic Accelerator") && Me.Level >= 57),
							Spell.Cast("High Impact Bolt", ret => Me.HasBuff("Ionic Accelerator") && Me.Level < 57),
							Spell.Cast("Hammer Shot")
							)),

					//Movement
					CombatMovement.CloseDistance(Distance.Ranged),

					//Rotation
					Spell.Cast("Disabling Shot", ret => Me.CurrentTarget.IsCasting && !pCombat.MovementDisabled),
					Spell.Cast("Mag Bolt", ret => Me.HasBuff("Ionic Accelerator") && Me.Level >= 57),
					Spell.Cast("High Impact Bolt", ret => Me.HasBuff("Ionic Accelerator") && Me.Level < 57),
					Spell.Cast("Explosive Round", ret => Me.HasBuff("Hyper Assault Rounds")),
					Spell.Cast("Assault Plastique"),
					Spell.DoT("Serrated Bolt", "Bleeding"),
					Spell.DoT("Incendiary Round", "Burning (Incendiary Round)"),
					Spell.Cast("Electro Net"),
					Spell.Cast("Full Auto"),
					Spell.Cast("Mag Bolt", ret => Me.Level >= 57),
					Spell.Cast("High Impact Bolt", ret => Me.Level < 57),
					Spell.Cast("Charged Bolts")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new LockSelector(
						Spell.DoT("Serrated Bolt", "Bleeding"),
						Spell.DoT("Incendiary Round", "Burning (Incendiary Round)"),
						Spell.Cast("Plasma Grenade", ret => Me.CurrentTarget.HasDebuff("Burning (Incendiary Round)")),
						Spell.Cast("Sticky Grenade", ret => Me.CurrentTarget.HasDebuff("Bleeding")),
						Spell.Cast("Explosive Round", ret => Me.HasBuff("Hyper Assault Rounds")),
						Spell.CastOnGround("Hail of Bolts", ret => Me.ResourcePercent() >= 90)
						));
			}
		}
	}
}