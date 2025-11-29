using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private List<PuzzleConfigSO> _puzzleLevels;

    public UnityEvent OnLevelCompleted;
    public UnityEvent OnLevelStarted;

    private int _currentLevelIndex = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void StartLevel()
    {
        if (_puzzleLevels == null || _puzzleLevels.Count == 0) return;
        if (_currentLevelIndex >= _puzzleLevels.Count){_currentLevelIndex = 0;}
        OnLevelStarted?.Invoke();
    }
    public void IncreaseLevelIndex(){ _currentLevelIndex++; }
    public PuzzleConfigSO GetCurrentLevelConfig(){ return _puzzleLevels[_currentLevelIndex]; }
}
