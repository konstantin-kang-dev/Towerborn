using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityUI : MonoBehaviour
{
    [SerializeField] public GameObject levelBlock;
    [SerializeField] public TextMeshPro levelTMP;

    [SerializeField] public ProgressBar hpProgressBar;

    [SerializeField] public ProgressBar buildProgressBar;
    [SerializeField] public TextMeshPro buildTimeTMP;

    [SerializeField] public EarnBuildingButton earnBuildingBtn;

    [SerializeField] public TextMeshPro devText;
    private void Awake()
    {

    }
    public void SetHpProgress(float value)
    {
        hpProgressBar.SetProgressValue(value);
    }
    public void SetLevelValue(int level)
    {
        levelTMP.text = level.ToString();
    }
    public void SetBuildProgress(float value)
    {
        buildProgressBar.SetProgressValue(value);
    }
    public void SetBuildTimeText(string text)
    {
        buildTimeTMP.text = text;
    }
    public void SetEarnButton(EarnBuildingButtonState state)
    {
        earnBuildingBtn.SetState(state);
    }

    public void SetDevText(string text)
    {
        if (devText == null) return;
        if(!devText.isActiveAndEnabled) devText.gameObject.SetActive(true);

        devText.text = text;
    }
}