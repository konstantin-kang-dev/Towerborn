using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveInfoUI : MonoBehaviour
{
    SimpleShowHideAnimator showHideAnimator;

    [SerializeField] Button btn;
    [SerializeField] TextMeshProUGUI enemiesCountTMP;
    [SerializeField] TextMeshProUGUI rewardTMP;

    bool isActive = false;
    void Awake()
    {
        showHideAnimator = GetComponent<SimpleShowHideAnimator>();

        btn.onClick.AddListener(HandleButton);
    }

    void HandleButton()
    {
        isActive = !isActive;

        if(isActive)
        {
            showHideAnimator.Show();
        }
        else
        {
            showHideAnimator.Hide();
        }
    }

    public void SetInfo(int enemiesCount, int rewardAmount)
    {
        enemiesCountTMP.text = ProjectUtils.FormatNumber(enemiesCount);
        rewardTMP.text = ProjectUtils.FormatNumber(rewardAmount);
    }
}