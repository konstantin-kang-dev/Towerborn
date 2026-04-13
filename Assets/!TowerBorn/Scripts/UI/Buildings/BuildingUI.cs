using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    [SerializeField] public GameObject levelBlock;
    [SerializeField] public TextMeshPro levelTMP;

    [SerializeField] List<GameObject> levelTiersBackgrounds = new List<GameObject>();

    [Header("Merge UI")]
    [SerializeField] SimpleShowHideAnimator mergeUIAnimator;
    [SerializeField] ParticleSystem mergeUIVFX;

    [SerializeField] public TextMeshPro devText;
    private void Awake()
    {

    }
    public void SetLevelBackgroundTier(int tier)
    {
        foreach (var tierBg in levelTiersBackgrounds)
        {
            tierBg.SetActive(false);
        }

        levelTiersBackgrounds[tier].SetActive(true);
    }
    public void SetLevelValue(int level)
    {
        levelTMP.text = level.ToString();
    }

    public void SetMergeUIVisibility(bool value)
    {
        if (value)
        {
            mergeUIAnimator.Show();
            mergeUIVFX.gameObject.SetActive(true);
            mergeUIVFX.Play();
        }
        else
        {
            mergeUIAnimator.Hide();
            mergeUIVFX.Stop();
        }
    }

    public void SetDevText(string text)
    {
        if (devText == null) return;
        if (!devText.isActiveAndEnabled) devText.gameObject.SetActive(true);

        devText.text = text;
    }
}