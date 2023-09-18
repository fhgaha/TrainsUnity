using UnityEngine;

namespace Trains
{
    public enum PathMovementStyle { Continuous, Slerp, Lerp, }

    public class PathController : MonoBehaviour
    {
        public float MovementSpeed = 20;
        public Transform PathContainer;

        public PathMovementStyle MovementStyle;
        public bool LoopThroughPoints = true;
        public bool StartAtFirstPointOnAwake = true;

        private Transform[] _points;

        private int _currentTargetIdx;

        private void Awake()
        {
            _points = PathContainer.GetComponentsInChildren<Transform>();
            if (StartAtFirstPointOnAwake)
            {
                transform.position = _points[0].position;
            }
        }

        private void Update()
        {
            if (_points == null || _points.Length == 0) return;

            var distance = Vector3.Distance(transform.position, _points[_currentTargetIdx].position);
            if (distance * distance < 0.1f)
            {
                _currentTargetIdx++;
                if (_currentTargetIdx >= _points.Length)
                {
                    _currentTargetIdx = LoopThroughPoints ? 0 : _points.Length - 1;
                }
            }

            transform.position = MovementStyle switch
            {
                PathMovementStyle.Lerp => Vector3.Lerp(transform.position, _points[_currentTargetIdx].position, MovementSpeed * Time.deltaTime),
                PathMovementStyle.Slerp => Vector3.Slerp(transform.position, _points[_currentTargetIdx].position, MovementSpeed * Time.deltaTime),
                _ => Vector3.MoveTowards(transform.position, _points[_currentTargetIdx].position, MovementSpeed * Time.deltaTime),
            };
        }

        private void OnDrawGizmosSelected()
        {
            if (_points == null || _points.Length == 0) return;

            for (int i = 0; i < _points.Length; i++)
            {
                Gizmos.color = Color.yellow;
                if (i < _currentTargetIdx) Gizmos.color = Color.red;
                if (i > _currentTargetIdx) Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(_points[i].position, 1f);
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _points[_currentTargetIdx].position);
        }
    }
}
