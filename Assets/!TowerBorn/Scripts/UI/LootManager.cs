using UnityEngine;

public class LootManager : MonoBehaviour
{
    public static LootManager Instance {get; private set;}
    [SerializeField] RewardTarget goldTarget;
    [SerializeField] RewardTarget gemsTarget;
    [SerializeField] GameObject lootGoldPrefab;
    [SerializeField] GameObject lootGemsPrefab;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        
    }
    
    public void HandleSpawnLoot(ResourceName resourceName, int value, Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        SpawnLoot(resourceName, value, screenPos);
    }
    public void HandleSpawnLoot(ResourceName resourceName, int value, Vector2 screenPos)
    {
        SpawnLoot(resourceName, value, screenPos);
    }

    private void SpawnLoot(ResourceName resourceName, int value, Vector2 screenPos)
    {
        RewardTarget rewardTarget = goldTarget;
        GameObject lootPrefab = lootGoldPrefab;

        switch (resourceName)
        {
            case ResourceName.Gold:
                rewardTarget = goldTarget;
                lootPrefab = lootGoldPrefab;
                break;
            case ResourceName.Gems:
                rewardTarget = gemsTarget;
                lootPrefab = lootGemsPrefab;
                break;
        }

        GameObject lootGO = Instantiate(lootPrefab, transform);
        lootGO.transform.position = screenPos;
        LootObject lootComp = lootGO.GetComponent<LootObject>();
        lootComp.Init(rewardTarget, value);
    }
}
