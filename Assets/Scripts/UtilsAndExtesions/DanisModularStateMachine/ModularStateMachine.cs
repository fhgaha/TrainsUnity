using System;
using UnityEngine;

namespace Trains
{
	public class ModularStateMachine 
	{
		private State currentState;

		public void Initialize(State initialState)
		{
			currentState = initialState;	
		}

		public void Handle() 
		{
			if (currentState == null) return;

			var nextState = currentState.Handle();
			if (nextState != null)
			{
				SetState(nextState);
			}	
		}

		private void SetState(State nextState)
		{
			currentState.OnExit();
			currentState = nextState;
			currentState.OnEnter();
		}
	}
}