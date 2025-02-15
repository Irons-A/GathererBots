using System;
using UnityEngine;

public class BotDistanceChecker : MonoBehaviour
{
    [SerializeField] private float _pickupDistance = 1f;

    private Vector3 _offset;
    private float _sqrLength;
    private Resource _targetResource;
    private Flag _targetFlag;
    private BotState _botState;

    public event Action ResourceReached;
    public event Action FlagReached;

    private void Update()
    {
        if (_botState == BotState.Gathering)
        {
            MoveToResource();
        }
        else if (_botState == BotState.Colonization)
        {
            MoveToFlag();
        }
    }

    public void SetTargetResource(Resource target)
    {
        _targetResource = target;
    }

    public void SetTargetFlag(Flag target)
    {
        _targetFlag = target;
    }

    public void SetState(BotState state)
    {
        _botState = state;
    }

    private void MoveToResource()
    {
        if (_targetResource != null)
        {
            _offset = _targetResource.transform.position - transform.position;
            _sqrLength = _offset.sqrMagnitude;

            if (_sqrLength < _pickupDistance * _pickupDistance)
            {
                ResourceReached?.Invoke();
                _targetResource = null;
            }
        }
    }

    private void MoveToFlag()
    {
        if (_targetFlag != null)
        {
            _offset = _targetFlag.transform.position - transform.position;
            _sqrLength = _offset.sqrMagnitude;

            if (_sqrLength < _pickupDistance * _pickupDistance)
            {
                FlagReached?.Invoke();
                _targetFlag = null;
            }
        }
    }
}
