using System.Collections;
using UnityEngine;

public class BotMover : MonoBehaviour
{
    [SerializeField] private float _speed = 33f;

    private ITargetable _target;
    private Coroutine _movementRoutine;

    public void SetTarget(ITargetable target)
    {
        _target = target;

        StopExistingCoroutine(_movementRoutine);

        _movementRoutine = StartCoroutine(MovementRoutine());
    }

    public void ClearTarget()
    {
        StopExistingCoroutine(_movementRoutine);

        _target = null;
    }

    private IEnumerator MovementRoutine()
    {
        while (enabled)
        {
            transform.LookAt(_target.Position);
            transform.position = Vector3.MoveTowards(transform.position, _target.Position, _speed * Time.deltaTime);

            yield return null;
        }
    }

    private void StopExistingCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }
}