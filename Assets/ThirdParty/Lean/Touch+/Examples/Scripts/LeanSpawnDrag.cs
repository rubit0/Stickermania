using UnityEngine;
using System.Collections.Generic;

namespace Lean.Touch
{
	/// <summary>This component extends LeanSpawn to also update the spawned position while the spawned object is being dragged.</summary>
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanSpawnDrag")]
	public class LeanSpawnDrag : LeanSpawn
	{
		/// <summary>This struct stores an associtation between a finger, and the prefab instance it spawned.</summary>
		struct Link
		{
			public LeanFinger Finger;
			public Transform  Root;
		}

		[System.NonSerialized]
		private List<Link> links = new List<Link>();

		/// <summary>This will spawn Prefab at the specified finger based on the ScreenDepth setting.</summary>
		public override void Spawn(LeanFinger finger)
		{
			var instance = default(Transform);

			if (TrySpawn(finger, ref instance) == true)
			{
				var link = default(Link);

				link.Finger = finger;
				link.Root   = instance;

				links.Add(link);
			}
		}

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerUp += FingerUp;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerUp -= FingerUp;
		}

		protected virtual void Update()
		{
			// Loop through and update all links
			for (var i = links.Count - 1; i >= 0; i--)
			{
				var link = links[i];

				if (link.Root != null)
				{
					UpdateSpawnedTransform(link.Finger, link.Root);
				}
			}
		}

		private void FingerUp(LeanFinger finger)
		{
			// Remove all links with the finger that just went up
			for (var i = links.Count - 1; i >= 0; i--)
			{
				var link = links[i];

				if (link.Finger == finger)
				{
					links.RemoveAt(i);
				}
			}
		}
	}
}