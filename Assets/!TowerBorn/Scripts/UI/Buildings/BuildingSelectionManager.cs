using UnityEngine;
using UnityEngine.UIElements;

public class BuildingSelectionManager : MonoBehaviour
{
    public static BuildingSelectionManager Instance {  get; private set; }


    public Building SelectedBuilding { get; private set; }
    public Building MergeableBuilding { get; private set; }

    public bool IsInitialized { get; private set; } = false;
    private void Awake()
    {
        
        Instance = this;
    }

    public void Init()
    {
        TouchSystem.Instance.OnTouchDown += (object sender, TouchEventArgs args) =>
        {
            //Debug.Log($"OnTouchDown hoveredBuilding: {args.HoveredBuilding}");
            Building building = args.HoveredBuilding;

            if (building == null) return;

            SelectBuilding(building);
        };
        TouchSystem.Instance.OnTouchUp += (object sender, TouchEventArgs args) =>
        {
            if(SelectedBuilding != null && MergeableBuilding != null)
            {
                BuildingsManager.Instance.MergeBuildings(SelectedBuilding, MergeableBuilding);
            }

            DeselectBuilding();

        };

        IsInitialized = true;
    }

    public void SelectBuilding(Building building)
    {
        if (building != null)
        {
            //Debug.Log($"Selected Building: {building.BuildingConfig.Id}");
            SelectedBuilding = building;
            SelectedBuilding.BuildingBoundaries.SetVisibility(true);
            BuildingPlacer.Instance.StartMovingBuilding(SelectedBuilding);
            BuildingsManager.Instance.ShowMergeableBuildings(SelectedBuilding);
            BuildingsManager.Instance.UpdateMergeArcLine(SelectedBuilding);
        }
    }

    public void DeselectBuilding()
    {
        if(SelectedBuilding == null) return;

        SelectedBuilding.BuildingBoundaries.SetVisibility(false);
        SelectedBuilding = null;
        BuildingPlacer.Instance.EndMovingBuilding();
        BuildingsManager.Instance.HideMergeableBuildings();
        BuildingsManager.Instance.HideMergeArcLine();
    }

    public void SetMergeableBuilding(Building building)
    {
        if(MergeableBuilding != null && MergeableBuilding != building)
        {
            ResetMergeableBuilding();
        }

        if(MergeableBuilding == null)
        {
            building.BuildingUI.SetMergeUIVisibility(true);
            building.BuildingVFXController.PlayMergeAvailableVFX();
        }

        MergeableBuilding = building;

    }
    public void ResetMergeableBuilding()
    {
        if (MergeableBuilding != null)
        {
            MergeableBuilding.BuildingUI.SetMergeUIVisibility(false);
            MergeableBuilding.BuildingVFXController.StopMergeAvailableVFX();
        }

        MergeableBuilding = null;
    }

}