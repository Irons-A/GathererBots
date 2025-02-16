using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Resource : MonoBehaviour, ITargetable
{
    public event Action<Resource> IsCollected;

    [field: SerializeField] public int Value { get; private set; } = 1;
    public bool IsAssignedToBot { get; private set; } = false;
    public Vector3 Position => transform.position;

    public void ResetParameters()
    {
        transform.SetParent(null);
        IsAssignedToBot = false;
    }

    public void MarkAsAssigned()
    {
        IsAssignedToBot = true;
    }

    public void MarkAsCollected()
    {
        IsCollected?.Invoke(this);
    }
}
