using UnityEngine;

public class BotMover : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;

    private ITargetable _target;

    private void Update()
    {
        if (_target != null)
        {
            transform.LookAt(_target.Position);
            transform.position = Vector3.MoveTowards(transform.position, _target.Position, _speed * Time.deltaTime);
        }
    }

    public void SetTarget(ITargetable target)
    {
        _target = target;
    }

    public void ClearTarget()
    {
        _target = null;
    }
}
