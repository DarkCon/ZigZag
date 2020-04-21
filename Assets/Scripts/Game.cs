using UnityEngine;

public class Game : MonoBehaviour {
    [SerializeField] private GameSettings _settings;
    [SerializeField] private MapView _mapView;
    [SerializeField] private Transform _ball;
    [SerializeField] private Camera _cam;

    private int _dir = Direction.RIGHT;
    private Vector2 _moveVector = Vector2.zero;
    
    private readonly CyclicArray<Segment> _map = new CyclicArray<Segment>();
    private MapGeneratorAbstract _generator;

    private bool _isFirstSegment;
    private Segment _currSegment;
    private Vector2 _ballPos;
    private bool _isGame;
    private float _distToTakeBonus;

    private void Awake() {
        if (_settings.BonusAlgorithm == GameSettings.BonusSpawnAlgorithm.BY_SEQUENCE)
            _generator = new MapGeneratorBonusBySequence();
        else
            _generator = new MapGeneratorRandomBonus();

        _generator.MinBonusOffsetFromEdge = _settings.MinBonusOffsetFromEdge;
        _generator.BonusPeriod = _settings.BonusPeriod;
    }

    private void Start() {
        StartGame();
        _mapView.Init(_cam, RequestNextSegment);
        _distToTakeBonus = (_ball.transform.localScale.z + _mapView.GetBonusSizeOnMap()) / 3f;
    }

    private void StartGame() {
        PrepareFirstSegment();
        _ballPos = _currSegment.size / 2f;
        _isGame = true;
    }

    private void StopGame() {
        _isGame = false;
    }

    private void PrepareFirstSegment() {
        _generator.ResetState();
        
        _generator.SegmentWidth = _settings.FirstTileSize;
        _generator.MinSegmentLen = 1;
        _generator.MaxSegmentLen = 1;
        
        _currSegment = _generator.GenerateNextSegment();
        
        _generator.SegmentWidth = _settings.TileWidth;
        _generator.MinSegmentLen = _settings.SegmentMinLen;
        _generator.MaxSegmentLen = _settings.SegmentMaxLen;
        
        _isFirstSegment = true;
    }

    private Segment RequestNextSegment() {
        if (_isFirstSegment) {
            _isFirstSegment = false;
            return _currSegment;
        }

        var segment = _generator.GenerateNextSegment(); 
        _map.Enqueue(segment);
        return segment;
    }

    private void Update() {
        if (!_isGame)
            return;

        UpdateInput();
        UpdateBall();
    }

    private void UpdateInput() {
        if (Input.GetMouseButtonDown(0)) {
            _dir = Direction.Next(_dir);
            _moveVector = GetMoveVector(_dir);
        }
    }

    private void UpdateBall() {
        _ballPos += _settings.BallSpeed * Time.deltaTime * _moveVector;
        if (_dir == Direction.RIGHT) {
            if (_ballPos.x > _currSegment.size.x) {
                GoToNextSegment();
            }
        } else if (_ballPos.y > _currSegment.size.y) {
            GoToNextSegment();
        }
        
        _ball.position = _mapView.MapSegmentPosToWorld(_map.Count, _ballPos) 
                         + _mapView.transform.up * _ball.localScale.y / 2f;

        if (_currSegment.hasBonus && Vector2.Distance(_ballPos, _currSegment.bonusPos) < _distToTakeBonus) {
            _mapView.TakeBonus(_map.Count);
        }
    }

    private void GoToNextSegment() {
        var oldSegment = _currSegment;
        _currSegment = _map.Dequeue();
        _ballPos -= oldSegment.size;
        var dir = Direction.Next(_currSegment.dir);
        _ballPos[dir] += _currSegment.size[dir];
        
        CheckGameOver();
    }

    private void CheckGameOver() {
        if (_ballPos.x < 0f || _ballPos.y < 0f
            || _ballPos.x > _currSegment.size.x || _ballPos.y > _currSegment.size.y) 
        {
            StopGame();
        }
    }

    private static Vector2 GetMoveVector(int dir) {
        if (dir == Direction.FORWARD) {
            return Vector2.up;
        }
        return Vector2.right;
    }
}