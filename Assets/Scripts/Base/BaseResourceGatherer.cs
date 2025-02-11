using TMPro;
using UnityEngine;

public class BaseResourceGatherer : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private int _resources = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Resource resource))
        {
            _resources += resource.Value;
            resource.MarkAsCollected();
            _text.text = _resources.ToString();
        }
    }
}
