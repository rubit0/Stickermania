using UnityEngine;

namespace Lean.Touch
{
	/// <summary>This component modifies LeanSelect to allow selection of 2D + 3D + UI at the same time.</summary>
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanSelectMulti")]
	public class LeanSelectMulti : LeanSelect
	{
		public SelectType SelectUsingAlt = SelectType.Manually;

		public SelectType SelectUsingAltAlt = SelectType.Manually;

		public override void SelectScreenPosition(LeanFinger finger, Vector2 screenPosition)
		{
			// Stores the component we hit (Collider or Collider2D)
			var component = default(Component);

			TryGetComponent(SelectUsing, screenPosition, ref component);

			if (component == null)
			{
				TryGetComponent(SelectUsingAlt, screenPosition, ref component);

				if (component == null)
				{
					TryGetComponent(SelectUsingAltAlt, screenPosition, ref component);
				}
			}

			Select(finger, component);
		}
	}
}