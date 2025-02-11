using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScanner : MonoBehaviour
{
    [SerializeField] private Base _core;
    [SerializeField] private float _searchFrequency = 1f;
    [SerializeField] private float _searchFieldRadius = 10f;

    private Coroutine _scanRoutine;

    private void Start()
    {
        if (_scanRoutine != null)
        {
            StopCoroutine(_scanRoutine);
        }

        _scanRoutine = StartCoroutine(ScanRoutine());
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

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.TryGetComponent(out Resource resource) && resource.IsMarkedForCollection == false)
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
                        bot.SetTargetResource(foundResource);
                    }
                }
            }
        }
    }
}
