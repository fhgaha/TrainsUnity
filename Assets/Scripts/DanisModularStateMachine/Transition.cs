using System;

namespace Trains
{
	public class Transition
	{
		public Func<bool> Condition { get; }
		public State To { get; }

		public Transition(Func<bool> condition, State to)
		{
			Condition = condition;
			To = to;
		}

	}
}