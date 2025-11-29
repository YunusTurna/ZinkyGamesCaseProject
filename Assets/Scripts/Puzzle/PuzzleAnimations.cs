using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;

public class PuzzleAnimations : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float _spawnDuration = 0.4f;
    [SerializeField] private Ease _spawnEase = Ease.OutBack;
    [SerializeField] private float _rowDelay = 0.1f;
    [SerializeField] private float _colDelay = 0.05f;

    [Header("Win Settings")]
    [SerializeField] private float _winPunchAmount = 0.2f;
    [SerializeField] private float _winPunchDuration = 0.5f;
    [SerializeField] private float _winDelayTillComplete = 3f;

    public void PlaySpawnAnimation(Transform target, int x, int y)
    {
        target.localScale = Vector3.zero;
        float delay = (y * _rowDelay) + (x * _colDelay);

        target.DOScale(Vector3.one, _spawnDuration)
              .SetDelay(delay)
              .SetEase(_spawnEase);
    }

    public void PlayWinAnimation(List<PuzzlePiece> pieces, Action onComplete = null)
    {
        if (pieces == null || pieces.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }
        Sequence winSequence = DOTween.Sequence();

        foreach (var p in pieces)
        {
            float startTime = p.CurrentCoord.y * _rowDelay;
            winSequence.Insert(startTime, p.transform.DOPunchScale(Vector3.one * _winPunchAmount, _winPunchDuration));
        }

        winSequence.AppendInterval(_winDelayTillComplete);

        winSequence.OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void KillAnimations(Transform target)
    {
        target.DOKill();
    }
}