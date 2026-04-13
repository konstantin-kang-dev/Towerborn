using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityPopup : MonoBehaviour
{
    public static AbilityPopup Instance;

    SimpleShowHideAnimator animator;

    [Header("TMP's")]
    [SerializeField] TextMeshProUGUI titleTMP;
    [SerializeField] TextMeshProUGUI descrTMP;

    [Header("Image")]
    [SerializeField] Image image;

    [Header("Buttons")]
    [SerializeField] Button bgBtn;
    [SerializeField] Button continueBtn;

    private void Awake()
    {
        animator = GetComponent<SimpleShowHideAnimator>();

        bgBtn.onClick.AddListener(() => { SetVisibility(false); });
        continueBtn.onClick.AddListener(() => { SetVisibility(false); });

        Instance = this;
    }
    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void SetData(AbilityConfig config)
    {
        titleTMP.text = config.displayName;
        descrTMP.text = config.FormattedDescription;

        image.sprite = config.icon;
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
