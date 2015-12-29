﻿// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using pCombat.Core;
using pCombat.Helpers;

namespace pCombat.Routines
{
	public class Tactics : RotationBase
	{
		public override string Name
		{
			get { return "Vanguard Tactics"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("High Energy Cell"),
					Spell.Buff("Fortification")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new LockSelector(
					Spell.Buff("Tenacity"),
					Spell.Buff("Recharge Cells", ret => Me.ResourcePercent() <= 50),
					Spell.Buff("Reactive Shield", ret => Me.HealthPercent <= 60),
					Spell.Buff("Adrenaline Rush", ret => Me.HealthPercent <= 30),
					Spell.Buff("Reserve Powercell", ret => Me.ResourceStat <= 80),
					Spell.Buff("Battle Focus")
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new LockSelector(
					//Movement
					Spell.Cast("Storm", ret => Me.CurrentTarget.Distance >= 1f && !pCombat.MovementDisabled),
					CombatMovement.CloseDistance(Distance.Melee),
					new Decorator(ret => Me.ResourcePercent() > 40,
						new LockSelector(
							Spell.Cast("High Impact Bolt", ret => Me.HasBuff("Tactical Accelerator")),
							Spell.Cast("Hammer Shot")
							)),
					Spell.Cast("Riot Strike", ret => Me.CurrentTarget.IsCasting && !pCombat.MovementDisabled),
					Spell.Cast("Cell Burst", ret => Me.BuffCount("Energy Lode") == 4),
					Spell.Cast("High Impact Bolt",
						ret => Me.CurrentTarget.HasDebuff("Bleeding (Gut)") && Me.HasBuff("Tactical Accelerator")),
					Spell.DoT("Gut", "Bleeding (Gut)"),
					Spell.Cast("Assault Plastique"),
					Spell.Cast("Stock Strike"),
					Spell.Cast("Tactical Surge", ret => Me.Level >= 26),
					Spell.Cast("Ion Pulse", ret => Me.Level < 26)
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new LockSelector(
					new Decorator(ret => Targeting.ShouldAoe,
						new LockSelector(
							Spell.CastOnGround("Morter Volley"),
							Spell.Cast("Sticky Grenade", ret => Me.CurrentTarget.HasDebuff("Bleeding (Retractable Blade)"))
							)),
					new Decorator(ret => Targeting.ShouldPbaoe,
						new LockSelector(
							Spell.Cast("Pulse Cannon"),
							Spell.Cast("Explosive Surge"))
						));
			}
		}
	}
}