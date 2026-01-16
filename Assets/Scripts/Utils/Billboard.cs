using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform camera;
    private Transform instanceTransform;

    private void OnEnable()
    {
        instanceTransform = transform;
        camera = Camera.main.transform;
    }

    private void LateUpdate()
    {
        instanceTransform.LookAt(instanceTransform.position + camera.transform.rotation * Vector3.forward,
            camera.transform.rotation * Vector3.up);
        var targetRotation = instanceTransform.rotation.eulerAngles;
        targetRotation.x = 0f;
        targetRotation.z = 0f;
        instanceTransform.eulerAngles = targetRotation;
    }
}
