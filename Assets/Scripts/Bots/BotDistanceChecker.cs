using System;
using UnityEngine;

public class BotDistanceChecker : MonoBehaviour
{
    [SerializeField] private float _pickupDistance = 1f;

    private Vector3 _offset;
    private float _sqrLength;
    private ITargetable _target;

    public event Action ResourceReached;
    public event Action FlagReached;

    private void Update()
    {
        MoveToTarget();
    }

    public void SetTarget(ITargetable target)
    {
        _target = target;
    }

    private void MoveToTarget()
    {
        if (_target != null)
        {
            _offset = _target.Position - transform.position;
            _sqrLength = _offset.sqrMagnitude;

            if (_sqrLength < _pickupDistance * _pickupDistance)
            {
                if (_target is Resource)
                {
                    ResourceReached?.Invoke();
                }
                else if (_target is Flag)
                {
                    FlagReached?.Invoke();
                }

                _target = null;
            }
        }
    }
}