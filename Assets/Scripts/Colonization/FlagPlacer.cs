using System.Collections.Generic;
using UnityEngine;

public class FlagPlacer : MonoBehaviour
{
    public const int MouseSelectionButtonIndex = 0;
    public const int MouseDeselectionButtonIndex = 1;

    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private float _flagPlacementHeight = -1f;

    private Base _selectedBase;
    private Vector3 _flagPlacementPoint;
    private List<Base> _colonizingBases = new List<Base>();

    private void OnEnable()
    {
        foreach (Base botBase in _colonizingBases)
        {
            _selectedBase.ColonizationFinished += ClearColonizationTask;
        }
    }

    private void OnDisable()
    {
        foreach (Base botBase in _colonizingBases)
        {
            _selectedBase.ColonizationFinished -= ClearColonizationTask;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(MouseSelectionButtonIndex))
        {
            ProcessSelectedBase();
        }
        else if (Input.GetMouseButtonDown(MouseDeselectionButtonIndex))
        {
            NulifyBaseSelection();
        }
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
            }
            else if (_selectedBase.CurrentState == BaseState.Colonization)
            {
                TryGetPlaceableSurface();
                _selectedBase.UpdateFlagPosition(_flagPlacementPoint);
            }
        }
    }

    private bool TryPlaceFlag()
    {
        if (TryGetPlaceableSurface())
        {
            Flag flag = Instantiate(_flagPrefab, _flagPlacementPoint, Quaternion.identity);
            _selectedBase.StartColonization(flag);
            _selectedBase.UpdateFlagPosition(flag.transform.position);

            return true;
        }
        else
        {
            NulifyBaseSelection();

            return false;
        }
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
        if (botBase.CurrentState == BaseState.Gathering && botBase.CanStartColonization == true)
        {
            _selectedBase = botBase;
        }
        else if (botBase.CurrentState == BaseState.Colonization)
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

        if(_selectedBase == botBase)
        {
            _selectedBase = null;
        }
    }
}
