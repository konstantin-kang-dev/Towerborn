using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class BuildingPlacer : MonoBehaviour
{
    public static BuildingPlacer Instance {  get; private set; }
    [field: SerializeField] public LayerMask BuildingLayer { get; private set; }
    [field: SerializeField] public LayerMask BuildingAreaLayer { get; private set; }
    public Building MovingBuilding { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {

    }

    public void StartMovingBuilding(Building building)
    {
        if (MovingBuilding != null) EndMovingBuilding();

        MovingBuilding = building;

        MovingBuilding.HandleStartMoving();
    }

    public void EndMovingBuilding()
    {

        if(MovingBuilding != null) MovingBuilding.HandlePlace();
        MovingBuilding = null;
    }

    public bool CompareMovingBuilding(Building building)
    {
        return MovingBuilding == building;
    }

    public bool IsMovingBuilding()
    {
        
        return MovingBuilding != null;
    }

    public void UpdateBuildingPos(Vector3 touchPos)
    {
        if(MovingBuilding == null) return;

        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        Vector3Int clickedCell = Vector3Int.zero;
        Vector3 clickedWorldPoint = Vector3.zero;
        if (Physics.Raycast(ray, out RaycastHit hit, 30f, LayerMask.GetMask("Plane")))
        {
#if UNITY_EDITOR
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red, 2f);
#endif

            clickedCell = GameGrid.Instance.FindNearestFreeCell(hit.point, MovingBuilding);
            clickedWorldPoint = hit.point;
        }

        Grid grid = GameGrid.Instance.grid;

        Building buildingInCell = GameGrid.Instance.GetBuildingAtCell(clickedCell);

        bool isMergeable = BuildingsManager.Instance.CompareBuildings(MovingBuilding, buildingInCell);

        if (isMergeable)
        {
            MovingBuilding.transform.position = buildingInCell.transform.position;

            BuildingSelectionManager.Instance.SetMergeableBuilding(buildingInCell);
            BuildingsManager.Instance.HideMergeArcLine();
        }
        else
        {
            GameGrid.Instance.MarkCellAsFree(MovingBuilding.PlacementCell);
            clickedCell = GameGrid.Instance.FindNearestFreeCell(clickedWorldPoint, null);

            Vector3 worldPosition = GameGrid.Instance.GetPositionFromCell(clickedCell);

            MovingBuilding.transform.position = worldPosition + GameGrid.Instance.CellPivotOffset;
            MovingBuilding.UpdatePlacePosition(clickedCell);
            GameGrid.Instance.MarkCellAsOccupied(clickedCell, MovingBuilding);

            BuildingSelectionManager.Instance.ResetMergeableBuilding();
            BuildingsManager.Instance.UpdateMergeArcLine(MovingBuilding);
        }
    }

    public Vector3 GetScreenCenterWorldPos()
    {
        Vector3 res = new Vector3();
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));

        if(Physics.Raycast(ray, out RaycastHit hit, 30f, LayerMask.GetMask("Plane")))
        {
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.blue, 2f);
            res = hit.point;
        }

        return res;
    }

    void Update()
    {

    }
}