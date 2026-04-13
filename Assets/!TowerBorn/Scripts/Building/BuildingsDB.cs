using System.Collections.Generic;
using System.Linq;
using TowerBorn.SaveSystem;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingsDB : MonoBehaviour
{
    public static BuildingsDB Instance { get; private set; }
    string configsPath = "BuildingsConfigs/";
    Dictionary<string, BuildingConfig> buildingsConfigs = new Dictionary<string, BuildingConfig>();
    public Dictionary<string, BuildingConfig> AllConfigs => buildingsConfigs;
    public Dictionary<string, BuildingConfig> UnlockedConfigs = new Dictionary<string, BuildingConfig>();

    [SerializeField] List<BuildingConfig> defaultConfigs = new List<BuildingConfig>();
    public bool IsInitialized { get; private set; } = false;
    void Awake()
    {
        Instance = this;
        LoadConfigs();
    }

    public void Init()
    {
        IsInitialized = true;
    }

    void LoadConfigs()
    {
        BuildingConfig[] configs = Resources.LoadAll<BuildingConfig>(configsPath);

        foreach (BuildingConfig buildingConfig in configs)
        {
            BuildingConfig copy = buildingConfig.Clone();
            buildingsConfigs.Add(copy.Id, copy);
        }
    }

    public BuildingConfig GetConfigById(string id)
    {
        if (AllConfigs.ContainsKey(id))
        {
            return AllConfigs[id];
        }
        else
        {
            return null;
        }
    }

    public void UnlockDefaultCards()
    {
        ResearchUI.Instance.UnlockCards(defaultConfigs);
        FilterUnlockedConfigs();
    }

    public void FilterUnlockedConfigs()
    {
        List<BuildingConfig> configs = buildingsConfigs.Values.Select((x) => x.Clone()).ToList();

        UnlockedConfigs.Clear();

        foreach (var save in SavesManager.ProgressSave.researchList)
        {
            if (!save.isUnlocked) continue;
            BuildingConfig config = configs.First((x) => x.Id == save.id);
            
            UnlockedConfigs.Add(config.Id, config);
        }
    }

    public BuildingConfig GetRandomUnlockedConfig()
    {
        int randomKey = Random.Range(0, UnlockedConfigs.Count);

        List<BuildingConfig> configs = UnlockedConfigs.Values.ToList();

        return configs[randomKey];
    }
    public BuildingConfig GetRandomUnlockedConfig(BuildingTier targetTier)
    {
        List<BuildingConfig> filteredConfigs = UnlockedConfigs.Values.Where((x) => x.BuildingTier == targetTier).ToList();

        if(filteredConfigs.Count != 0)
        {
            int randomKey = Random.Range(0, filteredConfigs.Count);

            return filteredConfigs[randomKey];
        }
        else
        {
            List<BuildingConfig> tempList = UnlockedConfigs.Values.ToList();
            int randomKey = Random.Range(0, tempList.Count);

            return tempList[randomKey];
        }
    }

    void Update()
    {

    }
}
