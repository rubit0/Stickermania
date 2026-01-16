using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace XR
{
    [RequireComponent(typeof(ARFeatheredPlaneMeshVisualizer))]
    [RequireComponent(typeof(FadePlaneOnBoundaryChange))]
    [RequireComponent(typeof(MeshRenderer))]
    public class ARPlaneVisualizationController : MonoBehaviour
    {
        [Serializable]
        public enum VisualizationMode
        {
            Feathered,
            TransparentShadow,
            Invisible
        }

        [SerializeField]
        private ARFeatheredPlaneMeshVisualizer arFeatheredPlaneMeshVisualizer;
        [SerializeField]
        private FadePlaneOnBoundaryChange fadePlaneOnBoundary;
        [SerializeField]
        private Material featheredPlaneMaterial;
        [SerializeField]
        private Material shadowPlaneMaterial;

        private MeshRenderer meshRenderer;
        private ARPlaneMeshVisualizer arPlaneMeshVisualizer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            arPlaneMeshVisualizer = GetComponent<ARPlaneMeshVisualizer>();
        }

        public void ToggleVisualizationMode(VisualizationMode mode)
        {
            switch (mode)
            {
                case VisualizationMode.Feathered:
                    arPlaneMeshVisualizer.enabled = true;
                    meshRenderer.sharedMaterial = featheredPlaneMaterial;
                    arFeatheredPlaneMeshVisualizer.enabled = true;
                    fadePlaneOnBoundary.enabled = true;
                    break;
                case VisualizationMode.TransparentShadow:
                    arPlaneMeshVisualizer.enabled = true;
                    arFeatheredPlaneMeshVisualizer.enabled = false;
                    fadePlaneOnBoundary.enabled = false;
                    meshRenderer.sharedMaterial = shadowPlaneMaterial;
                    break;
                case VisualizationMode.Invisible:
                    arPlaneMeshVisualizer.enabled = false;
                    arFeatheredPlaneMeshVisualizer.enabled = false;
                    fadePlaneOnBoundary.enabled = false;
                    break;
            }
        }
    }
}
