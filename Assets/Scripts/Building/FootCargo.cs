using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Trains
{
    public class FootCargo : MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField] float stepDist;
        public bool ShouldContinueGoingAfterTrain;
        public string destinationDisplay;
        IFootCargoDestination destination;
        public CargoType CargoType;
        public int Amnt;
        NavMeshAgent agent;
        Vector3[] path;
        int idx = 0;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void OnEnable()
        {
            Global.OnTick += Tick;
        }

        private void OnDisable()
        {
            Global.OnTick -= Tick;
        }

        public void Configure(CargoType cargoType, int amnt, Vector3 start, IFootCargoDestination destination)
        {
            transform.position = start;
            this.CargoType = cargoType;
            this.Amnt = amnt;
            this.destination = destination;
            destinationDisplay = destination.ToString();
            NavMeshPath path = new();
            int everythingMask = -1;
            NavMesh.CalculatePath(start, destination.transform.position, everythingMask, path);
            this.path = path.corners;
        }

        private void Tick(object sender, EventArgs e)
        {
            MovePerTick();
        }

        private void MovePerTick()
        {
            if (idx > path.Length - 1) return;

            transform.position = Vector3.MoveTowards(transform.position, path[idx], speed * Time.deltaTime);
            if (Vector3.SqrMagnitude(transform.position - path[idx]) < stepDist * stepDist)
            {
                transform.position = path[idx];
                idx++;
            }

            if (idx > path.Length - 1)
            {
                //idx--;
                OnReachedDestination();
            }
        }

        private void OnReachedDestination()
        {
            if (Vector3.SqrMagnitude(transform.position - destination.transform.position) < 0.1f)
            {
                //destination.Supply.Amnts[cargoType] += amnt;
                destination.OnFootCargoCame(this);
                //destination.Consumption.Amnts[cargoType] += amnt;
                Destroy(gameObject);
            }
        }
    }
}
