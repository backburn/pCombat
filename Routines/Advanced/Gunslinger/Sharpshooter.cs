﻿// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using pCombat.Core;
using pCombat.Helpers;

namespace pCombat.Routines
{
	internal class Sharpshooter : RotationBase
	{
		public override string Name
		{
			get { return "Gunslinger Sharpshooter"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Lucky Shots")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new LockSelector(
					Spell.Buff("Escape"),
					Spell.Buff("Burst Volley"),
					Spell.Buff("Defense Screen", ret => Me.HealthPercent <= 70),
					Spell.Buff("Dodge", ret => Me.HealthPercent <= 30),
					Spell.Buff("Cool Head", ret => Me.EnergyPercent <= 50),
					Spell.Buff("Smuggler's Luck"),
					Spell.Buff("Illegal Mods")
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new LockSelector(
					Spell.Cast("Flurry of Bolts", ret => Me.EnergyPercent < 60),

					//Movement
					CombatMovement.CloseDistance(Distance.Ranged),

					//Rotation
					Spell.Buff("Crouch", ret => !Me.IsInCover()),
					Spell.Cast("Sabotage Charge", ret => Me.HasBuff("Burst Volley")),
					Spell.Cast("Trickshot"),
					Spell.Cast("Penetrating Rounds", ret => Me.IsInCover()),
					Spell.DoT("Vital Shot", "Bleeding (Vital Shot)"),
					Spell.Cast("Aimed Shot", ret => Me.IsInCover() && Me.BuffCount("Charged Aim") == 2),
					Spell.Cast("Quickdraw", ret => Me.CurrentTarget.HealthPercent <= 30),
					Spell.Cast("Charged Burst", ret => Me.IsInCover())
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new LockSelector(
						Spell.CastOnGround("XS Freighter Flyby"),
						Spell.Cast("Thermal Grenade"),
						Spell.CastOnGround("Sweeping Gunfire")
						));
			}
		}
	}
}