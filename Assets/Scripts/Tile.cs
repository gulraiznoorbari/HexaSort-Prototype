using UnityEngine;

public class Tile : MonoBehaviour
{
    private bool dragging, placed;
    private Camera cam;
    private Vector3 offset;
    private Vector3 originalPosition;
    private Vector3 tileSize;

    private void Start()
    {
        originalPosition = transform.position;
        tileSize = transform.localScale;
        cam = Camera.main;
    }

    private void FixedUpdate()
    {
        if (!dragging) return;
        transform.position = GetSnappedPosition();
    }

    private void OnMouseDown()
    {
        dragging = true;
        offset = GetMousePosition() - transform.position;
    }

    private void OnMouseUp()
    {
        dragging = false;
        SnapToGrid();
    }

    private Vector3 GetSnappedPosition()
    {
        Vector3 mousePosition = GetMousePosition();
        float snapX = Mathf.Round(mousePosition.x / tileSize.x) * tileSize.x;
        float snapZ = Mathf.Round(mousePosition.z / tileSize.z) * tileSize.z;
        return new Vector3(snapX, 0.27f, snapZ);
    }

    private void SnapToGrid()
    {
        if (placed) return;
        Vector3 snappedPosition = GetSnappedPosition();
        if (GridManager.instance.IsCellEmptyAndInbounds(snappedPosition))
        {
            transform.parent = null;
            transform.position = snappedPosition;
            Cell cell = GridManager.instance.GetCellAtPosition(snappedPosition);
            if (cell != null)
            {
                transform.parent = cell.transform;
                placed = true;
                GridManager.instance.MarkCellOccupied(snappedPosition);
                GridManager.instance.CheckNeighborsForColorMatch(snappedPosition);
            } 
            TilesSpawner.instance.RemoveFromAvailableTilesList(gameObject);
        }
        else
        {
            transform.position = originalPosition;
        }
    }

    private Vector3 GetMousePosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}
