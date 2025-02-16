using System;
using UnityEngine;

public class Flag : MonoBehaviour, ITargetable
{
    public Vector3 Position => transform.position;

    public event Action<Bot, Flag> FlagReached;

    public void TriggerBaseBuilding(Bot bot)
    {
        FlagReached?.Invoke(bot, this);
        Destroy(gameObject);
    }
}
