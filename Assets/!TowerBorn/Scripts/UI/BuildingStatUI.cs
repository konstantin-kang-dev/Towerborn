using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingStatUI : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI textTMP;
    
    public void Init(CombatStatIconInfo combatIconInfo, string text)
    {
        image.sprite = combatIconInfo.icon;
        textTMP.text = text;
    }
}
