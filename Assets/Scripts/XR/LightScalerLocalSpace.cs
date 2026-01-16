using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightScalerLocalSpace : MonoBehaviour
{
    [SerializeField]
    private Transform parent;

    private Light light;
    private float initialRange;

    private void Start()
    {
        light = GetComponent<Light>();
        initialRange = light.range;
    }

    private void Update()
    {
        light.range = initialRange * parent.localScale.x;
    }
}
