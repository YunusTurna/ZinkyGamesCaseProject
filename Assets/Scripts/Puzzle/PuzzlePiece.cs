using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class PuzzlePiece : MonoBehaviour
{
    public Vector2Int CurrentCoord { get; private set; }
    public Vector2Int CorrectCoord { get; private set; }
    
    public bool IsInCorrectPosition => CurrentCoord == CorrectCoord;

    [SerializeField] private SpriteRenderer _renderer;
    private int _defaultSortingOrder;

    public void Init(Sprite sprite, Vector2Int correctCoord)
    {
        _renderer.sprite = sprite;
        CorrectCoord = correctCoord;
        _defaultSortingOrder = _renderer.sortingOrder;
    }

    public bool IsPointInside(Vector3 worldPoint)
    {
        Bounds bounds = _renderer.bounds;
        worldPoint.z = bounds.center.z;
        return bounds.Contains(worldPoint);
    }

    public void SetSortingOrder(int order)
    {
        _renderer.sortingOrder = order;
    }

    public void ResetSortingOrder()
    {
        _renderer.sortingOrder = _defaultSortingOrder;
    }
    public void ClearSprite()
    {
        _renderer.sprite = null;
    }

    public void SetPosition(Vector2Int newCoord, Vector3 localPos, bool animate = true)
    {
        CurrentCoord = newCoord;
        
        if (animate)
        {
            transform.DOLocalMove(localPos, 0.3f).SetEase(Ease.OutBack);
        }
        else
        {
            transform.localPosition = localPos;
        }
    }
}