using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private bool dragging, placed;
    private Camera cam;
    private Vector3 offset;
    private Vector3 originalPosition;
    private Vector3 tileSize;
    private Color currentTileColor;

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
        currentTileColor = gameObject.GetComponent<MeshRenderer>().material.color;
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
                Vector3? matchedPosition = GridManager.instance.CheckNeighborsForColorMatch(snappedPosition, currentTileColor);
                if (matchedPosition.HasValue)
                {
                    GridManager.instance.MarkCellUnOccupied(snappedPosition);
                    StartCoroutine(MoveToNeighborAndDestroy(matchedPosition.Value));
                }
            } 
            TilesSpawner.instance.RemoveFromAvailableTilesList(gameObject);
        }
        else
        {
            transform.position = originalPosition;
        }
    }

    private IEnumerator MoveToNeighborAndDestroy(Vector3 targetPosition)
    {
        float duration = 0.5f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(targetPosition.x, targetPosition.y + 0.5f, targetPosition.z);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // small delay when reached on above of matched tile:
        yield return new WaitForSeconds(0.2f);

        GameObject matchedTile = GridManager.instance.GetTileAtPosition(targetPosition);
        if (matchedTile != null) Destroy(matchedTile);
        Destroy(gameObject);
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
