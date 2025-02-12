using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private BaseScanner _scanner;
    [SerializeField] private BaseResourceGatherer _resourceGatherer;
    [SerializeField] private List<Bot> _bots;
    [SerializeField] private float _scanFrequency = 1f;
    [SerializeField] private TMP_Text _text;

    private int _resources = 0;
    private Coroutine _scanRoutine;
    private List<Resource> _expectedResources;

    private void Awake()
    {
        _scanner = GetComponentInChildren<BaseScanner>();
        _resourceGatherer = GetComponentInChildren<BaseResourceGatherer>();
    }

    private void OnEnable()
    {
        _resourceGatherer.ResourceDetected += VerifyExpectedResource;
    }

    private void OnDisable()
    {
        _resourceGatherer.ResourceDetected -= VerifyExpectedResource;
    }

    private void Start()
    {
        _expectedResources = new List<Resource>();

        foreach (Bot bot in _bots)
        {
            bot.SetBase(this);
        }

        if (_scanRoutine != null)
        {
            StopCoroutine(_scanRoutine);
        }

        _scanRoutine = StartCoroutine(ScanRoutine());
    }

    private List<Bot> SearchAvailableBots()
    {
        List<Bot> availableBots = new List<Bot>();

        foreach (Bot bot in _bots)
        {
            if (bot.TargetResource == null)
            {
                availableBots.Add(bot);
            }
        }

        return availableBots;
    }

    private void VerifyExpectedResource(Resource resource)
    {
        foreach (Resource expectedResource in _expectedResources)
        {
            if (expectedResource == resource)
            {
                CollectResource(resource);
                break;
            }
        }
    }

    private void CollectResource(Resource newResource)
    {
        _expectedResources = _expectedResources.Where(resource => resource != newResource).ToList();

        _resources += newResource.Value;
        newResource.MarkAsCollected();
        _text.text = _resources.ToString();
    }

    private void GiveOrder(Bot bot, Resource target)
    {
        bot.SetTargetResource(target);
    }

    private IEnumerator ScanRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(_scanFrequency);

        while (enabled)
        {
            yield return delay;

            List<Bot> availableBots = SearchAvailableBots();

            if (availableBots.Count > 0)
            {
                foreach (Bot bot in availableBots)
                {
                    Resource nearestResource = _scanner.PickNearestResource();

                    if (nearestResource != null)
                    {
                        GiveOrder(bot, nearestResource);
                        _scanner.AddAssignedResource(nearestResource);
                        _expectedResources.Add(nearestResource);
                    }
                }
            }
        }

    }
}
