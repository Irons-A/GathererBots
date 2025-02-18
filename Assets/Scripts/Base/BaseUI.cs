using TMPro;
using UnityEngine;

public class BaseUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Base _base;

    private void OnEnable()
    {
        _base.ResourceValueChanged += UpdateText;
    }

    private void OnDisable()
    {
        _base.ResourceValueChanged -= UpdateText;
    }

    private void UpdateText(int resources)
    {
        _text.text = resources.ToString();
    }
}
