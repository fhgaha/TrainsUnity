using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
	public class RailBuilder : MonoBehaviour
	{
		public List<Vector3> points;
		public bool HasPoints => points != null && points.Count > 0;
		public Vector3 startPos, endPos;
		public float startHeading, endHeading;

		[SerializeField] private Camera _camera;
		[SerializeField] private string _stateName;
		[SerializeField] private LineRenderer _lineRenderer;
		[SerializeField] private Mesh2D _road2DSO;
		[SerializeField] private RailContainer _railContainer;
		[SerializeField] private RoadSegment _segment;
		private RailBuilderState _state;
		private DubinsGeneratePaths dubinsPathGenerator = new();
		
		private void OnEnable()
		{
			_state = RailBuilderState.Init(this);
		}

		private void OnDisable()
		{
		}

		private void Update()
		{
			_state = _state.HandleInput(_camera);
			_stateName = _state.GetType().Name;

			if (HasPoints)
			{
				//draw line
				_lineRenderer.positionCount = points.Count;
				_lineRenderer.SetPositions(points.ToArray());
			}
		}

		public void CalculateStraightPoints(Vector3 startPos, Vector3 endPos)
		{
			startHeading = Vector3.SignedAngle(Vector3.forward, endPos - startPos, Vector3.up);
			endHeading = Vector3.SignedAngle(Vector3.forward, endPos - startPos, Vector3.up);

            List<OneDubinsPath> paths = dubinsPathGenerator.GetAllDubinsPaths(
				startPos,
				startHeading * Mathf.Deg2Rad,
				endPos,
				endHeading * Mathf.Deg2Rad
			);

            OneDubinsPath shortest = paths.Aggregate((min, next) => next.pathCoordinates.Count < min.pathCoordinates.Count ? next : min);

			if (shortest != null && shortest.pathCoordinates.Count > 0)
			{
				points = shortest.pathCoordinates;
				_segment.GenerateMesh(points);
			}
		}

		public void CalculateDubinsPoints(Vector3 startPos, Vector3 endPos)
		{
			// startHeading = Vector3.SignedAngle(Vector3.forward, endPos - startPos, Vector3.up);
			endHeading = Vector3.SignedAngle(Vector3.forward, endPos - startPos, Vector3.up);

			List<OneDubinsPath> paths = dubinsPathGenerator.GetAllDubinsPaths(
				startPos,
				startHeading * Mathf.Deg2Rad,
				endPos,
				endHeading * Mathf.Deg2Rad
			);

			OneDubinsPath shortest = paths.Aggregate((min, next) => next.pathCoordinates.Count < min.pathCoordinates.Count ? next : min);



            #region ugly doing same thing twice. not working

            //endHeading should be from tangent2 to endPos

            endHeading = Vector3.SignedAngle(Vector3.forward, endPos - shortest.tangent1, Vector3.up);

			paths = dubinsPathGenerator.GetAllDubinsPaths(
				startPos,
				startHeading * Mathf.Deg2Rad,
				endPos,
				endHeading * Mathf.Deg2Rad
			);

			shortest = paths.Aggregate((min, next) => next.pathCoordinates.Count < min.pathCoordinates.Count ? next : min);

            #endregion




			if (shortest != null && shortest.pathCoordinates.Count > 0)
			{
				points = shortest.pathCoordinates;
				_segment.GenerateMesh(points);
			}
		}

		public void PutDrawnSegmentToContainer()
		{
			_segment.points = points;
			_railContainer.Add(_segment);
		}
	}
}
