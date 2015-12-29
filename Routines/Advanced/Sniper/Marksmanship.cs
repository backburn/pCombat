// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using pCombat.Core;
using pCombat.Helpers;

namespace pCombat.Routines
{
	internal class Marksmanship : RotationBase
	{
		public override string Name
		{
			get { return "Sniper Marksmanship"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Coordination")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new LockSelector(
					Spell.Buff("Escape", ret => Me.IsStunned),
					Spell.Buff("Shield Probe", ret => Me.HealthPercent <= 70),
					Spell.Buff("Evasion", ret => Me.HealthPercent <= 30),
					Spell.Buff("Adrenaline Probe", ret => Me.EnergyPercent < 50),
					Spell.Buff("Sniper Volley", ret => Me.EnergyPercent <= 60),
					Spell.Buff("Entrench", ret => Me.CurrentTarget.StrongOrGreater() && Me.IsInCover()),
					Spell.Buff("Laze Target"),
					Spell.Buff("Target Acquired")
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

					//Low Energy
					new Decorator(ret => Me.EnergyPercent < 60,
						new LockSelector(
							Spell.Cast("Rifle Shot")
							)),

					//Rotation
					Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting),
                    //1. “Laze Target + Ambush > Followthrough”
                    Spell.Cast("Followthrough"),
                    Spell.Cast("Ambush"),
                    //2. Suppressive Fire on 3 + targets > Corrosive Dart(The target does not the “Marked Target” debuff) > Ambush
                    Spell.DoT("Corrosive Dart", "Corrosive Dart"),
                    Spell.Cast("Ambush", ret => Me.BuffCount("Zeroing Shots") == 2),
                    //3. “Penetrating Blasts > Followthrough”
                    Spell.Cast("Penetrating Blasts", ret => Me.Level >= 26),
                    Spell.Cast("Series of Shots", ret => Me.Level < 26),
                    //4. “Snipe > Snipe > Followthrough(20 % Damage Increased)”
                    Spell.Cast("Snipe"),
                    //5. Takedown
                    Spell.Cast("Takedown", ret => Me.CurrentTarget.HealthPercent <= 30)
                    //6. Corrosive Dart(The target does have the “Marked Target” debuff)
                    //7. Followthrough
                    //8. Snipe > Corrosive Dart(Dart should only be applied to targets that don’t have the “Marked Target” debuff or targets that have over 200k HP for burst scenarios. It is generally better to precast this and just hope you get an 8k + Snipe which with 3 stacks of Honed Shots is like 2 / 3 chance.
                    //9. Rifle Shot
                   )
                    ;
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new LockSelector(
						Spell.CastOnGround("Orbital Strike"),
						Spell.Cast("Fragmentation Grenade"),
						Spell.CastOnGround("Suppressive Fire")
						));
			}
		}
	}
}