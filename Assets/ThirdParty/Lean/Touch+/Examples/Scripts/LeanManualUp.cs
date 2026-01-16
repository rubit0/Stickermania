using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System.Collections.Generic;

namespace Lean.Touch
{
	/// <summary>This component calls the OnUp event when a finger monitored by this component goes up.
	/// NOTE: This component doesn't do anything on its own, you first must call the AddFinger method.</summary>
	public class LeanManualUp : MonoBehaviour
	{
		// Event signature
		[System.Serializable] public class FingerEvent : UnityEvent<LeanFinger> {}

		/// <summary>This event is invoked when a monitored finger taps.</summary>
		public FingerEvent OnUp { get { if (onUp == null) onUp = new FingerEvent(); return onUp; } } [FormerlySerializedAs("OnUp")] [SerializeField] private FingerEvent onUp;

		[System.NonSerialized]
		private List<LeanFinger> fingers = new List<LeanFinger>();

		/// <summary>This removes all monitored fingers from this component.</summary>
		public void ClearFingers()
		{
			fingers.Clear();
		}

		/// <summary>If you're using manual finger filtering then you must call this method to add a finger to be monitored by this component.</summary>
		public void AddFinger(LeanFinger finger)
		{
			if (fingers.Contains(finger) == false)
			{
				fingers.Add(finger);
			}
		}

		protected virtual void OnEnable()
		{
			// Hook events
			LeanTouch.OnFingerUp += FingerUp;
		}

		protected virtual void OnDisable()
		{
			// Unhook events
			LeanTouch.OnFingerUp -= FingerUp;

			ClearFingers();
		}

		private void FingerUp(LeanFinger finger)
		{
			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				if (fingers[i] == finger)
				{
					if (onUp != null)
					{
						onUp.Invoke(finger);
					}

					fingers.RemoveAt(i);

					return;
				}
			}
		}
	}
}