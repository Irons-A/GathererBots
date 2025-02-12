using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private BaseScanner _scanner;

    [field: SerializeField] public List<Bot> Bots { get; private set; }

    private void Awake()
    {
        _scanner = GetComponentInChildren<BaseScanner>();
    }

    private void OnEnable()
    {
        _scanner.GivingOrder += GiveOrder;
    }

    private void OnDisable()
    {
        _scanner.GivingOrder -= GiveOrder;
    }

    private void Start()
    {
        foreach (Bot bot in Bots)
        {
            bot.SetBase(this);
        }
    }

    private void GiveOrder(Bot bot, Resource target)
    {
        bot.SetTargetResource(target);
    }
}
