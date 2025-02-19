using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Base : MonoBehaviour, ITargetable
{
    [SerializeField] private BaseScanner _scanner;
    [SerializeField] private BaseResourceGatherer _resourceGatherer;
    [SerializeField] private BaseDispatcher _dispatcher;
    [SerializeField] private List<Bot> _bots;
    [SerializeField] private float _scanFrequency = 1f;
    [SerializeField] private Bot _botPrefab;
    [SerializeField] private Transform _botSpawnPoint;
    [SerializeField] private int _newBotPrice = 3;
    [SerializeField] private int _newBaseBotAmountRequired = 1;
    [SerializeField] private int _newBasePrice = 5;

    private int _resources = 0;
    private Coroutine _scanRoutine;
    private List<Resource> _expectedResources;
    private Flag _flag;

    public event Action<Base> ColonizationFinished;
    public event Action<int> ResourceValueChanged;

    [field: SerializeField] public BaseState CurrentState { get; private set; }

    public bool CanStartColonization => _bots.Count > _newBaseBotAmountRequired;
    public Vector3 Position => transform.position;

    private void Awake()
    {
        _scanner = GetComponentInChildren<BaseScanner>();
        _resourceGatherer = GetComponentInChildren<BaseResourceGatherer>();
        _dispatcher = FindObjectOfType<BaseDispatcher>();
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
        CurrentState = BaseState.Gathering;

        _expectedResources = new List<Resource>();

        StartingConfiguration();
    }

    private void Update()
    {
        if (CurrentState == BaseState.Gathering && _resources >= _newBotPrice)
        {
            SpawnBot();
        }
    }

    public void AddBot(Bot bot)
    {
        _bots.Add(bot);
        bot.SetBase(this);
        bot.SetState(BotState.Gathering);
    }

    public void StartColonization(Flag flag)
    {
        CurrentState = BaseState.Colonization;
        _flag = flag;
    }

    public void UpdateFlagPosition(Vector3 newPosition)
    {
        _flag.transform.position = newPosition;
    }

    private void StartingConfiguration()
    {
        if (_bots.Count > 0)
        {
            foreach (Bot bot in _bots)
            {
                bot.SetBase(this);
                bot.SetState(BotState.Gathering);
            }
        }

        if (_scanRoutine != null)
        {
            StopCoroutine(_scanRoutine);
        }

        _scanRoutine = StartCoroutine(ScanRoutine());
    }

    private void SpawnBot()
    {
        _resources -= _newBotPrice;
        ResourceValueChanged?.Invoke(_resources);
        Bot newBot = Instantiate(_botPrefab, _botSpawnPoint.position, Quaternion.identity);
        _bots.Add(newBot);
        newBot.SetBase(this);
        newBot.SetState(BotState.Gathering);
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
        _dispatcher.RemoveBusyResource(newResource);
        newResource.MarkAsCollected();
        ResourceValueChanged?.Invoke(_resources);
    }

    private void GiveGatheringOrder(Bot bot, Resource target)
    {
        bot.SetTarget(target);
    }

    private void GiveColonizationOrder(Bot bot, Flag target)
    {
        bot.SetState(BotState.Colonization);
        bot.SetTargetFlag(target);
        bot.SetTarget(target);
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
                if (CurrentState == BaseState.Colonization && _resources >= _newBasePrice)
                {
                    GiveColonizationOrder(SelectColonizerBot(availableBots), _flag);
                    _resources -= _newBasePrice;
                    ResourceValueChanged?.Invoke(_resources);
                    CurrentState = BaseState.Gathering;

                    ColonizationFinished?.Invoke(this);
                }
                else
                {
                    AssignResources(availableBots);
                }
            }
        }
    }

    private void AssignResources(List<Bot> availableBots)
    {
        List<Resource> availableResources = _scanner.GetAvailableResources();

        if (availableResources != null)
        {
            availableResources = SortResources(availableResources);

            for (int i = 0; i < availableBots.Count; i++)
            {
                if (availableResources.Count > i)
                {
                    GiveGatheringOrder(availableBots[i], availableResources[i]);
                    _dispatcher.AddBusyResource(availableResources[i]);
                    _expectedResources.Add(availableResources[i]);
                }
            }
        }
    }

    private List<Resource> SortResources(List<Resource> availableResources)
    {
        availableResources = availableResources.Where(resource => _dispatcher.CheckIsResourceBusy(resource) == false).ToList();

        availableResources = availableResources.OrderBy(resource =>
            (transform.position - resource.transform.position).sqrMagnitude).ToList();

        return availableResources;
    }

    private Bot SelectColonizerBot(List<Bot> availableBots)
    {
        Bot selectedBot = availableBots[0];
        _bots.Remove(selectedBot);

        return selectedBot;
    }
}