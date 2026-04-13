using TMPro;
using UnityEngine;

public class VersionChecker : MonoBehaviour
{
    TextMeshProUGUI tmp;
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        tmp.text = $"v{Application.version}";
    }

    void Update()
    {
        
    }
}
