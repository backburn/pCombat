﻿// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using pCombat.Core;
using pCombat.Helpers;

namespace pCombat.Routines
{
	internal class Serenity : RotationBase
	{
		public override string Name
		{
			get { return "Shadow Serenity"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Force Technique"),
					Spell.Buff("Force Valor"),
					Spell.Cast("Guard", on => Me.Companion,
						ret => Me.Companion != null && !Me.Companion.IsDead && !Me.Companion.HasBuff("Guard")),
					Spell.Buff("Stealth", ret => !Rest.KeepResting() && !pCombat.MovementDisabled)
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new LockSelector(
					Spell.Buff("Force of Will"),
					Spell.Buff("Battle Readiness", ret => Me.HealthPercent <= 85),
					Spell.Buff("Deflection", ret => Me.HealthPercent <= 60),
					Spell.Buff("Resilience", ret => Me.HealthPercent <= 50),
					Spell.Buff("Force Potency"),
					Spell.Buff("Blackout", ret => Me.ForcePercent <= 40)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new LockSelector(
					Spell.Buff("Force Speed",
						ret => !pCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

					//Movement
					CombatMovement.CloseDistance(Distance.Melee),

					//Low Energy
					Spell.Cast("Mind Crush", ret => Me.ForcePercent < 25 && Me.HasBuff("Force Strike")),
					Spell.Cast("Saber Strike", ret => Me.ForcePercent < 25),

					//Rotation
					Spell.Cast("Mind Snap", ret => Me.CurrentTarget.IsCasting && !pCombat.MovementDisabled),
					Spell.CastOnGround("Force in Balance"),
					Spell.Cast("Sever Force", ret => !Me.CurrentTarget.HasDebuff("Sever Force")),
					Spell.DoT("Force Breach", "Crushed (Force Breach)"),
					Spell.Cast("Vanquish", ret => Me.HasBuff("Force Strike") && Me.Level >= 57),
					Spell.Cast("Mind Crush", ret => Me.HasBuff("Force Strike") && Me.Level < 57),
					Spell.Cast("Spinning Strike", ret => Me.CurrentTarget.HealthPercent <= 30 || Me.HasBuff("Crush Spirit")),
					Spell.Cast("Serenity Strike", ret => Me.HealthPercent <= 70),
					Spell.Cast("Double Strike"),
					Spell.Buff("Force Speed", ret => Me.CurrentTarget.Distance >= 1.1f && Me.IsMoving && Me.InCombat)
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldPbaoe,
					new LockSelector(
						Spell.DoT("Force Breach", "Crushed (Force Breach)"),
						Spell.Cast("Sever Force", ret => !Me.CurrentTarget.HasDebuff("Sever Force")),
						Spell.CastOnGround("Force in Balance"),
						Spell.Cast("Whirling Blow", ret => Me.ForcePercent > 70)
						));
			}
		}
	}
}