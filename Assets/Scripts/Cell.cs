using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private Color _evenPositionColor, _oddPositionColor;
    [SerializeField] private MeshRenderer _meshRenderer;

    public void ChangeColor(bool isOdd)
    {
        _meshRenderer.material.color = isOdd ? _oddPositionColor : _evenPositionColor;
    }
}
