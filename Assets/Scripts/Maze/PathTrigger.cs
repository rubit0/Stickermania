using System.Linq;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(BoxCollider))]
public class PathTrigger : MonoBehaviour
{
    private class MeshMaterialPair
    {
        public MeshRenderer MeshRenderer;
        public Material Material;

        public MeshMaterialPair(MeshRenderer renderer)
        {
            MeshRenderer = renderer;
            Material = renderer.material;
        }

        public void FadeToUnlit(string shaderPropertyName)
        {
            Material.DOFloat(0.4f, shaderPropertyName, 0.75f);
        }
    }

    [SerializeField]
    private MeshRenderer[] meshes;
    [SerializeField]
    private string playerTag = "Player";
    [SerializeField]
    private string shaderPropertyName = "_Unlit";

    private MeshMaterialPair[] originalMaterials;

    private void Awake()
    {
        originalMaterials = meshes.Select(m => new MeshMaterialPair(m)).ToArray();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(playerTag))
        {
            return;
        }

        foreach (var material in originalMaterials)
        {
            material.FadeToUnlit(shaderPropertyName);
        }

        Destroy(this);
    }
}
