using UnityEngine;

public class Tile : MonoBehaviour
{
    private bool dragging, placed;
    private Camera camera;
    private Vector3 offset;
    private Vector3 originalPosition;
    private Vector3 tileSize;

    private void Start()
    {
        originalPosition = transform.position;
        tileSize = transform.localScale;
        camera = Camera.main;
    }

    private void FixedUpdate()
    {
        if (!dragging) return;
        transform.position = GetSnappedPosition();
        //transform.position = new Vector3(GetMousePosition().x - offset.x, 0.5f, GetMousePosition().z - offset.z);
    }

    private void OnMouseDown()
    {
        dragging = true;
        offset = GetMousePosition() - transform.position;
    }

    private void OnMouseUp()
    {
        dragging = false;
        //transform.position = originalPosition;
        SnapToGrid();
    }

    private Vector3 GetSnappedPosition()
    {
        Vector3 mousePosition = GetMousePosition();
        float snapX = Mathf.Round(mousePosition.x / tileSize.x) * tileSize.x;
        float snapZ = Mathf.Round(mousePosition.z / tileSize.z) * tileSize.z;
        return new Vector3(snapX, 0.4f, snapZ);
    }

    private void SnapToGrid()
    {
        Vector3 snappedPosition = GetSnappedPosition();
        if (GridManager.instance.IsCellEmpty(snappedPosition))
        {
            transform.position = snappedPosition;
            TilesSpawner.instance.RemoveFromAvailableTilesList(gameObject);
        }
        else
        {
            transform.position = originalPosition;
        }
    }

    private Vector3 GetMousePosition()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}
