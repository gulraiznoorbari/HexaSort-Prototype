using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Generation Info")]
    [SerializeField] private Cell _gridCellPrefab;
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

    private void Update()
    {

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
                gridCell.name = $"Tile_{i}x{j}";
                gridCell.transform.parent = gridParent.transform;

                var isOddPosition = (i % 2 != 0 && j % 2 == 0) || (i % 2 == 0 && j % 2 != 0);
                gridCell.ChangeColor(isOddPosition);

                // Mark the cell as initially unoccupied
                occupiedCells.Add(cellPosition, false);
            }
        }
    }

    public bool IsCellEmpty(Vector3 position)
    {
        // Check if the position is within the bounds of the grid
        if (position.x < 0 || position.x >= _rows || position.z < 0 || position.z >= _columns)
        {
            return false;
        }

        // Check if the position exists in the dictionary, if not, consider it as empty
        bool isCellEmpty;
        if (!occupiedCells.TryGetValue(position, out isCellEmpty))
        {
            return true;
        }
        return !isCellEmpty;
    }

}
