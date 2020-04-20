using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ZigZag/GameSettings")]
public class GameSettings : ScriptableObject {
    [SerializeField] private float _tileWidth = 1f;
    [SerializeField] private float _segmentMinLen = 1f;
    [SerializeField] private float _segmentMaxLen = 5f;
    [SerializeField] private float _firstTileSize = 3f;
    [SerializeField] private float _ballSpeed = 1f;

    public float TileWidth => _tileWidth;
    public float SegmentMinLen => _segmentMinLen;
    public float SegmentMaxLen => _segmentMaxLen;
    public float FirstTileSize => _firstTileSize;
    public float BallSpeed => _ballSpeed;
}