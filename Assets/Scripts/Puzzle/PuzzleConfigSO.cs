using UnityEngine;

public enum PuzzleMode
{
    DragAndDrop,   
    ClickAndSwap   
}

[CreateAssetMenu(fileName = "PuzzleConfig", menuName = "Puzzle/Config")]
public class PuzzleConfigSO : ScriptableObject
{
    public const int MaxGridDimension = 20; 
    public Texture2D TargetImage;
    [Range(2, MaxGridDimension)] public int Rows = 3;
    [Range(2, MaxGridDimension)] public int Columns = 3;

    [Header("Oynanış Ayarları")]
    public PuzzleMode PuzzleMode;
}