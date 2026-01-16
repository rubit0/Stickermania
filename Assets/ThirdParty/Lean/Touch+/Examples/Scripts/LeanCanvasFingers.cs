using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System.Collections.Generic;

namespace Lean.Touch
{
	/// <summary>This component stores a list of all fingers that began touching the current RectTransform.</summary>
	[RequireComponent(typeof(RectTransform))]
	public class LeanCanvasFingers : MonoBehaviour
	{
		// Event signature
		[System.Serializable] public class LeanFingerEvent : UnityEvent<LeanFinger> {}
		[System.Serializable] public class LeanFingerListEvent : UnityEvent<List<LeanFinger>> {}

		public LeanFingerEvent OnDown { get { if (onDown == null) onDown = new LeanFingerEvent(); return onDown; } } [FormerlySerializedAs("OnDown")] [SerializeField] private LeanFingerEvent onDown;

		public LeanFingerListEvent OnSet { get { if (onSet == null) onSet = new LeanFingerListEvent(); return onSet; } } [FormerlySerializedAs("OnSet")] [SerializeField] private LeanFingerListEvent onSet;

		[System.NonSerialized]
		public List<LeanFinger> Fingers = new List<LeanFinger>();

		protected virtual void OnEnable()
		{
			// Hook events
			LeanTouch.OnFingerDown += FingerDown;
			LeanTouch.OnFingerUp   += FingerUp;
		}

		protected virtual void OnDisable()
		{
			// Unhook events
			LeanTouch.OnFingerDown -= FingerDown;
			LeanTouch.OnFingerUp   -= FingerUp;
		}

		protected virtual void Update()
		{
			if (Fingers.Count > 0)
			{
				if (onSet != null)
				{
					onSet.Invoke(Fingers);
				}
			}
		}

		private void FingerDown(LeanFinger finger)
		{
			var results = LeanTouch.RaycastGui(finger.ScreenPosition);

			if (results != null && results.Count > 0)
			{
				if (results[0].gameObject == gameObject)
				{
					Fingers.Add(finger);

					if (onDown != null)
					{
						onDown.Invoke(finger);
					}
				}
			}
		}

		private void FingerUp(LeanFinger finger)
		{
			Fingers.Remove(finger);
		}
	}
}