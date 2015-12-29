﻿// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using System.Windows;
using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.Swtor;
using pCombat.Core;
using pCombat.Helpers;
using Targeting = pCombat.Core.Targeting;

namespace pCombat
{
	public class pCombat : CombatRoutine
	{
		public static bool IsHealer;
		private static readonly InventoryManagerItem MedPack = new InventoryManagerItem("Medpac", 90);
		private Composite _combat;
		private Composite _ooc;
		private Composite _pull;

		public static bool MovementDisabled
		{
			get { return BotMain.CurrentBot.Name == "Combat Bot"; }
		}

		public override string Name
		{
			get { return "pCombat"; }
		}

		public override Window ConfigWindow
		{
			get { return null; }
		}

		public override CharacterClass Class
		{
			get { return BuddyTor.Me.Class; }
		}

		public override Composite OutOfCombat
		{
			get { return _ooc; }
		}

		public override Composite Pull
		{
			get { return _pull; }
		}

		public override Composite Combat
		{
			get { return _combat; }
		}

		public override void Dispose()
		{
		}

		public override void Initialize()
		{
			Logger.Write("Level: " + BuddyTor.Me.Level);
			Logger.Write("Class: " + Class);
			Logger.Write("Advanced Class: " + BuddyTor.Me.AdvancedClass);
			Logger.Write("Discipline: " + BuddyTor.Me.Discipline);

			var f = new RotationFactory();
			var b = f.Build(BuddyTor.Me.Discipline.ToString());

			CombatHotkeys.Initialize();

			if (b == null)
				b = f.Build(BuddyTor.Me.CharacterClass.ToString());

			Logger.Write("Rotation Selected : " + b.Name);

			if (BuddyTor.Me.IsHealer())
			{
				IsHealer = true;
				Logger.Write("Healing Enabled");
			}

			_ooc = new Decorator(ret => !BuddyTor.Me.IsDead && !BuddyTor.Me.IsMounted && !CombatHotkeys.PauseRotation,
				new PrioritySelector(
					Spell.Buff(BuddyTor.Me.SelfBuffName()),
					b.Buffs,
					Rest.HandleRest
					));

			_combat = new Decorator(ret => !CombatHotkeys.PauseRotation,
				new LockSelector(
					Spell.WaitForCast(),
					MedPack.UseItem(ret => BuddyTor.Me.HealthPercent <= 30),
					Targeting.ScanTargets,
					b.Cooldowns,
					new Decorator(ret => CombatHotkeys.EnableAoe, b.AreaOfEffect),
					b.SingleTarget));

			_pull = new Decorator(ret => !CombatHotkeys.PauseRotation && !MovementDisabled || IsHealer,
				_combat
				);
		}
	}
}