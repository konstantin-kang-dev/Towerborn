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
    public void SetDevText(string text)
    {
        if (devText == null) return;
        if(!devText.isActiveAndEnabled) devText.gameObject.SetActive(true);

        devText.text = text;
    }
}