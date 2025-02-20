using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Resource : MonoBehaviour, ITargetable
{
    public event Action<Resource> IsCollected;

    [field: SerializeField] public int Value { get; private set; } = 1;
    public Vector3 Position => transform.position;

    public void ResetParameters()
    {
        transform.SetParent(null);
    }

    public void MarkAsCollected()
    {
        IsCollected?.Invoke(this);
    }
}
