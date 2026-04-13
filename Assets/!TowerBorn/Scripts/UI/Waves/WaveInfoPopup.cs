using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveInfoPopup : MonoBehaviour
{
    public static WaveInfoPopup Instance {get; private set;}
    SimpleShowHideAnimator animator;

    [SerializeField] TextMeshProUGUI titleTMP;
    [SerializeField] TextMeshProUGUI rewardTMP;
    [SerializeField] TextMeshProUGUI enemiesTMP;

    [SerializeField] Button bgBtn;
    [SerializeField] Button playBtn;

    void Awake()
    {
        animator = GetComponent<SimpleShowHideAnimator>();
        bgBtn.onClick.AddListener(() =>
        {
            SetVisibility(false);
        });
        playBtn.onClick.AddListener(() =>
        {
            WavesManager.Instance.StartWave();
            SetVisibility(false);
        });

        Instance = this;
    }
    
    public void SetInfo(WaveConfig config)
    {
        titleTMP.text = $"Wave {config.waveNumber}";
        rewardTMP.text = ProjectUtils.FormatNumber(config.rewardAmount);
        enemiesTMP.text = ProjectUtils.FormatNumber(config.GetTotalEnemies());
    }

    public void SetVisibility(bool value)
    {
        if (value)
        {
            animator.Show();
        }
        else
        {
            animator.Hide();
        }
    }

}
