public interface IMapGenerator {
    int MinSegmentLen { get; set; }
    int MaxSegmentLen { get; set; }
    float SegmentWidth { get; set; }
    int BonusPeriod { get; set; }
    float MinBonusOffsetFromEdge { get; set; }

    void ResetState();
    Segment GenerateNextSegment();
}