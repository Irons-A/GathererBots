using System;
using System.Collections;
using UnityEngine;

public class BotDistanceChecker : MonoBehaviour
{
    [SerializeField] private float _pickupDistance = 1f;

    private Vector3 _offset;
    private float _sqrLength;
    private ITargetable _target;
    private Coroutine _distanceRoutine;

    public event Action ResourceReached;
    public event Action FlagReached;

    public void SetTarget(ITargetable target)
    {
        _target = target;

        if (_distanceRoutine != null)
        {
            StopCoroutine(DistanceRoutine());
        }

        _distanceRoutine = StartCoroutine(DistanceRoutine());
    }

    private IEnumerator DistanceRoutine()
    {
        while (_target != null)
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

            yield return null;
        }
    }
}