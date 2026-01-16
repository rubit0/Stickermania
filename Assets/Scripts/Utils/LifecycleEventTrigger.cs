using UnityEngine;
using UnityEngine.Events;

public class LifecycleEventTrigger : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onEnableEvent;

    private void OnEnable()
    {
        onEnableEvent.Invoke();
    }
}
