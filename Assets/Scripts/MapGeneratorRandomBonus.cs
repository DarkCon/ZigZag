using UnityEngine;

public class MapGeneratorRandomBonus : MapGeneratorAbstract {
    protected override void GenerateBonus(ref Segment segment) {
        segment.hasBonus = Random.Range(0, BonusPeriod) < 1;
        if (segment.hasBonus) {
            segment.bonusPos = GetRandomPosWithOffset(segment.size, MinBonusOffsetFromEdge);
        }
    }
}