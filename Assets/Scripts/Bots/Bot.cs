using UnityEngine;
using static UnityEditor.Progress;

[RequireComponent(typeof(BotMover))]
public class Bot : MonoBehaviour
{
    [SerializeField] private Transform _resourceAnchor;
    [SerializeField] private Base _assignedBase;

    private BotMover _mover;

    public bool IsCarryingResource { get; private set; } = false;
    public Resource TargetResource { get; private set; }

    private void Awake()
    {
        _mover = GetComponent<BotMover>();
    }

    private void Update()
    {
        if (TargetResource == null)
        {
            IsCarryingResource = false;
            _mover.ClearTarget();
        }
    }

    public void SetBase(Base botBase)
    {
        _assignedBase = botBase;
    }

    public void SetTargetResource(Resource resource)
    {
        IsCarryingResource = false;
        TargetResource = resource;
        _mover.SetTarget(resource.transform);
    }

    public void PickUpResource()
    {
        TargetResource.GetPicked(_resourceAnchor);

        IsCarryingResource = true;
        _mover.SetTarget(_assignedBase.transform);
    }
}
