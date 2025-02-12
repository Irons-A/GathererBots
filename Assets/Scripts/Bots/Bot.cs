using UnityEngine;

[RequireComponent(typeof(BotMover))]
[RequireComponent(typeof(BotDistanceChecker))]
public class Bot : MonoBehaviour
{
    [SerializeField] private Transform _resourceAnchor;
    [SerializeField] private Base _assignedBase;

    private BotMover _mover;
    private BotDistanceChecker _distanceChecker;

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
        TargetResource = resource;
        _mover.SetTarget(resource.transform);
        _distanceChecker.SetTarget(resource);
    }

    private void PickUpResource()
    {
        TargetResource.Rigidbody.velocity = Vector3.zero;
        TargetResource.Rigidbody.angularVelocity = Vector3.zero;
        TargetResource.Rigidbody.isKinematic = true;

        TargetResource.transform.SetParent(_resourceAnchor);

        TargetResource.transform.localPosition = Vector3.zero;
        TargetResource.transform.localEulerAngles = Vector3.zero;

        _mover.SetTarget(_assignedBase.transform);
    }
}
