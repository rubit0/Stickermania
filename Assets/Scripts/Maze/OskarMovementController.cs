using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class OskarMovementController : MonoBehaviour
{
    public UnityEvent onEnemyHit;
    public UnityEvent onEnemyExit;

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Transform initPosition;
    [SerializeField]
    private Transform raycastSource;
    [SerializeField]
    private float raycastDistance = 0.45f;
    [SerializeField]
    private float movementSpeed = 2.5f;
    [SerializeField]
    private LayerMask raycastFilter;

    private Transform mainCamera;
    private Transform instanceTransform;
    private Transform parentTransform;
    private bool enemyHit;

    private void Start()
    {
        instanceTransform = transform;
        parentTransform = instanceTransform.parent;
        mainCamera = Camera.main.transform;

        if(initPosition != null)
        {
            transform.localPosition = initPosition.localPosition;
            transform.rotation = initPosition.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            var obstacle = other.GetComponent<Obstacle>();
            if (obstacle.enabled)
            {
                enemyHit = true;
                onEnemyHit?.Invoke();
            }
        }
    }

    public void DelegateObstacleAwayEvent()
    {
        enemyHit = false;
        onEnemyExit?.Invoke();
    }

    private void Update()
    {
        if (enemyHit)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        if (!horizontal.Equals(0) && !vertical.Equals(0))
        {
            var targetRotation = new Vector3(0f, Mathf.Atan2(horizontal, vertical) * Mathf.Rad2Deg, 0f);
            var parentForwardNormalized = Vector3.Scale(parentTransform.forward.normalized, new Vector3(1, 0, 1)).normalized;
            var parentRotation = new Vector3(0f, Mathf.Atan2(parentForwardNormalized.x, parentForwardNormalized.z) * Mathf.Rad2Deg, 0f);
            var cameraYawAngle = new Vector3(0f, mainCamera.localRotation.eulerAngles.y, 0f);

            instanceTransform.localEulerAngles = (targetRotation - parentRotation) + cameraYawAngle;
        }
        if (!horizontal.Equals(0) || !vertical.Equals(0))
        {
            if (CanStepForward())
            {
                var speed = (movementSpeed * parentTransform.lossyScale.x) * Time.deltaTime;
                instanceTransform.Translate(Vector3.forward * speed, Space.Self);
            }
        }

        float joystickMagnitued = Mathf.Clamp01(new Vector2(horizontal, vertical).magnitude);
        animator.SetFloat("Speed", joystickMagnitued);
    }

    private bool CanStepForward()
    {
        return !Physics.Raycast(raycastSource.position, raycastSource.forward, raycastDistance * parentTransform.localScale.x, raycastFilter);
    }
}
