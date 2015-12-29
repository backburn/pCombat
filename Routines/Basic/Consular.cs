﻿// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using pCombat.Core;
using pCombat.Helpers;

namespace pCombat.Routines
{
	public class Consular : RotationBase
	{
		public override string Name
		{
			get { return "Basic Consular"; }
		}

		public override Composite Buffs
		{
			get { return new PrioritySelector(); }
		}

		public override Composite Cooldowns
		{
			get { return new PrioritySelector(); }
		}

		public override Composite SingleTarget
		{
			get
			{
				return new LockSelector(
					CombatMovement.CloseDistance(Distance.Melee),
					Spell.Cast("Telekinetic Throw"),
					Spell.Cast("Project", ret => Me.Force > 75),
					Spell.Cast("Double Strike", ret => Me.Force > 70),
					Spell.Cast("Saber Strike")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new LockSelector(
						Spell.Cast("Force Wave", ret => Me.CurrentTarget.Distance <= Distance.MeleeAoE))
					);
			}
		}
	}
}