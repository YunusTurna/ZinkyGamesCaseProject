using UnityEngine;
using DG.Tweening;
using System;

public class PuzzleInputManager : MonoBehaviour
{
    public static PuzzleInputManager Instance { get; private set; }

    private PuzzleControls _controls;
    private PuzzlePiece _selectedPiece;
    private PuzzlePiece _draggedPiece;
    private Vector3 _startDragLocalPos;
    private Vector3 _dragOffset;
    
    [SerializeField] private Camera _camera;
    private Action<PuzzlePiece, Vector3> _onPieceClicked;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        if (_camera == null) { _camera = Camera.main; }
        
        _controls = new PuzzleControls();
        _controls.Gameplay.Click.started += ctx => OnPress();
        _controls.Gameplay.Click.canceled += ctx => OnRelease();
        _controls.Gameplay.Position.performed += ctx => OnMove(ctx.ReadValue<Vector2>());
    }
    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void OnPress()
    {
        if (PuzzleManager.Instance.IsInputLocked) return;

        Vector2 screenPos = _controls.Gameplay.Position.ReadValue<Vector2>();
        Vector3 worldPos = GetWorldPosition(screenPos);

        PuzzlePiece clickedPiece = FindPieceAtPosition(worldPos);

        if (clickedPiece != null)
        {
            _onPieceClicked?.Invoke(clickedPiece, worldPos);
        }
    }

    private void OnMove(Vector2 screenPos)
    {
        if (_draggedPiece != null)
        {
            Vector3 worldPos = GetWorldPosition(screenPos);

            Vector3 newPos = worldPos + _dragOffset;
            newPos.z = -1f;
            _draggedPiece.transform.position = newPos;
        }
    }

    private void OnRelease()
    {
        if (_draggedPiece != null)
        {
            FinishDragging();
        }
    }

    public void SetCurrentPuzzleConfig(PuzzleConfigSO puzzleConfig)
    {
        if (LevelManager.Instance == null) return;

        puzzleConfig = LevelManager.Instance.GetCurrentLevelConfig();

        if (puzzleConfig.PuzzleMode == PuzzleMode.DragAndDrop)
        {
            _onPieceClicked = StartDragging;
        }
        else if (puzzleConfig.PuzzleMode == PuzzleMode.ClickAndSwap)
        {
            _onPieceClicked = (piece, worldPos) => ProcessClickSelection(piece);
        }
    }

    #region Helper Methods

    private PuzzlePiece FindPieceAtPosition(Vector3 worldPos)
    {
        var pieces = PuzzleManager.Instance.Pieces;
        for (int i = pieces.Count - 1; i >= 0; i--)
        {
            if (pieces[i].IsPointInside(worldPos))
            {
                return pieces[i];
            }
        }
        return null;
    }

    private Vector3 GetWorldPosition(Vector2 screenPos)
    {
        Vector3 worldPos = _camera.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;
        return worldPos;
    }

    #endregion

    #region Interaction Logics

    private void StartDragging(PuzzlePiece piece, Vector3 worldMousePos)
    {
        piece.transform.DOComplete();
        _draggedPiece = piece;
        _startDragLocalPos = piece.transform.localPosition;
        _dragOffset = piece.transform.position - worldMousePos;
        _dragOffset.z = 0;
        piece.SetSortingOrder(100);
        piece.transform.DOScale(Vector3.one * 1.1f, 0.15f);
    }

    private void FinishDragging()
    {
        PuzzlePiece piece = _draggedPiece;
        piece.transform.DOScale(Vector3.one, 0.15f);
        piece.ResetSortingOrder();

        PuzzlePiece target = PuzzleManager.Instance.GetPieceAtPosition(piece.transform.position, piece);

        if (target != null)
        {
            PuzzleManager.Instance.SwapPieces(piece, target);
        }
        else
        {
            piece.transform.DOLocalMove(_startDragLocalPos, 0.2f);
        }

        _draggedPiece = null;
    }

    private void ProcessClickSelection(PuzzlePiece clickedPiece)
    {
        if (_selectedPiece == null)
        {
            _selectedPiece = clickedPiece;
            _selectedPiece.transform.DOPunchScale(Vector3.one * 0.15f, 0.2f);
            _selectedPiece.SetSortingOrder(10);
        }
        else if (_selectedPiece == clickedPiece)
        {
            _selectedPiece.ResetSortingOrder();
            _selectedPiece = null;
        }
        else
        {
            _selectedPiece.ResetSortingOrder();
            PuzzleManager.Instance.SwapPieces(_selectedPiece, clickedPiece);
            _selectedPiece = null;
        }
    }

    #endregion
}