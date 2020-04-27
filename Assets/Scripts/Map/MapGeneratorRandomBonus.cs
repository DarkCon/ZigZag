using Data;
using UnityEngine;

namespace Map {
    public class MapGeneratorRandomBonus : MapGeneratorAbstract {
        private bool _isFirstSegment;
        
        public override void ResetState() {
            base.ResetState();
            _isFirstSegment = true;
        }

        protected override void GenerateBonus(ref Segment segment) {
            if (_isFirstSegment) {
                _isFirstSegment = false;
            } else {
                segment.hasBonus = Random.Range(0, BonusPeriod) < 1;
                if (segment.hasBonus) {
                    segment.bonusPos = GetRandomPosWithOffset(segment.size, MinBonusOffsetFromEdge);
                }
            }
        }
    }
}