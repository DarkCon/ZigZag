using Data;
using Game;
using UnityEngine;

namespace Map {
    public class MapManager : MonoBehaviour {
        [SerializeField] private MapView _mapView;
    
        public static MapManager Instance { get; private set; }

        public readonly CyclicArray<Segment> Map = new CyclicArray<Segment>();
        public MapView View => _mapView;

        private IMapGenerator _generator;

        private void Awake() {
            Instance = this;
            GameEvent.OnPrepare += ReInit;
            GameEvent.OnReady += ReInit;
            GameEvent.OnChangeStage += OnGameStageChange;
        }

        private void OnDestroy() {
            GameEvent.OnPrepare -= ReInit;
            GameEvent.OnReady -= ReInit;
            GameEvent.OnChangeStage -= OnGameStageChange;
        }

        private void OnGameStageChange() {
            _mapView.CanExpand = GameEvent.Current == GameStage.READY || GameEvent.Current == GameStage.START;
            _mapView.CanTrim = GameEvent.Current == GameStage.START;
        }

        private void ReInit() {
            var settings = GameSettings.Current;
            if (settings.BonusAlgorithm == GameSettings.BonusSpawnAlgorithm.BY_SEQUENCE)
                _generator = new MapGeneratorBonusBySequence();
            else
                _generator = new MapGeneratorRandomBonus();

            _generator.MinBonusOffsetFromEdge = settings.MinBonusOffsetFromEdge;
            _generator.BonusPeriod = settings.BonusPeriod;
        
            PrepareFirstSegment(settings);
        }

        private void PrepareFirstSegment(GameSettings settings) {
            Map.Clear();
            _generator.ResetState();
        
            _generator.SegmentWidth = settings.FirstTileSize;
            _generator.MinSegmentLen = 1;
            _generator.MaxSegmentLen = 1;
        
            var _firstSegment = RequestNextSegment();
        
            _generator.SegmentWidth = settings.TileWidth;
            _generator.MinSegmentLen = settings.SegmentMinLen;
            _generator.MaxSegmentLen = settings.SegmentMaxLen;
        
            _mapView.ReCreate(_firstSegment, RequestNextSegment);
        }

        private Segment RequestNextSegment() {
            var segment = _generator.GenerateNextSegment(); 
            Map.Enqueue(segment);
            return segment;
        }
    }
}