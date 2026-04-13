using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    AbilityConfig config;
    [SerializeField] TextMeshProUGUI titleTMP;
    [SerializeField] Image image;
    [SerializeField] Button button;
    void Start()
    {
        
    }
    
    public void Init(AbilityConfig abilityConfig)
    {
        this.config = abilityConfig;

        titleTMP.text = config.displayName;
        image.sprite = config.icon;

        button.onClick.AddListener(() =>
        {
            AbilityPopup.Instance.SetData(config);
            AbilityPopup.Instance.SetVisibility(true);
        });
    }
}
