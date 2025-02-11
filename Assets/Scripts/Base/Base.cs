using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [field: SerializeField] public List<Bot> Bots { get; private set; }

    private void Start()
    {
        foreach (Bot bot in Bots)
        {
            bot.SetBase(this);
        }
    }
}
