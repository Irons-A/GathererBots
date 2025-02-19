using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDispatcher : MonoBehaviour
{
    private List<Resource> _busyResources;

    private void Awake()
    {
        _busyResources = new List<Resource>();
    }

    public void AddBusyResource(Resource resource)
    {
        _busyResources.Add(resource);
    }

    public void RemoveBusyResource(Resource resource)
    {
        _busyResources.Remove(resource);
    }

    public bool CheckIsResourceBusy(Resource resource)
    {
        if (_busyResources.Contains(resource))
        {
            return true;
        }

        return false;
    }
}
