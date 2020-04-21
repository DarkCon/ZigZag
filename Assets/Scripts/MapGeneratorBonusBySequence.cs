public class MapGeneratorBonusBySequence : MapGeneratorAbstract {
    private int _tilesCounter;
    private int _tilesCounterMax;
    private int _tileWithBonus;
    
    public override void ResetState() {
        base.ResetState();
        
        _tilesCounter = -1;
        _tilesCounterMax = BonusPeriod * BonusPeriod - 1;
        _tileWithBonus = 0;
    }

    protected override void GenerateBonus(ref Segment segment) {
        if (_tilesCounter == _tileWithBonus) {
            _tileWithBonus += BonusPeriod + 1;
            segment.hasBonus = true;
            segment.bonusPos = GetRandomPosWithOffset(segment.size, MinBonusOffsetFromEdge);
        } else {
            segment.hasBonus = false;
        }

        ++_tilesCounter;
        if (_tilesCounter > _tilesCounterMax) {
            _tilesCounter = 0;
            _tileWithBonus = 0;
        }
    }
}