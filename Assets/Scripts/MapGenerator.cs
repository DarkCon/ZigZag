using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator {
    public float MinSegmentLen = 1f;
    public float MaxSegmentLen = 5f;
    public float SegmentWidth = 1f;

    private int _nextDir;

    public MapGenerator() {
        _nextDir = Random.value < 0.5f 
            ? Direction.RIGHT 
            : Direction.FORWARD;
    }

    public MapGenerator(int firstDirection) {
        _nextDir = firstDirection;
    }

    public Segment GenerateNextSegment() {
        var dir = _nextDir;
            
        var size = Vector2.one * SegmentWidth;
        size[dir] = Random.Range(MinSegmentLen, MaxSegmentLen);
        
        _nextDir = Direction.Next(dir);
        
        return new Segment {
            dir = dir,
            size = size
        };
    }
}