using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseScanner : MonoBehaviour
{
    [SerializeField] private float _searchFieldRadius = 60f;

    public List<Resource> GetSortedResources()
    {
        List<Resource> availableResources = ScanForResorces();

        if (availableResources.Count > 0)
        {
            List<Resource> sortedResources = new List<Resource>();

            for (int i = 0; i < availableResources.Count; i++)
            {
                float minSqrDistance = Mathf.Infinity;
                Resource nearestResource = null;

                foreach (Resource resource in availableResources)
                {
                    float sqrDistanceToCenter = (transform.position - resource.transform.position).sqrMagnitude;

                    if (sqrDistanceToCenter < minSqrDistance)
                    {
                        minSqrDistance = sqrDistanceToCenter;
                        nearestResource = resource;
                    }
                }

                sortedResources.Add(nearestResource);

                availableResources = availableResources.Where(resource => resource != nearestResource).ToList();
            }


            return sortedResources;
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
