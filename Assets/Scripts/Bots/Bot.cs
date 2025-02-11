using UnityEngine;

[RequireComponent(typeof(BotMover))]
[RequireComponent(typeof(BotDistanceChecker))]
public class Bot : MonoBehaviour
{
    [SerializeField] private Transform _resourceAnchor;
    [SerializeField] private Base _assignedBase;

    private BotMover _mover;
    private BotDistanceChecker _distanceChecker;

    public bool IsCarryingResource { get; private set; } = false;
    public Resource TargetResource { get; private set; }

    private void Awake()
    {
        _mover = GetComponent<BotMover>();
        _distanceChecker = GetComponent<BotDistanceChecker>();
    }

    private void OnEnable()
    {
        _distanceChecker.TargetReached += PickUpResource;
    }

    private void OnDisable()
    {
        _distanceChecker.TargetReached -= PickUpResource;
    }

    private void Update()
    {
        if (TargetResource == null || TargetResource.isActiveAndEnabled == false)
        {
            IsCarryingResource = false;
            TargetResource = null;
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
        _distanceChecker.SetTarget(resource);
    }

    public void PickUpResource()
    {
        TargetResource.GetPicked(_resourceAnchor);

        IsCarryingResource = true;
        _mover.SetTarget(_assignedBase.transform);
    }
}
