using UnityEngine;

namespace Lean.Touch
{
	/// <summary>This component allows you to select multiple objects by dragging across the screen through them.</summary>
	public class LeanDragSelect : MonoBehaviour
	{
		[Tooltip("The select component that will be used.")]
		public LeanSelect Select;

		[Tooltip("If you begin dragging while objects are already selected, skip?")]
		public bool RequireNoSelectables;

		[Tooltip("If you begin dragging on a point that isn't above a selectable object, skip?")]
		public bool RequireInitialSelection;

		[Tooltip("Autoaticaly deselect all objects when the drag starts?")]
		public bool DeselectAllAtStart;

		[Tooltip("Must the next selected object be within a specified world space distance? (0 = any distance)")]
		public float MaximumSeparation;

		[System.NonSerialized]
		private LeanFinger selectingFinger;

		[System.NonSerialized]
		private LeanSelectable lastSelectable;

		[System.NonSerialized]
		private bool waitingForSelection;

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerDown += FingerDown;
			LeanTouch.OnFingerSet  += FingerSet;
			LeanTouch.OnFingerUp   += FingerUp;

			LeanSelectable.OnSelectGlobal += SelectGlobal;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerDown -= FingerDown;
			LeanTouch.OnFingerSet  -= FingerSet;
			LeanTouch.OnFingerUp   -= FingerUp;

			LeanSelectable.OnSelectGlobal -= SelectGlobal;
		}

		private void FingerDown(LeanFinger finger)
		{
			if (Select != null)
			{
				if (RequireNoSelectables == true && LeanSelectable.IsSelectedCount > 0)
				{
					return;
				}

				if (DeselectAllAtStart == true)
				{
					LeanSelectable.DeselectAll();
				}

				if (RequireInitialSelection == true)
				{
					waitingForSelection = true;

					Select.SelectScreenPosition(finger);

					waitingForSelection = false;
				}
				else
				{
					selectingFinger = finger;

					Select.SelectScreenPosition(finger);
				}
			}
		}

		private void FingerSet(LeanFinger finger)
		{
			if (finger == selectingFinger)
			{
				if (Select != null)
				{
					Select.SelectScreenPosition(finger);
				}
			}
		}

		private void FingerUp(LeanFinger finger)
		{
			if (finger == selectingFinger)
			{
				selectingFinger = null;
				lastSelectable  = null;
			}
		}

		private void SelectGlobal(LeanSelectable selectable, LeanFinger finger)
		{
			if (waitingForSelection == true)
			{
				selectingFinger = finger;
				lastSelectable  = selectable;
			}
			else if (finger == selectingFinger)
			{
				// Good selection?
				if (MaximumSeparation <= 0.0f || Vector3.Distance(lastSelectable.transform.position, selectable.transform.position) <= MaximumSeparation)
				{
					lastSelectable = selectable;
				}
				// Too far to select?
				else
				{
					selectable.Deselect();
				}
			}
		}
	}
}