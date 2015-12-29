﻿// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using pCombat.Core;
using pCombat.Helpers;

namespace pCombat.Routines
{
    internal class Immortal : RotationBase
    {
        public override string Name
        {
            get { return "Juggernaut Immortal"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Soresu Form"),
                    Spell.Buff("Unnatural Might"),
                    Spell.Cast("Guard", on => Me.Companion,
                        ret => Me.Companion != null && !Me.Companion.IsDead && !Me.Companion.HasBuff("Guard"))
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new LockSelector(
                    Spell.Buff("Unleash"),
                    Spell.Buff("Saber Reflect", ret => Me.HealthPercent <= 90),
                    Spell.Buff("Endure Pain", ret => Me.HealthPercent <= 80),
                    Spell.Buff("Enraged Defense", ret => Me.HealthPercent < 70),
                    Spell.Buff("Invincible", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Enrage", ret => Me.ActionPoints <= 6)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new LockSelector(
                    Spell.Cast("Saber Throw",
                        ret => !pCombat.MovementDisabled && Me.CurrentTarget.Distance >= 0.5f && Me.CurrentTarget.Distance <= 3f),
                    Spell.Cast("Force Charge",
                        ret => !pCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    //Rotation
                    Spell.Cast("Retaliation"),
                    Spell.Cast("Crushing Blow"),
                    Spell.Cast("Force Scream"),
                    Spell.Cast("Aegis Assault", ret => Me.ActionPoints <= 7 || !Me.HasBuff("Aegis")),
                    Spell.Cast("Smash", ret => !Me.CurrentTarget.HasDebuff("Unsteady (Force)") && Targeting.ShouldPbaoe),
                    Spell.Cast("Backhand", ret => !Me.CurrentTarget.IsStunned),
                    Spell.Cast("Ravage"),
                    Spell.Cast("Vicious Throw", ret => Me.CurrentTarget.HealthPercent <= 30 || Me.HasBuff("War Bringer")),
                    Spell.Cast("Vicious Slash", ret => Me.ActionPoints >= 9),
                    Spell.Cast("Assault"),
                    Spell.Cast("Saber Throw", ret => Me.CurrentTarget.Distance >= 0.5f && Me.InCombat)
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldPbaoe,
                    new LockSelector(
                        Spell.Cast("Smash"),
                        Spell.Cast("Crushing Blow", ret => Me.HasBuff("Aegis")),
                        Spell.Cast("Aegis Assault", ret => !Me.HasBuff("Aegis")),
                        Spell.Cast("Retaliation"),
                        Spell.Cast("Force Scream"),
                        Spell.Cast("Sweeping Slash")
                        ));
            }
        }
    }
}