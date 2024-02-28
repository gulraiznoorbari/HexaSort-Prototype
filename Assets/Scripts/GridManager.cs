using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Generation Info")]
    [SerializeField] private Cell _gridCellPrefab;
    [SerializeField] private LayerMask _gridCellLayer;
    [SerializeField] private int _rows = 7;
    [SerializeField] private int _columns = 7;

    private Dictionary<Vector3, bool> occupiedCells = new Dictionary<Vector3, bool>();

    public static GridManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        GameObject gridParent = new GameObject("Grid Cells");

        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                Vector3 cellPosition = new Vector3(i, 0, j);
                var gridCell = Instantiate(_gridCellPrefab, cellPosition, Quaternion.identity);
                gridCell.name = $"Cell_{i}x{j}";
                gridCell.transform.parent = gridParent.transform;

                var isOddPosition = (i % 2 != 0 && j % 2 == 0) || (i % 2 == 0 && j % 2 != 0);
                gridCell.ChangeColor(isOddPosition);

                // Mark the cell as initially unoccupied:
                occupiedCells.Add(cellPosition, false);
            }
        }
    }

    public void CheckNeighborsForColorMatch(Vector3 currentTilePosition)
    {
        // Check immediate neighbors (up, down, left, right) and diagonals:
        Vector3[] directions = {
            Vector3.forward, Vector3.back, Vector3.left, Vector3.right,
            new Vector3(1, 0, 1), new Vector3(1, 0, -1),
            new Vector3(-1, 0, 1), new Vector3(-1, 0, -1)
        };

        foreach (Vector3 direction in directions)
        {
            Vector3 neighborPosition = currentTilePosition + direction;
            bool isCellEmptyAndInbounds = IsCellEmptyAndInbounds(neighborPosition);
            Debug.Log(isCellEmptyAndInbounds);
            // If the neighboring cell is not empty and within bounds:
            if (!isCellEmptyAndInbounds)
            {
                Cell neighborCell = GetCellAtPosition(neighborPosition);
                if (neighborCell != null && neighborCell.transform.childCount > 0)
                {
                    Tile neighborTile = neighborCell.transform.GetChild(0).GetComponent<Tile>();
                    if (neighborTile != null && neighborTile.gameObject != gameObject)
                    {
                        // Check if the neighbor tile has the same color:
                        if (neighborTile.GetComponent<Renderer>().material.color == GetComponent<Renderer>().material.color)
                        {
                            Destroy(neighborTile.gameObject);
                            Destroy(gameObject);
                            return;
                        }
                    }
                }
            }
        }
    }

    public Cell GetCellAtPosition(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 10f, Vector3.down, out hit, 50f, _gridCellLayer))
        {
            return hit.collider.GetComponent<Cell>();
        }
        return null;
    }

    public void MarkCellOccupied(Vector3 position)
    {
        if (occupiedCells.ContainsKey(position))
        {
            occupiedCells[position] = true;
        }
    }

    public bool IsCellEmptyAndInbounds(Vector3 position)
    {
        // Check if the position is within the bounds of the grid:
        if (position.x < 0 || position.x >= _rows || position.z < 0 || position.z >= _columns)
        {
            return false;
        }

        // Check if the position exists in the dictionary, if not, consider it as empty:
        bool isCellEmpty;
        if (!occupiedCells.TryGetValue(position, out isCellEmpty))
        {
            return true;
        }
        return !isCellEmpty;
    }

}
