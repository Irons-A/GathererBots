using System;
using UnityEngine;

public class BotDistanceChecker : MonoBehaviour
{
    [SerializeField] private float _pickupDistance = 1f;

    private Vector3 _offset;
    private float _sqrLength;
    private Resource _targetResource;

    public event Action TargetReached;

    private void Update()
    {
        if (_targetResource != null)
        {
            _offset = _targetResource.transform.position - transform.position;
            _sqrLength = _offset.sqrMagnitude;

            if (_sqrLength < _pickupDistance * _pickupDistance)
            {
                TargetReached?.Invoke();
                _targetResource = null;
            }
        }
    }

    public void SetTarget(Resource target)
    {
        _targetResource = target;
    }
}
