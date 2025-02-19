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
            List<Resource> sortedResources = availableResources.OrderBy(resource => 
            (transform.position - resource.transform.position).sqrMagnitude).ToList();

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
