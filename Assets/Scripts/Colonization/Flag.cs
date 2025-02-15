using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour, ITargetable
{
    [SerializeField] private Base _basePrefab;
    [SerializeField] private float _baseBuildHeight = 0;

    public Vector3 Position => transform.position;

    public void BuildBase(Bot firstBot)
    {
        Base newBase = Instantiate(_basePrefab, new Vector3(transform.position.x, 
            _baseBuildHeight, transform.position.z), Quaternion.identity);
        newBase.AddBot(firstBot);
        firstBot.SetBase(newBase);
        firstBot.ClearTargetFlag();
        Destroy(gameObject);
    }
}
