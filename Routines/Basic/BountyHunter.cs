﻿// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using pCombat.Core;
using pCombat.Helpers;

namespace pCombat.Routines
{
	public class BountyHunter : RotationBase
	{
		public override string Name
		{
			get { return "Basic Bounty Hunter"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Hunter's Boon")
					);
			}
		}

		public override Composite Cooldowns
		{
			get { return new LockSelector(); }
		}

		public override Composite SingleTarget
		{
			get
			{
				return new LockSelector(
					CombatMovement.CloseDistance(Distance.Ranged),
					Spell.Cast("Explosive Dart"),
					Spell.CastOnGround("Death from Above", ret => Me.CurrentTarget.Distance > .5f),
					Spell.Cast("Rail Shot"),
					Spell.Cast("Vent Heat", ret => Me.ResourcePercent() >= 50),
					Spell.Cast("Rocket Punch", ret => Me.CurrentTarget.Distance <= .4f),
					Spell.Cast("Flame Thrower", ret => Me.CurrentTarget.Distance <= 1f),
					Spell.Cast("Flame Burst", ret => Me.ResourcePercent() <= 50),
					Spell.Cast("Rapid Shots")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new LockSelector());
			}
		}
	}
}