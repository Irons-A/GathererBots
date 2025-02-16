using System;
using UnityEngine;

public class InputReader : MonoBehaviour
{
    public const int MouseSelectionButtonIndex = 0;
    public const int MouseDeselectionButtonIndex = 1;

    public event Action SelectionButtonPressed;
    public event Action DeselectionButtonPressed;

    private void Update()
    {
        if (Input.GetMouseButtonDown(MouseSelectionButtonIndex))
        {
            SelectionButtonPressed?.Invoke();
        }
        else if (Input.GetMouseButtonDown(MouseDeselectionButtonIndex))
        {
            DeselectionButtonPressed?.Invoke();
        }
    }
}
