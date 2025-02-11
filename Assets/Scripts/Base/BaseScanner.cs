using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseScanner : MonoBehaviour
{
    [SerializeField] private Base _core;
    [SerializeField] private float _searchFrequency = 1f;
    [SerializeField] private float _searchFieldRadius = 10f;

    private Coroutine _scanRoutine;
    private List<Resource> _assignedResources;

    public event Action<Bot, Resource> GivingOrder;

    private void Start()
    {
        _assignedResources = new List<Resource>();

        if (_scanRoutine != null)
        {
            StopCoroutine(_scanRoutine);
        }

        _scanRoutine = StartCoroutine(ScanRoutine());
    }

    private void ClearTakenResources()
    {
        _assignedResources = _assignedResources.Where(r => r.isActiveAndEnabled == true).ToList();
    }

    private List<Bot> CheckForAvailableBots()
    {
        List<Bot> availableBots = new List<Bot>();

        foreach (Bot bot in _core.Bots)
        {
            if (bot.TargetResource == null)
            {
                availableBots.Add(bot);
            }
        }

        return availableBots;
    }

    private Resource PickNearestResource()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _searchFieldRadius);

        List<Resource> availableResources = new List<Resource>();

        ClearTakenResources();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.TryGetComponent(out Resource resource) 
                && _assignedResources.Contains(resource) == false)
            {
                availableResources.Add(resource);
            }
        }

        if (availableResources.Count > 0)
        {
            Resource nearestResource = null;
            float minSqrDistance = Mathf.Infinity;

            foreach (Resource resource in availableResources)
            {
                float sqrDistanceToCenter = (transform.position - resource.transform.position).sqrMagnitude;

                if (sqrDistanceToCenter < minSqrDistance)
                {
                    minSqrDistance = sqrDistanceToCenter;
                    nearestResource = resource;
                }
            }

            return nearestResource;
        }

        return null;
    }

    private IEnumerator ScanRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(_searchFrequency);

        while (enabled)
        {
            yield return delay;

            List<Bot> availableBots = CheckForAvailableBots();

            if (availableBots.Count > 0)
            {
                foreach (Bot bot in availableBots)
                {
                    Resource foundResource = PickNearestResource();

                    if (foundResource != null)
                    {
                        GivingOrder?.Invoke(bot, foundResource);
                        _assignedResources.Add(foundResource);
                    }
                }
            }
        }
    }
}
