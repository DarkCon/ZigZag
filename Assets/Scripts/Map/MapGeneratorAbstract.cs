using Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Map {
    public abstract class MapGeneratorAbstract : IMapGenerator {
        public int MinSegmentLen { get; set; }
        public int MaxSegmentLen { get; set; }
        public float SegmentWidth { get; set; }
        public int BonusPeriod { get; set; }
        public float MinBonusOffsetFromEdge { get; set; }

        private int _currDir;
        private int _currSegmentLen;

        public virtual void ResetState() {
            _currSegmentLen = 0;
            _currDir = Random.value < 0.5f 
                ? Direction.RIGHT 
                : Direction.FORWARD;
        }

        public Segment GenerateNextSegment() {
            if (_currSegmentLen <= 0) {
                _currDir = Direction.Next(_currDir);
                _currSegmentLen = Random.Range(MinSegmentLen, MaxSegmentLen);
            }

            var segment = new Segment {
                dir = _currDir,
                size = Vector2.one * SegmentWidth
            };
        
            GenerateBonus(ref segment);
            --_currSegmentLen;

            return segment;
        }

        protected abstract void GenerateBonus(ref Segment segment);
    
        protected static Vector2 GetRandomPosWithOffset(Vector2 size, float offset) {
            return new Vector2(
                Random.Range(offset, size.x - offset),
                Random.Range(offset, size.y - offset)
            );
        }
    }
}