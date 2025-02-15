using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private float _spawnFreuency = 1f;
    [SerializeField] private int _resourceDepositCapacity = 25;
    [SerializeField] private ResourceObjectPool _pool;
    [SerializeField] private float _resourceSpawnHeight = 1f;

    private Coroutine _spawnRoutine;
    private BoxCollider _spawnField;
    private Vector3 _spawnFieldSize;

    private void Awake()
    {
        _spawnField = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        _spawnFieldSize = _spawnField.size;

        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
        }

        _spawnRoutine = StartCoroutine(ResourceSpawnRoutine());
    }

    private void Spawn()
    {
        Vector3 randomPositionAddition = new Vector3(Random.Range(0, _spawnFieldSize.x),
            _resourceSpawnHeight, Random.Range(0, _spawnFieldSize.z));

        Resource resource = _pool.GetResource();

        resource.gameObject.SetActive(true);
        resource.transform.position = _spawnField.transform.position + randomPositionAddition;
    }

    private IEnumerator ResourceSpawnRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(_spawnFreuency);

        while (_resourceDepositCapacity > 0)
        {
            yield return delay;

            Spawn();
            _resourceDepositCapacity--;
        }
    }
}
