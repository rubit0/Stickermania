using UnityEngine;

namespace Lean.Touch
{
	/// <summary>This script will constrain the current transform.position to the specified collider.
	/// NOTE: If you're using a MeshCollider then it must be marked as <b>convex</b>.</summary>
	public class LeanConstrainToCollider : MonoBehaviour
	{
		[Tooltip("The collider this transform will be constrained to.")]
		public Collider Target;

		protected virtual void LateUpdate()
		{
			if (Target != null)
			{
				transform.position = Target.ClosestPoint(transform.position);
			}
		}
	}
}