using System.Collections.Generic;
using UnityEngine;

public class BuildingsPlacer : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private float _flagPlacementHeight = -1f;
    [SerializeField] private Base _basePrefab;
    [SerializeField] private float _baseBuildHeight = 0;

    private Base _selectedBase;
    private Vector3 _flagPlacementPoint;
    private List<Base> _colonizingBases = new List<Base>();

    private void OnEnable()
    {
        foreach (Base botBase in _colonizingBases)
        {
            _selectedBase.ColonizationFinished += ClearColonizationTask;
        }

        _inputReader.SelectionButtonPressed += ProcessSelectedBase;
        _inputReader.DeselectionButtonPressed += NulifyBaseSelection;
    }

    private void OnDisable()
    {
        foreach (Base botBase in _colonizingBases)
        {
            _selectedBase.ColonizationFinished -= ClearColonizationTask;
        }

        _inputReader.SelectionButtonPressed -= ProcessSelectedBase;
        _inputReader.DeselectionButtonPressed -= NulifyBaseSelection;
    }

    private void ProcessSelectedBase()
    {
        Base botBase = GetBaseClick();

        if (botBase != null)
        {
            PickBase(botBase);
        }
        else if (_selectedBase != null)
        {
            if (_selectedBase.CurrentState == BaseState.Gathering)
            {
                if (TryPlaceFlag())
                {
                    _colonizingBases.Add(_selectedBase);
                    _selectedBase.ColonizationFinished += ClearColonizationTask;
                }
                else
                {
                    NulifyBaseSelection();
                }
            }
            else if (_selectedBase.CurrentState == BaseState.Colonization)
            {
                TryGetPlaceableSurface();
                _selectedBase.UpdateFlagPosition(_flagPlacementPoint);
            }

            NulifyBaseSelection();
        }
    }

    private bool TryPlaceFlag()
    {
        if (TryGetPlaceableSurface())
        {
            Flag flag = Instantiate(_flagPrefab, _flagPlacementPoint, Quaternion.identity);
            flag.FlagReached += BuildBase;
            _selectedBase.StartColonization(flag);
            _selectedBase.UpdateFlagPosition(flag.transform.position);

            return true;
        }

        return false;
    }

    private bool TryGetPlaceableSurface()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.TryGetComponent(out Surface surface))
            {
                _flagPlacementPoint = new Vector3(hit.point.x, _flagPlacementHeight, hit.point.z);
                return true;
            }
        }

        return false;
    }

    private Base GetBaseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.TryGetComponent(out Base botBase))
            {
                return botBase;
            }
        }

        return null;
    }

    private void PickBase(Base botBase)
    {
        if ((botBase.CurrentState == BaseState.Gathering && botBase.CanStartColonization) 
            || botBase.CurrentState == BaseState.Colonization)
        {
            _selectedBase = botBase;
        }
    }

    private void NulifyBaseSelection()
    {
        _selectedBase = null;
    }

    private void ClearColonizationTask(Base botBase)
    {
        _colonizingBases.Remove(botBase);
        botBase.ColonizationFinished -= ClearColonizationTask;

        if (_selectedBase == botBase)
        {
            _selectedBase = null;
        }
    }

    private void BuildBase(Bot firstBot, Flag flag)
    {
        Base newBase = Instantiate(_basePrefab, new Vector3(flag.transform.position.x,
            _baseBuildHeight, flag.transform.position.z), Quaternion.identity);
        newBase.AddBot(firstBot);
        firstBot.SetBase(newBase);
        firstBot.ClearTarget();
    }
}