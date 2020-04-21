using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ZigZag/GameSettings")]
public class GameSettings : ScriptableObject {
    [Serializable]
    public enum BonusSpawnAlgorithm {
        RANDOM,
        BY_SEQUENCE
    }
    
    [Header("Tiles")]
    [SerializeField] private float _tileWidth = 1f;
    [SerializeField] private int _segmentMinLen = 1;
    [SerializeField] private int _segmentMaxLen = 5;
    [SerializeField] private float _firstTileSize = 3f;
    [Header("Bonuses")]
    [SerializeField] private int _bonusPeriod = 5;
    [SerializeField] private float _minBonusOffsetFromEdge = 0.3f;
    [SerializeField] private BonusSpawnAlgorithm _bonusAlgorithm = BonusSpawnAlgorithm.BY_SEQUENCE;
    [Header("Ball")]
    [SerializeField] private float _ballSpeed = 1f;

    public float TileWidth => _tileWidth;
    public int SegmentMinLen => _segmentMinLen;
    public int SegmentMaxLen => _segmentMaxLen;
    public float FirstTileSize => _firstTileSize;
    public int BonusPeriod => _bonusPeriod;
    public float MinBonusOffsetFromEdge => _minBonusOffsetFromEdge;
    public BonusSpawnAlgorithm BonusAlgorithm => _bonusAlgorithm; 
    public float BallSpeed => _ballSpeed;
}