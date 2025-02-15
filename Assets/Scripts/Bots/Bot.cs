using UnityEngine;

[RequireComponent(typeof(BotMover))]
[RequireComponent(typeof(BotDistanceChecker))]
public class Bot : MonoBehaviour
{
    [SerializeField] private Transform _resourceAnchor;
    [SerializeField] private Base _assignedBase;

    private BotMover _mover;
    private BotDistanceChecker _distanceChecker;
    private BotState _currentState;
    private Flag _targetFlag;

    public Resource TargetResource { get; private set; }

    private void Awake()
    {
        _mover = GetComponent<BotMover>();
        _distanceChecker = GetComponent<BotDistanceChecker>();
    }

    private void OnEnable()
    {
        _distanceChecker.ResourceReached += PickUpResource;
        _distanceChecker.FlagReached += ActivateFlag;
    }

    private void OnDisable()
    {
        _distanceChecker.ResourceReached -= PickUpResource;
        _distanceChecker.FlagReached -= ActivateFlag;
    }

    private void Update()
    {
        if (_currentState == BotState.Gathering)
        {
            if (TargetResource == null || TargetResource.isActiveAndEnabled == false)
            {
                TargetResource = null;
                _mover.ClearTarget();
            }
        }
    }

    public void SetState(BotState state)
    {
        _currentState = state;
        _distanceChecker.SetState(state);
    }

    public void SetBase(Base botBase)
    {
        _assignedBase = botBase;
    }

    public void SetTargetFlag(Flag flag)
    {
        _targetFlag = flag;
        _mover.SetTarget(flag.transform);
        _distanceChecker.SetTargetFlag(flag);
    }

    public void ClearTargetFlag()
    {
        _targetFlag = null;
        _mover.ClearTarget();
    }

    public void SetTargetResource(Resource resource)
    {
        TargetResource = resource;
        _mover.SetTarget(resource.transform);
        _distanceChecker.SetTargetResource(resource);
    }

    private void PickUpResource()
    {
        TargetResource.transform.SetParent(_resourceAnchor);

        TargetResource.transform.localPosition = Vector3.zero;
        TargetResource.transform.localEulerAngles = Vector3.zero;

        _mover.SetTarget(_assignedBase.transform);
    }

    private void ActivateFlag()
    {
        _targetFlag.BuildBase(this);
    }
}
