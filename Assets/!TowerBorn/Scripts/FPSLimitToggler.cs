using TMPro;
using UnityEngine;

using UnityEngine;
using UnityEngine.UI;

public class FPSLimitToggler : MonoBehaviour
{
    Button btn;
    TextMeshProUGUI tmp;

    void Start()
    {
        btn = GetComponent<Button>();
        tmp = GetComponentInChildren<TextMeshProUGUI>();

        btn.onClick.AddListener(Toggle);
    }

    bool toggleValue = false;
    public void Toggle()
    {
        toggleValue = !toggleValue;
        if(toggleValue)
        {
            Application.targetFrameRate = 120;
        }
        else
        {
            Application.targetFrameRate = 60;
        }

        tmp.text = $"FPS {Application.targetFrameRate} Max";
    }

}
