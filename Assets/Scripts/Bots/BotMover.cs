using UnityEngine;

public class BotMover : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;

    private Transform _target;

    private void Update()
    {
        if (_target != null)
        {
            transform.LookAt(_target);
            transform.position = Vector3.MoveTowards(transform.position, _target.position, _speed * Time.deltaTime);
        }
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void ClearTarget()
    {
        _target = null;
    }
}
