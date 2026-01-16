using Lean.Touch;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LeanPath))]
[RequireComponent(typeof(LeanPathLineRenderer))]
public class WaypointBuilder : MonoBehaviour
{
    [SerializeField]
    private Transform localParent;
    [SerializeField]
    private Transform waypointsParent;
    [SerializeField]
    private LeanPathLineRenderer pathRenderer;
    [SerializeField]
    private float lineWidth = 0.3f;

    private void Awake()
    {
        var waypoints = new List<Vector3>(waypointsParent.childCount);

        for (int i = 0; i < waypointsParent.childCount; i++)
        {
            waypoints.Add(waypointsParent.GetChild(i).localPosition);
        }

        pathRenderer.Path.Points = waypoints;
    }

    private void Update()
    {
        var targetWidth = lineWidth * localParent.localScale.x;
        pathRenderer.LineRenderer.startWidth = targetWidth;
        pathRenderer.LineRenderer.endWidth = targetWidth;
    }
}
