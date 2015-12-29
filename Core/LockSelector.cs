﻿// Copyright (C) 2011-2015 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using Buddy.Swtor;

namespace pCombat.Core
{
	public class LockSelector : PrioritySelector
	{
		public LockSelector(params Composite[] children)
			: base(children)
		{
		}

		public override RunStatus Tick(object context)
		{
			using (BuddyTor.Memory.AcquireFrame())
			{
				return base.Tick(context);
			}
		}
	}
}