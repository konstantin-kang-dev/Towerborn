
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using TowerBorn.SaveSystem;
using System;
using Zenject;

public class BuildingsManager : MonoBehaviour
{
    public static BuildingsManager Instance { get; private set; }

    [Inject] private IInstantiator _instantiator;
    public List<IBuilding> Buildings { get; private set; }
    public int BuildingsQuantity => Buildings.Count;

    private ArcLineRenderer _arcLineRenderer;

    public event Action<Building> OnBuildingSpawn;
    public event Action OnBuildingsMerge;
    public event Action<IBuilding> OnBuildingAdded;
    public event Action<IBuilding> OnBuildingRemoved;

    public bool IsInitialized {  get; private set; } = false;
    void Awake()
    {
        Buildings = new List<IBuilding>();
        _arcLineRenderer = GetComponentInChildren<ArcLineRenderer>();

        OnBuildingAdded += HandleBuildingAdded;
        OnBuildingRemoved += HandleBuildingRemoved;

        Instance = this;
    }

    public void Init()
    {
        IsInitialized = true;
    }


    public Building SpawnBuilding(BuildingConfig config, Vector3 position)
    {
        Debug.Log($"SpawnBuilding: {config.DisplayName}");
        if (BuildingPlacer.Instance.IsMovingBuilding()) return null;
        BuildingConfig configClone = config.Clone();

        Building buildingComp = _instantiator.InstantiatePrefabForComponent<Building>(
            configClone.BuildingPrefab, position, Quaternion.identity, transform);

        Vector3Int cell = GameGrid.Instance.GetCellFromPosition(position);
        cell = GameGrid.Instance.FindNearestFreeCell(cell, null);

        Vector3 worldPosition = GameGrid.Instance.GetPositionFromCell(cell);
        buildingComp.transform.position = worldPosition + GameGrid.Instance.CellPivotOffset;
        buildingComp.UpdatePlacePosition(cell);
        GameGrid.Instance.MarkCellAsOccupied(cell, buildingComp);

        buildingComp.SetDataAndPlace(configClone);
        OnBuildingSpawn?.Invoke(buildingComp);

        return buildingComp;
    }

    void HandleBuildingAdded(IBuilding building)
    {
        if(GameGrid.Instance.GetFreeCellsCount() < ProjectConstants.MIN_FREE_GRID_CELLS)
        {
            DeckUI.Instance.SetCardsBlocker(true);
        }
        else
        {
            DeckUI.Instance.SetCardsBlocker(false);
        }
    }
    
    void HandleBuildingRemoved(IBuilding building)
    {
        if(GameGrid.Instance.GetFreeCellsCount() < ProjectConstants.MIN_FREE_GRID_CELLS)
        {
            DeckUI.Instance.SetCardsBlocker(true);
        }
        else
        {
            DeckUI.Instance.SetCardsBlocker(false);
        }
    }

    public void AddBuilding(IBuilding building)
    {
        if(Buildings.Contains(building)) return;

        Buildings.Add(building);
        OnBuildingAdded?.Invoke(building);
    }
    public void RemoveBuilding(IBuilding building)
    {
        if (!Buildings.Contains(building)) return;

        Buildings.Remove(building);
        OnBuildingRemoved?.Invoke(building);
    }

    public void DestroyAllBuildings()
    {
        foreach (IBuilding building in Buildings.ToList())
        {
            RemoveBuilding(building);
            building.DestroyBuilding();
        }
    }
    public IBuilding GetNearestBuilding(Vector3 position)
    {
        return Buildings.Count == 0 ? null :
               Buildings.OrderBy(b => Vector3.Distance(position, b.Position)).First();
    }
    public IBuilding GetNearestBuilding(Vector3 position, IBuilding excludeBuilding)
    {
        return Buildings.Where(b => b != excludeBuilding)
                       .OrderBy(b => Vector3.Distance(position, b.Position))
                       .FirstOrDefault();
    }

    public List<IBuilding> GetBuildingsInRadius(Vector3 position, float radius)
    {
        return Buildings.Where(b => Vector3.Distance(position, b.Position) <= radius).ToList();
    }

    public List<IBuilding> GetBuildingsInRadius(Vector3 position, float radius, IBuilding excludeBuilding)
    {
        return Buildings.Where(b => b != excludeBuilding &&
                                   Vector3.Distance(position, b.Position) <= radius)
                       .ToList();
    }

    public bool CompareBuildings(IBuilding buildingA, IBuilding buildingB)
    {
        if(buildingA == null || buildingB == null) return false;
        if(!buildingA.IsPlaced || !buildingB.IsPlaced) return false;

        bool isMaxLevel = buildingA.BuildingConfig.Level == ProjectConstants.MAX_BUILDING_LEVEL;
        bool isSameBuilding = buildingA == buildingB;
        bool isSameId = buildingA.BuildingConfig.Id == buildingB.BuildingConfig.Id;
        bool isSameLevel = buildingA.BuildingConfig.Level == buildingB.BuildingConfig.Level;

        return !isMaxLevel && !isSameBuilding && isSameId && isSameLevel;
    }

    public void MergeBuildings(Building buildingA, Building buildingB)
    {
        //Debug.Log($"Merge buildings a: {buildingA.gameObject.name} b: {buildingB.gameObject.name}");
        GameGrid.Instance.MarkCellAsFree(buildingA.PlacementCell);
        RemoveBuilding(buildingA);
        Destroy(buildingA.gameObject);

        buildingB.Upgrade();

        OnBuildingsMerge?.Invoke();

        BuildingSelectionManager.Instance.ResetMergeableBuilding();
    }

    public void ShowMergeableBuildings(IBuilding targetBuilding)
    {
        foreach (var building in Buildings)
        {
            if (building.InstanceId == targetBuilding.InstanceId || CompareBuildings(targetBuilding, building)) continue;

            building.BuildingTransparency.MakeTransparent();
        }

    }

    public void HideMergeableBuildings()
    {
        foreach (var building in Buildings)
        {
            building.BuildingTransparency.RestoreOriginalMaterials();
        }

    }
    public void UpdateMergeArcLine(IBuilding targetBuilding)
    {
        List<IBuilding> mergeableBuildings = Buildings.Where((x) =>
        {
            return x != targetBuilding && CompareBuildings(targetBuilding, x);
        }).ToList();

        mergeableBuildings.Sort((a, b) =>
        {
            float aDistance = Vector3.Distance(a.Position, targetBuilding.Position);
            float bDistance = Vector3.Distance(b.Position, targetBuilding.Position);
            return Mathf.RoundToInt(aDistance - bDistance);
        });

        if (mergeableBuildings.Count == 0) return;

        _arcLineRenderer.CreateArc(targetBuilding.Position, mergeableBuildings[0].Position);
    }
    public void HideMergeArcLine()
    {
        _arcLineRenderer.HideArc();
    }
    public void ApplySave()
    {

        if (!SavesManager.ProgressSave.isFirstLaunch)
        {

        }
        else
        {

        }
    }
}