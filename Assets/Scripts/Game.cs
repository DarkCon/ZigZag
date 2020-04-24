using UnityEngine;

public class Game : MonoBehaviour {
    [SerializeField] private GameSettings _settings;
    [SerializeField] private MapView _mapView;
    [SerializeField] private Ball _ball;
    [SerializeField] private Camera _cam;
    [Header("UI")] 
    [SerializeField] private GameObject _pnlChooseDifficult;

    private int _dir = Direction.RIGHT;
    private Vector2 _moveVector = Vector2.zero;
    
    private readonly CyclicArray<Segment> _map = new CyclicArray<Segment>();
    private IMapGenerator _generator;

    private Segment _currSegment;
    private Vector2 _ballPos;
    private bool _isGame;
    private float _distToTakeBonus;
    private FallowTo _camFallowTo;

    private void Awake() {
        _camFallowTo = _cam.GetComponent<FallowTo>();
        SetSettings(_settings);
    }

    public void SetSettings(GameSettings settings) {
        _settings = settings;
        if (_settings.BonusAlgorithm == GameSettings.BonusSpawnAlgorithm.BY_SEQUENCE)
            _generator = new MapGeneratorBonusBySequence();
        else
            _generator = new MapGeneratorRandomBonus();

        _generator.MinBonusOffsetFromEdge = _settings.MinBonusOffsetFromEdge;
        _generator.BonusPeriod = _settings.BonusPeriod;
    }

    public void OnChooseDifficult(GameSettings settings) {
        SetSettings(settings);
        _pnlChooseDifficult.SetActive(false);
        StartGame();
    }

    private void Start() {
        PrepareFirstSegment();
        _distToTakeBonus = (_ball.GetSize() + _mapView.GetBonusSizeOnMap()) / 3f;
    }

    private void StartGame() {
        PrepareFirstSegment();
        _mapView.enabled = true;
        _ballPos = _currSegment.size / 2f;
        _isGame = true;
        _camFallowTo.enabled = true;
    }

    private void StopGame() {
        _isGame = false;
        _camFallowTo.enabled = false;
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
        
        _mapView.Init(_cam, RequestNextSegment, _currSegment);
    }
 
    private Segment RequestNextSegment() {
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
        
        _ball.transform.position = _mapView.MapSegmentPosToWorld(_map.Count, _ballPos) 
                         + _mapView.transform.up * _ball.GetSize() / 2f;

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
            _ball.AnimateFall(MapView.LocalFromMap(_moveVector), _settings.BallSpeed);
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