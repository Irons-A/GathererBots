using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseScanner : MonoBehaviour
{
    [SerializeField] private float _searchFieldRadius = 10f;

    private List<Resource> _assignedResources;

    private void Start()
    {
        _assignedResources = new List<Resource>();
    }

    public Resource PickNearestResource()
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

    public void AddAssignedResource(Resource resource)
    {
        _assignedResources.Add(resource);
    }

    private void ClearTakenResources()
    {
        _assignedResources = _assignedResources.Where(resource => resource.isActiveAndEnabled).ToList();
    }
}
