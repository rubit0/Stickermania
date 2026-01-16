using UnityEngine;

public class ObjectFollower : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private Transform target;
    [SerializeField]
    [Range(0f, 5f)]
    private float speed = 1.5f;
    [Header("Exclude Axis")]
    [SerializeField]
    private bool x;
    [SerializeField]
    private bool y = true;
    [SerializeField]
    private bool z;

    private Transform instanceTransform;
    private Vector3 initialPosition;

    private void Start()
    {
        instanceTransform = transform;
        initialPosition = instanceTransform.position;
    }

    private void Update()
    {
        instanceTransform.position = Vector3.Slerp(instanceTransform.position, GetTargetPosition(), Time.deltaTime * speed);
    }

    private Vector3 GetTargetPosition()
    {
        var targetPositionNormalized = target.position;
        if (x)
        {
            targetPositionNormalized.x = initialPosition.x;
        }
        if (y)
        {
            targetPositionNormalized.y = initialPosition.y;
        }
        if (z)
        {
            targetPositionNormalized.z = initialPosition.z;
        }

        return targetPositionNormalized;
    }
}
