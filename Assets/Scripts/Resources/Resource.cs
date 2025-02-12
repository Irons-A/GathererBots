using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Resource : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public Rigidbody Rigidbody => _rigidbody;

    public event Action<Resource> IsCollected;

    [field: SerializeField] public int Value { get; private set; } = 1;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void ResetParameters()
    {
        transform.SetParent(null);
        _rigidbody.isKinematic = false;
    }

    public void MarkAsCollected()
    {
        IsCollected?.Invoke(this);
    }
}
