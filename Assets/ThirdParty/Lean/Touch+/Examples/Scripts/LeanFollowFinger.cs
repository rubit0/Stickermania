using UnityEngine;
using System.Collections.Generic;

namespace Lean.Touch
{
	/// <summary>This component causes the current GameObject to follow the specified finger around the screen at a set speed.
	/// NOTE: You must call the BeginFollowing function from somewhere (e.g. LeanFingerDown, LeanSelectable.OnSelect).</summary>
	public class LeanFollowFinger : MonoBehaviour
	{
		public enum RotateType
		{
			None,
			Forward,
			TopDown,
			Side2D
		}

		[Tooltip("The conversion method used to find a world point from a screen point")]
		public LeanScreenDepth ScreenDepth;

		[Tooltip("This allows you to specify how many seconds must elapse before the following begins.")]
		public float FollowDelay;

		[Tooltip("When this object is within this many world space units of the next point, it will be removed.")]
		public float Threshold = 0.001f;

		[Tooltip("The speed of the following in units per seconds.")]
		public float Speed = 1.0f;

		[Tooltip("Should this GameObject be rotated to the follow path too?")]
		public RotateType RotateTo;

		[Tooltip("If you enable RotateTo, then you can set the speed of the rotation here.")]
		public float RotateDampening = 10.0f;

		// This stores the future positions we want to move to
		private LinkedList<Vector3> positions = new LinkedList<Vector3>();

		// This is the finger we are currently following
		private LeanFinger currentFinger;

		// This allows for the follow delay
		private float currentTimer;

		/// <summary>Make sure you call this from somewhere (e.g. <b>LeanFingerDown</b>, <b>LeanSelectable.OnSelect</b>) to begin the following.</summary>
		public void BeginFollowing(LeanFinger finger)
		{
			positions.Clear();

			currentFinger = finger;
			currentTimer  = 0.0f;
		}

		protected virtual void OnEnable()
		{
			// Hook events
			LeanTouch.OnFingerSet += OnFingerSet;
			LeanTouch.OnFingerUp  += OnFingerUp;
		}

		protected virtual void OnDisable()
		{
			// Unhook events
			LeanTouch.OnFingerSet -= OnFingerSet;
			LeanTouch.OnFingerUp  -= OnFingerUp;
		}

		protected virtual void Update()
		{
			TrimPositions();

			if (positions.Count > 0)
			{
				currentTimer += Time.deltaTime;

				if (currentTimer >= FollowDelay)
				{
					if (positions.Count > 0)
					{
						var currentPosition = transform.position;
						var targetPosition  = positions.First.Value;

						UpdateRotation(targetPosition - currentPosition);

						currentPosition = Vector3.MoveTowards(currentPosition, targetPosition, Speed * Time.deltaTime);

						transform.position = currentPosition;
					}
				}
			}
		}

		private void UpdateRotation(Vector3 vector)
		{
			var currentRotation = transform.localRotation;

			switch (RotateTo)
			{
				case RotateType.Forward:
				{
					transform.forward = vector;
				}
				break;

				case RotateType.TopDown:
				{
					var yaw = Mathf.Atan2(vector.x, vector.z) * Mathf.Rad2Deg;

					transform.rotation = Quaternion.Euler(0.0f, yaw, 0.0f);
				}
				break;

				case RotateType.Side2D:
				{
					var roll = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;

					transform.rotation = Quaternion.Euler(0.0f, 0.0f, -roll);
				}
				break;
			}

			var factor = Lean.Common.LeanHelper.DampenFactor(RotateDampening, Time.deltaTime);

			transform.localRotation = Quaternion.Slerp(currentRotation, transform.localRotation, factor);
		}

		private void TrimPositions()
		{
			var currentPosition = transform.position;

			while (positions.Count > 0)
			{
				var first    = positions.First;
				var distance = Vector3.Distance(currentPosition, first.Value);

				if (distance > Threshold)
				{
					break;
				}

				positions.Remove(first);
			}
		}

		private void TryAddPosition(Vector3 newPosition)
		{
			var currentPosition = transform.position;

			// Only add newPosition if it's far enough away from the last added point
			if (positions.Count == 0 || Vector3.Distance(positions.Last.Value, newPosition) > Threshold)
			{
				positions.AddLast(newPosition);
			}
		}

		private void OnFingerSet(LeanFinger finger)
		{
			if (finger == currentFinger)
			{
				var position = ScreenDepth.Convert(finger.ScreenPosition, gameObject);

				TryAddPosition(position);
			}
		}

		private void OnFingerUp(LeanFinger finger)
		{
			if (finger == currentFinger)
			{
				currentFinger = null;
			}
		}
	}
}