using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class CubeQueueCommandTest : MonoBehaviour
    {
        Queue queue;

        private void Start()
        {
            queue = new();
            queue.Add(new Wait(Random.Range(0.5f, 5)))
                .Add(new MoveTo(transform, 10 * Vector3.up, 10))
                .Add(new MoveTo(transform, Vector3.zero, 10));
        }

        private void Update()
        {
            if (queue.IsFinished)
            {
                queue.Restart();
            }

            queue.Tick(Time.deltaTime);
        }
    }

    public class Wait : ICommand
    {
        float waitTime;
        float clock;

        public Wait(float t)
        {
            waitTime = t;
        }

        public void Enter(Queue queue)
        {
            clock = 0;
        }

        public bool Tick(Queue queue, float dt)
        {
            clock += dt;
            return clock > waitTime;
        }
    }

    public class MoveTo : ICommand
    {
        Transform trns;
        private Vector3 target;
        private float speed;

        public MoveTo(Transform trns, Vector3 target, float speed)
        {
            this.trns = trns;
            this.target = target;
            this.speed = speed;
        }

        public void Enter(Queue queue)
        {

        }

        public bool Tick(Queue queue, float dt)
        {
            trns.position = Vector3.MoveTowards(trns.position, target, speed * dt);
            return trns.position == target;
        }
    }

    public class Queue
    {
        public bool IsFinished;
        public int pointer;
        public List<ICommand> queue = new();
        public ICommand currentCommand;

        public void Tick(float dt)
        {
            if (pointer >= queue.Count)
            {
                IsFinished = true;
                return;
            }

            var command = queue[pointer];

            if (currentCommand != command)
            {
                currentCommand = command;
                currentCommand.Enter(this);
            }

            if (command.Tick(this, dt))
            {
                pointer++;
            }
        }

        public Queue Add(ICommand wait)
        {
            queue.Add(wait);
            return this;
        }

        public void Restart()
        {
            IsFinished = false;
            pointer = 0;
        }
    }

    public interface ICommand
    {
        void Enter(Queue queue);
        bool Tick(Queue queue, float dt);
        //exit
    }
}
