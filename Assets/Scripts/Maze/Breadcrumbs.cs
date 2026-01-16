using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Breadcrumbs : MonoBehaviour
{
    [SerializeField]
    private Transform localParent;
    [SerializeField]
    private bool tryFindParent = true;
    [SerializeField]
    public LineRenderer lineRendererPrefab;
    [SerializeField]
    private float visibilityDuration = 3f;
    [SerializeField]
    private float lineWidth = 0.3f;

    private List<Transform> breadcrumbs;
    private Tween fadeOutTween;

    private GameObject breadcrumbRootInstance;

    private void OnEnable()
    {
        if(localParent == null && tryFindParent)
        {
            localParent = transform.parent;
        }
    }

    public void AddBreadCrumb(Vector3 position)
    {
        if (breadcrumbs == null || !breadcrumbs.Any())
        {
            breadcrumbs = new List<Transform>();
            breadcrumbRootInstance = new GameObject("BreadcrumbsRoot");
            breadcrumbRootInstance.transform.SetParent(localParent);
        }

        var go = new GameObject($"Breadcrumb_#{breadcrumbs.Count}");
        go.transform.SetParent(breadcrumbRootInstance.transform);
        position.y += (0.25f - Random.Range(0.01f, 0.019f)) * localParent.localScale.x;
        go.transform.position = position;

        breadcrumbs.Add(go.transform);
    }

    public void PresentBreadcrumbs()
    {
        if (breadcrumbs == null || !breadcrumbs.Any())
        {
            return;
        }

        if(fadeOutTween != null && fadeOutTween.IsActive())
        {
            fadeOutTween.Kill(true);
            StopAllCoroutines();
        }

        StartCoroutine(AnimateLinesCoroutine());
    }

    private IEnumerator AnimateLinesCoroutine()
    {
        var currentRoot = breadcrumbRootInstance;
        breadcrumbRootInstance = null;
        var currentBreadcrumbs = breadcrumbs;
        breadcrumbs = null;

        var lineRendererInstance = Instantiate(lineRendererPrefab);
        var targetWidth = lineWidth * localParent.localScale.x;
        lineRendererInstance.startWidth = targetWidth;
        lineRendererInstance.endWidth = targetWidth;
        lineRendererInstance.transform.SetParent(currentRoot.transform);
        lineRendererInstance.positionCount = currentBreadcrumbs.Count;
        for (int i = 0; i < currentBreadcrumbs.Count; i++)
        {
            lineRendererInstance.SetPosition(i, currentBreadcrumbs[i].position);
        }

        var sourceColor = new Color2(lineRendererInstance.colorGradient.colorKeys[0].color, lineRendererInstance.colorGradient.colorKeys[1].color);
        var targetColor = new Color2(lineRendererInstance.colorGradient.colorKeys[0].color, lineRendererInstance.colorGradient.colorKeys[1].color);
        targetColor.ca.a = 0f;
        targetColor.cb.a = 0f;
        fadeOutTween = lineRendererInstance
            .DOColor(sourceColor, targetColor, 1f)
            .SetDelay(visibilityDuration);

        while (fadeOutTween.IsActive())
        {
            yield return null;
            for (int i = 0; i < currentBreadcrumbs.Count; i++)
            {
                lineRendererInstance.SetPosition(i, currentBreadcrumbs[i].position);
            }

            targetWidth = lineWidth * localParent.localScale.x;
            lineRendererInstance.startWidth = targetWidth;
            lineRendererInstance.endWidth = targetWidth;
        }

        fadeOutTween = null;
        Destroy(lineRendererInstance.gameObject);
        lineRendererInstance = null;
        Destroy(currentRoot);
        currentBreadcrumbs = null;
    }
}
