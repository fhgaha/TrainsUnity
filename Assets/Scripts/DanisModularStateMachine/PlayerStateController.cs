using System;
using UnityEngine;

namespace Trains
{
    class PlayerStateController : MonoBehaviour
    {
        ModularStateMachine stateMachine;
        Movement movement;
        Animator animator;

        private void Awake()
        {
            movement = GetComponent<Movement>();
            animator = GetComponent<Animator>();
            SetUpStates();
        }

        private void SetUpStates()
        {
            State idleState = new();
            State walkState = new();
            State runState = new();

            idleState.AddBehaviours(() => movement.Turn(100f));
            idleState.AddTransitions(new Transition(condition: () => GetKeyForward() && !GetKeyBackwards(), to: walkState));

            walkState.AddEnterBehaviours(() => animator.SetBool("isWalking", true));
            walkState.AddExitBehaviours(() => animator.SetBool("isWalking", false));
            walkState.AddBehaviours(
                () => movement.Turn(80f),
                () => movement.Move(1.6f)
            );
            walkState.AddTransitions(
                new Transition(condition: () => !GetKeyForward(), to: idleState),
                new Transition(condition: () => GetKeyRun(), to: runState)
            );

            runState.AddEnterBehaviours(() => animator.SetBool("isRunning", true));
            runState.AddExitBehaviours(() => animator.SetBool("isRunning", false));
            runState.AddBehaviours(() => movement.Turn(70f), () => movement.Move(3.1f));
            runState.AddTransitions(new Transition(() => !GetKeyRun() || !GetKeyForward(), walkState));

            stateMachine = new ModularStateMachine();
            stateMachine.Initialize(idleState);
        }

        private void Update()
        {
            stateMachine.Handle();
        }

        private void TurnNode(Transform targetTransform)
        {
            if (Input.GetKey(KeyCode.A))
            {
                targetTransform.Rotate(0, -100 * Time.deltaTime, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                targetTransform.Rotate(0, 100 * Time.deltaTime, 0);
            }
        }

        private bool GetKeyForward() => Input.GetKey(KeyCode.W);
        private bool GetKeyBackwards() => Input.GetKey(KeyCode.S);
        private bool GetKeyRun() => Input.GetKey(KeyCode.L);
    }
}
