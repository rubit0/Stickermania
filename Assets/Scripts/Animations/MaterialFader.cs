using System.Linq;
using UnityEngine;
using DG.Tweening;

public class MaterialFader : MonoBehaviour
{
    [SerializeField]
    private float duration = 0.5f;
    [SerializeField]
    private float delay = 1f;
    [SerializeField]
    private float delaySecond = 2f;
    [SerializeField]
    private Texture[] textures;
    [SerializeField]
    private SkinnedMeshRenderer renderer;

    private Texture currentMainTexture;
    private Texture currentSecondaryTexture;
    private Material materialInstance;

    private void Start()
    {
        materialInstance = renderer.sharedMaterial;
        currentMainTexture = materialInstance.GetTexture("_MainTex");
        currentSecondaryTexture = materialInstance.GetTexture("_SecondaryTex");
    }

    public void FadeMaterial()
    {
        currentSecondaryTexture = GetRandomTexture();
        materialInstance.SetTexture("_SecondaryTex", currentSecondaryTexture);
        materialInstance.DOFloat(1f, "_Fade", duration)
            .SetDelay(delay)
            .OnComplete(() =>
            {
                SetSecondaryTextureAsMain();
                currentSecondaryTexture = GetRandomTexture(true);
                materialInstance.SetTexture("_SecondaryTex", currentSecondaryTexture);
                materialInstance.DOFloat(1f, "_Fade", duration)
                    .SetDelay(delaySecond)
                    .OnComplete(() => SetSecondaryTextureAsMain());
            });
    }

    private Texture GetRandomTexture(bool excludeSecondaryTexture = false)
    {
        var pool = textures.ToList();
        pool.Remove(currentMainTexture);
        if (excludeSecondaryTexture)
        {
            pool.Remove(currentSecondaryTexture);
        }

        var index = Random.Range(0, pool.Count - 1);

        return pool[index];
    }

    private void SetSecondaryTextureAsMain()
    {
        materialInstance.SetTexture("_MainTex", currentSecondaryTexture);
        currentMainTexture = currentSecondaryTexture;
        materialInstance.SetFloat("_Fade", 0f);
    }
}
