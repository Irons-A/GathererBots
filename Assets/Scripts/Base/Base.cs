using System;
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

    [field: SerializeField] public BaseState CurrentState { get; private set; }
    public bool CanStartColonization => _bots.Count > _newBaseBotAmountRequired;

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
        CurrentState = BaseState.Gathering;

        _expectedResources = new List<Resource>();

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

    private void Update()
    {
        if (CurrentState == BaseState.Gathering && _resources >= _newBotPrice)
        {
            CreateNewBot();
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

    private void CreateNewBot()
    {
        _resources -= _newBotPrice;
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
        newResource.MarkAsCollected();
        UpdateText();
    }

    private void UpdateText()
    {
        _text.text = _resources.ToString();
    }

    private void GiveGatheringOrder(Bot bot, Resource target)
    {
        bot.SetTargetResource(target);
    }

    private void GiveColonizationOrder(Bot bot, Flag target)
    {
        bot.SetState(BotState.Colonization);
        bot.SetTargetFlag(target);
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
                    UpdateText();
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
        foreach (Bot bot in availableBots)
        {
            Resource nearestResource = _scanner.PickNearestResource();

            if (nearestResource != null)
            {
                GiveGatheringOrder(bot, nearestResource);
                nearestResource.MarkAsAssigned();
                _expectedResources.Add(nearestResource);
            }
        }
    }

    private Bot SelectColonizerBot(List<Bot> availableBots)
    {
        Bot selectedBot = availableBots[0];
        _bots.Remove(selectedBot);

        return selectedBot;
    }
}
