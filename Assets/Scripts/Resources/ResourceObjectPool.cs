using System.Collections.Generic;
using UnityEngine;

public class ResourceObjectPool : MonoBehaviour
{
    [SerializeField] private Resource _prefab;

    private Queue<Resource> _pool = new Queue<Resource>();

    private void OnEnable()
    {
        foreach (Resource resource in _pool)
        {
            resource.IsCollected += ReturnResource;
        }
    }

    private void OnDisable()
    {
        foreach (Resource resource in _pool)
        {
            resource.IsCollected -= ReturnResource;
        }
    }

    public Resource GetResource()
    {
        if (_pool.Count == 0)
        {
            Resource resource = Instantiate(_prefab);
            resource.IsCollected += ReturnResource;

            return resource;
        }

        return _pool.Dequeue();
    }

    public void ReturnResource(Resource resource)
    {
        _pool.Enqueue(resource);
        resource.gameObject.SetActive(false);
        resource.ResetParameters();
    }
}
