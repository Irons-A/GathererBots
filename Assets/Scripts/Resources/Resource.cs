using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Resource : MonoBehaviour
{
    private Rigidbody _rigidbody;

    public event Action<Resource> IsCollected;

    [field: SerializeField] public int Value { get; private set; } = 1;
    public bool IsAssignedToBot { get; private set; } = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

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
