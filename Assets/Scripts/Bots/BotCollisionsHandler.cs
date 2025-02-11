using UnityEngine;

public class BotCollisionsHandler : MonoBehaviour
{
    private Bot _core;

    private void Awake()
    {
        _core = GetComponentInParent<Bot>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_core.IsCarryingResource == false && TryGetComponent(out Resource resource) 
            && resource == _core.TargetResource)
        {
            _core.PickUpResource();
        }
    }
}
