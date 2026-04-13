using TMPro;
using UnityEngine;

public class ResourcesUI : MonoBehaviour
{
    public static ResourcesUI Instance { get; private set; }

    [SerializeField] TextMeshProUGUI goldTMP;
    [SerializeField] TextMeshProUGUI gemsTMP;
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ResourcesManager.OnResourceUpdated += UpdateUI;
    }

    void Update()
    {
        
    }

    public void UpdateUI(Resource resource)
    {
        UpdateGoldUI();
        UpdateGemsUI();
    }

    void UpdateGoldUI()
    {
        goldTMP.text = ResourcesManager.GetResource(ResourceName.Gold).amount.ToString();
    }
    void UpdateGemsUI()
    {
        gemsTMP.text = ResourcesManager.GetResource(ResourceName.Gems).amount.ToString();
    }
}
