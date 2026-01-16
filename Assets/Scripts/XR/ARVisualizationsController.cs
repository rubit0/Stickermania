using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using XR;

public class ARVisualizationsController : MonoBehaviour
{
    public VisualizationMode PointCloudsVisualizationMode { get; private set; } = VisualizationMode.Show;
    public VisualizationMode PlanesVisualizationMode { get; private set; } = VisualizationMode.Show;
    public ARPlaneManager PlaneManager => planeManager;
    public ARPointCloudManager PointCloudManager => pointCloudManager;

    [Serializable]
    public enum VisualizationMode
    {
        Invisible,
        Shadow,
        Show
    }

    [SerializeField]
    private ARPlaneManager planeManager;
    [SerializeField]
    private ARPointCloudManager pointCloudManager;

    private void Start()
    {
        pointCloudManager.pointCloudsChanged += OnPointCloudsChanged;
        planeManager.planesChanged += OnPlanesChanged;
    }

    #region PointClouds
    public void ChangePointCloudVisualization(VisualizationMode mode)
    {
        if (PointCloudsVisualizationMode == mode)
        {
            return;
        }
        PointCloudsVisualizationMode = mode;

        var trackables = new List<ARPointCloud>(pointCloudManager.trackables.count);
        foreach (var trackable in pointCloudManager.trackables)
        {
            trackables.Add(trackable);
        }

        ChangePointCloudsVisualization(trackables, mode);
    }

    private void ChangePointCloudsVisualization(List<ARPointCloud> clouds, VisualizationMode mode)
    {
        foreach (var trackable in clouds)
        {
            trackable.gameObject.SetActive(mode == VisualizationMode.Show);
        }
    }

    private void OnPointCloudsChanged(ARPointCloudChangedEventArgs arg)
    {
        ChangePointCloudsVisualization(arg.added, PointCloudsVisualizationMode);
    }
    #endregion

    #region Planes
    public void ChangePlaneVisualization(VisualizationMode mode)
    {
        if (PlanesVisualizationMode == mode)
        {
            return;
        }
        PlanesVisualizationMode = mode;

        foreach (var trackable in planeManager.trackables)
        {
            ChangePlaneVisualization(trackable, PlanesVisualizationMode);
        }
    }

    private void ChangePlaneVisualization(ARPlane plane, VisualizationMode mode)
    {
        var planeVisualizationController = plane.GetComponent<ARPlaneVisualizationController>();
        if (planeVisualizationController)
        {
            Debug.Log($"### Changing Plance Visualization for {plane.trackableId} to {mode}");

            ARPlaneVisualizationController.VisualizationMode visualizationMode = ARPlaneVisualizationController.VisualizationMode.Feathered;
            switch (mode)
            {
                case VisualizationMode.Invisible:
                    visualizationMode = ARPlaneVisualizationController.VisualizationMode.Invisible;
                    break;
                case VisualizationMode.Shadow:
                    visualizationMode = ARPlaneVisualizationController.VisualizationMode.TransparentShadow;
                    break;
                case VisualizationMode.Show:
                    visualizationMode = ARPlaneVisualizationController.VisualizationMode.Feathered;
                    break;
            }

            planeVisualizationController.ToggleVisualizationMode(visualizationMode);
        }
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        foreach (var trackable in args.added)
        {
            ChangePlaneVisualization(trackable, PlanesVisualizationMode);
        }
    }
    #endregion
}
