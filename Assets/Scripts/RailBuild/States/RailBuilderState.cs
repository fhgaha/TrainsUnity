using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
	public class RailBuilderState
	{
		public virtual RailBuilderState HandleInput(Camera camera) { return this; }
		public static SelectingStart selectingStart;
		public static DrawingInitialSegment drawingInitialSegment;
		public static DrawingNoninitialSegment drawingNoninitialSegment;
		protected static RailBuilder rb;

		public static RailBuilderState Init(RailBuilder rb)
		{
			selectingStart = new();
			drawingInitialSegment = new();
			drawingNoninitialSegment = new();
			RailBuilderState.rb = rb;
			return selectingStart;
		}
	}

	public class SelectingStart : RailBuilderState
	{
		//move mouse around, click lmb to start drawing
		public override RailBuilderState HandleInput(Camera camera)
		{
			if (Input.GetKeyUp(KeyCode.Mouse0))
			{
				if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f))
				{
					rb.startPos = hit.point;
					return drawingInitialSegment;
				}
			}

			return selectingStart;
		}
	}

	public class DrawingInitialSegment : RailBuilderState
	{
		private Vector3 mousePos;

		public override RailBuilderState HandleInput(Camera camera)
		{
			//handle mouse movement
			if (Input.mousePosition != mousePos)
			{
				if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f))
				{
					rb.endPos = hit.point;
					rb.CalculateStraightPoints(rb.startPos, hit.point);
				}

				mousePos = Input.mousePosition;
			}

			//on lmb save drawn segment to a rail container
			if (Input.GetKeyUp(KeyCode.Mouse0))
			{
				rb.PutDrawnSegmentToContainer();

				rb.startPos = rb.endPos;
				rb.startHeading = rb.endHeading;

				return drawingNoninitialSegment;
			}

			//on rmb cancel drawing
			if (Input.GetKeyUp(KeyCode.Mouse1))
			{
				return selectingStart;
			}

			return drawingInitialSegment;
		}
	}

	public class DrawingNoninitialSegment : RailBuilderState
	{
		private Vector3 mousePos;

		public override RailBuilderState HandleInput(Camera camera)
		{
			//handle mouse movement
			if (Input.mousePosition != mousePos)
			{
				if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f))
				{
					rb.endPos = hit.point;
					rb.CalculateDubinsPoints(rb.startPos, hit.point);
				}

				mousePos = Input.mousePosition;
			}

			//on lmb save drawn segment to a rail container
			if (Input.GetKeyUp(KeyCode.Mouse0))
			{
				rb.PutDrawnSegmentToContainer();

				rb.startPos = rb.endPos;
				rb.startHeading = rb.endHeading;

				return drawingNoninitialSegment;
			}

			//on rmb cancel drawing
			if (Input.GetKeyUp(KeyCode.Mouse1))
			{
				return selectingStart;
			}

			return drawingNoninitialSegment;
		}
	}
}