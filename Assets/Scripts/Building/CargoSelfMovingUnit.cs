using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Trains
{
    public class CargoSelfMovingUnit : MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField] float stepDist;
        NavMeshAgent agent;
        Vector3[] points;
        int idx = 0;
        StationCargoHandler destination;

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

        public void Configure(Vector3 start, StationCargoHandler station)
        {
            transform.position = start;
            destination = station;
            NavMeshPath path = new();
            int everythingMask = -1;
            NavMesh.CalculatePath(start, station.transform.position, everythingMask, path);
            points = path.corners;
        }

        private void Tick(object sender, EventArgs e)
        {
            MovePerTick();
        }

        private void MovePerTick()
        {
            if (idx > points.Length - 1) return;

            transform.position = Vector3.MoveTowards(transform.position, points[idx], speed * Time.deltaTime);
            if (Vector3.SqrMagnitude(transform.position - points[idx]) < stepDist * stepDist)
            {
                transform.position = points[idx];
                idx++;
            }

            if (idx >= points.Length - 1)
            {
                //send cargo from this to station
                Destroy(gameObject);
            }
        }
    }
}
