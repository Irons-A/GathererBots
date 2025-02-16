using TMPro;
using UnityEngine;

public class BaseUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    public void UpdateText(int resources)
    {
        _text.text = resources.ToString();
    }
}
