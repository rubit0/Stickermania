using UnityEngine;
using System.Collections.Generic;

namespace Lean.Touch
{
	/// <summary>This component calls the OnFlick event when a finger monitored by this component flicks.
	/// NOTE: This component doesn't do anything on its own, you first must call the AddFinger method.</summary>
	public class LeanManualFlick : LeanManualSwipe
	{
		[System.NonSerialized]
		private List<LeanFinger> fingers = new List<LeanFinger>();

		protected virtual void Update()
		{
			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				if (UpdateFinger(i) == true)
				{
					fingers.RemoveAt(i);
				}
			}
		}

		protected override void FingerUp(LeanFinger finger)
		{
			fingers.Remove(finger);
		}

		private bool UpdateFinger(int index)
		{
			var finger = fingers[index];

			// Remove fingers that are too old
			if (finger.Age >= LeanTouch.CurrentTapThreshold)
			{
				return true;
			}

			for (var i = finger.Snapshots.Count - 1; i >= 0; i--)
			{
				var shapshot = finger.Snapshots[i];
				var delta    = finger.ScreenPosition - shapshot.ScreenPosition;

				// Invalid angle?
				if (CheckAngle == true)
				{
					var angle      = Mathf.Atan2(delta.x, delta.y) * Mathf.Rad2Deg;
					var angleDelta = Mathf.DeltaAngle(angle, Angle);

					if (angleDelta < AngleThreshold * -0.5f || angleDelta >= AngleThreshold * 0.5f)
					{
						continue;
					}
				}

				if (delta.magnitude * LeanTouch.ScalingFactor > LeanTouch.CurrentSwipeThreshold)
				{
					if (onSwipe != null)
					{
						onSwipe.Invoke(finger);
					}

					return true;
				}
			}

			return false;
		}
	}
}