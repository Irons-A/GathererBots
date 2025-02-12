using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private BaseScanner _scanner;
    [SerializeField] private List<Bot> Bots;

    private void Awake()
    {
        _scanner = GetComponentInChildren<BaseScanner>();
    }

    private void OnEnable()
    {
        _scanner.GaveOrder += GiveOrder;
    }

    private void OnDisable()
    {
        _scanner.GaveOrder -= GiveOrder;
    }

    private void Start()
    {
        foreach (Bot bot in Bots)
        {
            bot.SetBase(this);
        }
    }

    public List<Bot> CheckForAvailableBots()
    {
        List<Bot> availableBots = new List<Bot>();

        foreach (Bot bot in Bots)
        {
            if (bot.TargetResource == null)
            {
                availableBots.Add(bot);
            }
        }

        return availableBots;
    }

    private void GiveOrder(Bot bot, Resource target)
    {
        bot.SetTargetResource(target);
    }
}
