using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Resource : MonoBehaviour
{
    private Rigidbody _rigidbody;

    public event Action<Resource> IsCollected;

    [field: SerializeField] public int Value { get; private set; } = 1;
    [field: SerializeField] public bool IsMarkedForCollection { get; private set; } = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void MarkForCollection()
    {
        IsMarkedForCollection = true;
    }

    public void ResetParameters()
    {
        IsMarkedForCollection = false;
        transform.SetParent(null);
        _rigidbody.isKinematic = false;
    }

    public void MarkAsCollected()
    {
        IsCollected?.Invoke(this);
    }

    public void GetPicked(Transform parent)
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.isKinematic = true;

        transform.SetParent(parent);

        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }
}
