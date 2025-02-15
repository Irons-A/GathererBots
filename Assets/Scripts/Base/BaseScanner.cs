using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BaseScanner : MonoBehaviour
{
    [SerializeField] private float _searchFieldRadius = 60f;

    public Resource PickNearestResource()
    {
        List<Resource> availableResources = ScanForResorces();

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

    private List<Resource> ScanForResorces()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _searchFieldRadius);

        List<Resource> availableResources = new List<Resource>();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.TryGetComponent(out Resource resource)
                && resource.IsAssignedToBot == false)
            {
                availableResources.Add(resource);
            }
        }

        return availableResources;
    }
}
