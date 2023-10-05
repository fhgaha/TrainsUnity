using System;

namespace Trains
{
    public class State
    {
        private Transition[] transitions;
        private Action[] behaviours;
        private Action[] OnEnterBehaviours;
        private Action[] OnExitBehaviours;

        public void AddTransitions(params Transition[] transitions) => this.transitions = transitions;
        public void AddBehaviours(params Action[] behaviours) => this.behaviours = behaviours;
        public void AddEnterBehaviours(params Action[] behaviours) => OnEnterBehaviours = behaviours;
        public void AddExitBehaviours(params Action[] behaviours) => OnExitBehaviours = behaviours;

        public void OnEnter()
        {
            foreach (var enterBehaviour in OnEnterBehaviours)
            {
                enterBehaviour();
            }
        }

        public void OnExit()
        {
            foreach (var exitBehaviour in OnExitBehaviours)
            {
                exitBehaviour();
            }
        }

        public State Handle()
        {
            foreach (var transition in transitions)
            {
                if (transition.Condition())
                {
                    return transition.To;
                }
            }
            foreach (var behaviour in behaviours)
            {
                behaviour();
            }
            return null;
        }
    }
}