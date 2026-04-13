using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchCard : MonoBehaviour
{
    public BuildingConfig Config;

    [SerializeField] public Button unlockBtn;
    [SerializeField] public TextMeshProUGUI titleTMP;
    [SerializeField] public TextMeshProUGUI priceTMP;
    [SerializeField] public Image cardImage;

    [Header("Unlocked properties")]
    [SerializeField] public GameObject priceBlock;
    [SerializeField] public GameObject unlockedOverlay;
    [SerializeField] public GameObject unlockedText;

    public bool isUnlocked = false;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Init(BuildingConfig config)
    {
        Config = config;

        titleTMP.text = config.DisplayName;
        cardImage.sprite = config.CardSprite;
        priceTMP.text = ProjectUtils.FormatNumber(config.UnlockPrice);

        unlockBtn.onClick.AddListener(()=>
        {
            if (isUnlocked) return;

            bool isEnoughResource = ResourcesManager.CheckResourceForEnough(ResourceName.Gems, Config.UnlockPrice);

            if(!isEnoughResource) return;
            Unlock();
            ResourcesManager.SpendResource(ResourceName.Gems, Config.UnlockPrice);

            ResearchUI.Instance.Save();

            BuildingsDB.Instance.FilterUnlockedConfigs();
        });
    }

    public void UpdateCard()
    {
        if (isUnlocked) return;
        bool isEnoughResource = ResourcesManager.CheckResourceForEnough(ResourceName.Gems, Config.UnlockPrice);

        if (isEnoughResource)
        {
            priceTMP.color = Color.white;
        }
        else
        {
            priceTMP.color = Color.red;
        }
    }

    public void SetUnlockedState(bool state)
    {
        if (state)
        {
            Unlock();
        }
        else
        {
            Lock();
        }
    }

    public int GetPriority()
    {
        return (isUnlocked ? 10 : 0) + (int)Config.BuildingTier;
    }

    void Lock()
    {
        priceBlock.SetActive(true);
        unlockedText.SetActive(false);
        unlockedOverlay.SetActive(false);

        isUnlocked = false;
    }

    void Unlock()
    {
        priceBlock.SetActive(false);
        unlockedText.SetActive(true);
        unlockedOverlay.SetActive(true);

        isUnlocked = true;
    }
}
