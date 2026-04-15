using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GameGrid : MonoBehaviour
{
    public static GameGrid Instance { get; private set; }
    [Header("Grid Settings")]
    [SerializeField] public Grid grid;
    [SerializeField] public MeshRenderer planeRenderer;
    Material planeRendererMat;

    int gridWidth;
    int gridHeight;

    [SerializeField] bool showGizmos = true;
    Vector3 cellSize;
    [SerializeField] int cellsQuantity = 10;

    public int MaxBuildings => cellsQuantity * cellsQuantity;

    public Dictionary<Vector3Int, Building> gridCells = new Dictionary<Vector3Int, Building>();

    public Vector3 CellPivotOffset => new Vector3(grid.cellSize.x * 0.5f, 0, grid.cellSize.z * 0.5f);

    private void Awake()
    {
        Instance = this;

        planeRendererMat = planeRenderer.material;
        cellSize = grid.cellSize;
        Vector3 worldBounds = new Vector3(10f, 0f, 10f);

        ChangeGridSize(cellsQuantity);
    }

    private void Start()
    {

    }

    public Vector3Int GetCellFromPosition(Vector3 worldPosition)
    {
        if (grid == null)
        {
            Debug.LogError("Grid не найден!");
            return Vector3Int.zero;
        }

        return grid.WorldToCell(worldPosition);
    }

    public Vector3 GetPositionFromCell(Vector3Int cellPosition)
    {
        if (grid == null)
        {
            Debug.LogError("Grid не найден!");
            return Vector3Int.zero;
        }

        return grid.CellToWorld(cellPosition);
    }

    public void ChangeGridSize(int cellsAmount)
    {
        cellsQuantity = cellsAmount;
        gridWidth = cellsAmount;
        gridHeight = cellsAmount;

        planeRenderer.transform.localScale = new Vector3(cellsQuantity, planeRenderer.transform.localScale.y, cellsQuantity);
        planeRendererMat.SetVector("_BaseMap_ST", new Vector4(cellsQuantity / 2f, cellsQuantity / 2f, 0, 0));

        transform.position = new Vector3(-cellsAmount / 2f, 0, -cellsAmount / 2f);

        gridCells.Clear();

        Debug.Log($"MaxBuildings: {MaxBuildings}");
    }

    public void MarkCellAsOccupied(Vector3Int cell, Building building)
    {
        if (IsValidGridPosition(cell))
        {
            gridCells[cell] = building;
            //Debug.Log($"Ячейка {cell} отмечена как занятая зданием {building.name}");
        }
        else
        {
            //Debug.LogWarning($"Попытка отметить недопустимую ячейку: {cell}");
        }
    }

    public void MarkCellAsFree(Vector3Int cell)
    {
        if (gridCells.ContainsKey(cell))
        {
            //Debug.Log($"Ячейка {cell} освобождена от здания {gridCells[cell].name}");
            gridCells.Remove(cell);
        }
        else
        {
            //Debug.LogWarning($"Попытка освободить уже свободную ячейку: {cell}");
        }
    }

    public bool CheckIsCellOccupied(Vector3Int cell)
    {
        return gridCells.ContainsKey(cell);
    }

    public Building GetBuildingAtCell(Vector3Int cell)
    {
        return gridCells.TryGetValue(cell, out Building building) ? building : null;
    }

    public Vector3Int FindNearestFreeCell(Vector3 worldPosition, Building checkingBuilding)
    {
        Vector3Int bestCell = Vector3Int.zero;

        float bestDistance = float.MaxValue;
        bool found = false;
        int checkedCells = 0;
        int freeCells = 0;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3Int testCell = new Vector3Int(x, 0, z);
                checkedCells++;

                bool isOccupied = CheckIsCellOccupied(testCell);

                bool isMergeable = false;
                if (checkingBuilding != null && isOccupied)
                {
                    Building buildingInCell = GetBuildingAtCell(testCell);
                    isMergeable = BuildingsManager.Instance.CompareBuildings(checkingBuilding, buildingInCell);
                }

                if (!isOccupied || (isOccupied && isMergeable))
                {
                    freeCells++;
                    Vector3 cellWorldPos = grid.CellToWorld(testCell);
                    float distance = Vector3.Distance(worldPosition, cellWorldPos);
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        bestCell = testCell;
                        found = true;
                    }
                }
            }
        }

        return found ? bestCell : Vector3Int.zero;
    }

    private bool IsValidGridPosition(Vector3Int position)
    {
        return position.x >= 0 && position.x < gridWidth &&
               position.z >= 0 && position.z < gridHeight;
    }

    private Vector3Int ClampToGrid(Vector3Int position)
    {
        return new Vector3Int(
            Mathf.Clamp(position.x, 0, gridWidth - 1),
            position.y,
            Mathf.Clamp(position.z, 0, gridHeight - 1)
        );
    }

    public List<Vector3Int> GetOccupiedCells()
    {
        return new List<Vector3Int>(gridCells.Keys);
    }

    public int GetFreeCellsCount()
    {
        return MaxBuildings - gridCells.Count;
    }

    private void OnDrawGizmos()
    {
        if (grid == null)
            grid = GetComponent<Grid>();

        if (grid == null) return;

        if (!showGizmos) return;

        Gizmos.color = Color.green;
        Vector3 cellSize = grid.cellSize;

        for (int x = 0; x <= cellsQuantity; x++)
        {
            for (int z = 0; z <= cellsQuantity; z++)
            {
                Vector3 start = grid.transform.position + new Vector3(x * cellSize.x, 0, z * cellSize.z);
                Vector3 endX = start + new Vector3(cellSize.x, 0, 0);
                Vector3 endZ = start + new Vector3(0, 0, cellSize.z);

                if (x < cellsQuantity) Gizmos.DrawLine(start, endX);
                if (z < cellsQuantity) Gizmos.DrawLine(start, endZ);
            }
        }

        Gizmos.color = Color.red;
        foreach (var occupiedCell in gridCells.Keys)
        {
            Vector3 cellWorldPos = GetPositionFromCell(occupiedCell);
            Vector3 cellCenter = cellWorldPos + CellPivotOffset;
            Gizmos.DrawWireCube(cellCenter, new Vector3(cellSize.x * 0.8f, 0.1f, cellSize.z * 0.8f));
        }
    }
}