using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PuzzleAnimations _animations;
    [SerializeField] private PuzzlePiece _piecePrefab;
    [SerializeField] private SpriteRenderer _boardBackground;
    [SerializeField] private Transform _piecesContainer;

    [Header("Settings")]
    [Range(1.0f, 1.5f)][SerializeField] private float _pieceSpacing = 1.05f;
    [Range(0.8f, 1f)][SerializeField] private float _boardMargin = 0.9f;

    public bool IsInputLocked { get; private set; } = true;
    public List<PuzzlePiece> Pieces { get; private set; } = new List<PuzzlePiece>();

    private PuzzleConfigSO _config;
    private List<PuzzlePiece> _piecePool = new List<PuzzlePiece>();

    private Vector2 _gridStep;
    private Vector2 _gridOrigin;
    private Vector2 _totalSize;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        SetupPool();
    }

    private void SetupPool()
    {
        _piecesContainer.localPosition = Vector3.zero;
        int maxPossiblePieces = PuzzleConfigSO.MaxGridDimension * PuzzleConfigSO.MaxGridDimension;

        for (int i = 0; i < maxPossiblePieces; i++)
        {
            PuzzlePiece piece = Instantiate(_piecePrefab, _piecesContainer);
            piece.name = $"Pool_Piece_{i}";
            piece.gameObject.SetActive(false);
            _piecePool.Add(piece);
        }
    }

    public void StartPuzzle()
    {
        _config = LevelManager.Instance.GetCurrentLevelConfig();

        if (_config.TargetImage == null) return;

        PuzzleInputManager.Instance.SetCurrentPuzzleConfig(_config);

        SetupPuzzleSequence().Forget();
    }

    private async UniTaskVoid SetupPuzzleSequence()
    {
        if (Pieces.Count > 0)
        {
            foreach (var p in Pieces)
            {
                _animations.KillAnimations(p.transform);
                p.ClearSprite();
                p.gameObject.SetActive(false);
            }
            Pieces.Clear();
        }

        await Resources.UnloadUnusedAssets();

        CalculateLayoutMetrics();

        GenerateGridAndAnimate();
        FitPuzzleToBoard();

        float animationDuration = (_config.Rows * 0.1f) + 0.5f;
        await UniTask.Delay((int)(animationDuration * 1000));

        await ShufflePieces();
    }

    private void CalculateLayoutMetrics()
    {
        Texture2D texture = _config.TargetImage;

        float pieceWorldW = (float)texture.width / _config.Columns / 100f;
        float pieceWorldH = (float)texture.height / _config.Rows / 100f;

        _gridStep = new Vector2(
            pieceWorldW * _pieceSpacing,
            pieceWorldH * _pieceSpacing
        );

        _totalSize = new Vector2(
            _gridStep.x * _config.Columns,
            _gridStep.y * _config.Rows
        );

        _gridOrigin = new Vector2(
            -(_totalSize.x * 0.5f) + (_gridStep.x * 0.5f),
            -(_totalSize.y * 0.5f) + (_gridStep.y * 0.5f)
        );
    }

    private void GenerateGridAndAnimate()
    {
        Texture2D texture = _config.TargetImage;
        int pxWidth = texture.width / _config.Columns;
        int pxHeight = texture.height / _config.Rows;

        int poolIndex = 0;

        for (int y = 0; y < _config.Rows; y++)
        {
            for (int x = 0; x < _config.Columns; x++)
            {
                if (poolIndex >= _piecePool.Count) return;

                PuzzlePiece piece = _piecePool[poolIndex];
                poolIndex++;

                piece.gameObject.SetActive(true);
                piece.name = $"Piece_{x}_{y}";

                Sprite newSprite = Sprite.Create(
                    texture,
                    new Rect(x * pxWidth, y * pxHeight, pxWidth, pxHeight),
                    new Vector2(0.5f, 0.5f),
                    100f
                );

                Vector2Int coord = new Vector2Int(x, y);
                piece.Init(newSprite, coord);

                Vector3 localPos = GetLocalPosition(coord);
                piece.SetPosition(coord, localPos, false);

                Pieces.Add(piece);

                _animations.PlaySpawnAnimation(piece.transform, x, y);
            }
        }
    }

    private void FitPuzzleToBoard()
    {
        if (_boardBackground == null) return;

        Vector3 boardSize = _boardBackground.bounds.size;

        float ratioX = boardSize.x / _totalSize.x;
        float ratioY = boardSize.y / _totalSize.y;

        float scaleFactor = Mathf.Min(ratioX, ratioY) * _boardMargin;

        _piecesContainer.localScale = Vector3.one * scaleFactor;

        _piecesContainer.position = _boardBackground.transform.position;
        _piecesContainer.position += Vector3.back * 0.1f;
    }

    private async UniTask ShufflePieces()
    {
        IsInputLocked = true;
        var correctCoords = Pieces.Select(p => p.CorrectCoord).ToList();
        var shuffledCoords = GenerateDerangement(correctCoords);

        for (int i = 0; i < Pieces.Count; i++)
        {
            Pieces[i].SetPosition(shuffledCoords[i], GetLocalPosition(shuffledCoords[i]), true);
        }
        await UniTask.Delay(600);
        IsInputLocked = false;
    }

    private List<Vector2Int> GenerateDerangement(List<Vector2Int> original)
    {
        int n = original.Count;
        if (n < 2) return new List<Vector2Int>(original);

        var result = new List<Vector2Int>(original);
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i);
            var temp = result[i];
            result[i] = result[j];
            result[j] = temp;
        }

        return result;
    }

    public PuzzlePiece GetPieceAtPosition(Vector3 worldPosition, PuzzlePiece ignorePiece)
    {
        float minDistance = float.MaxValue;
        PuzzlePiece closest = null;
        float threshold = 1.0f;

        foreach (var p in Pieces)
        {
            if (p == ignorePiece) continue;

            float dist = Vector3.Distance(worldPosition, p.transform.position);

            if (dist < minDistance && dist < threshold)
            {
                minDistance = dist;
                closest = p;
            }
        }
        return closest;
    }

    public void SwapPieces(PuzzlePiece p1, PuzzlePiece p2)
    {
        IsInputLocked = true;

        Vector2Int pos1 = p1.CurrentCoord;
        Vector2Int pos2 = p2.CurrentCoord;

        Vector3 targetPos1 = GetLocalPosition(pos2);
        Vector3 targetPos2 = GetLocalPosition(pos1);

        p1.SetPosition(pos2, targetPos1);
        p2.SetPosition(pos1, targetPos2);

        DOVirtual.DelayedCall(0.35f, () =>
        {
            IsInputLocked = false;
            CheckWinCondition();
        });
    }

    private void CheckWinCondition()
    {
        if (Pieces.All(p => p.IsInCorrectPosition))
        {
            IsInputLocked = true;
            _animations.PlayWinAnimation(Pieces, () =>
            {
                LevelManager.Instance.OnLevelCompleted?.Invoke();
            });
        }
    }

    private Vector3 GetLocalPosition(Vector2Int coord)
    {
        return new Vector3(
            _gridOrigin.x + (coord.x * _gridStep.x),
            _gridOrigin.y + (coord.y * _gridStep.y),
            0
        );
    }
}