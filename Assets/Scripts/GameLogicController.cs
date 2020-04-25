using System.Collections;
using Data;
using Game;
using Map;
using UnityEngine;

public class GameLogicController : MonoBehaviour {
    [SerializeField] private Ball _ball;

    private CyclicArray<Segment> _map;
    private MapView _mapView; 

    private Segment _currSegment;
    private Vector2 _ballPos;
    private int _dir = Direction.RIGHT;
    private Vector2 _moveVector = Vector2.zero;
    private float _distToTakeBonus;

    private void Awake() {
        GameEvent.OnReady += OnGameReady;
    }

    private void OnDestroy() {
        GameEvent.OnReady -= OnGameReady;
    }

    private void OnGameReady() {
        Score.Value = 0;

        _map = MapManager.Instance.Map;
        _mapView = MapManager.Instance.View;
        _distToTakeBonus = (_ball.GetSize() + _mapView.GetBonusSizeOnMap()) / 3f;
        
        _ball.ResetAnimations();
        _ball.transform.position = _mapView.transform.up * _ball.GetSize() / 2f;

        StopCoroutine(nameof(UpdateGame));
        StartCoroutine(nameof(UpdateGame));
    }

    private IEnumerator UpdateGame() {
        while (!HandleInput()) {
            yield return null;
        }
        
        GameController.StartGame();
        _currSegment = _map.Dequeue();
        _ballPos = _currSegment.size / 2f;

        while (GameEvent.Current == GameStage.START) {
            HandleInput();
            UpdateBall();
            yield return null;
        }
    }

    private bool HandleInput() {
        if (Input.GetMouseButtonDown(0)) {
            _dir = Direction.Next(_dir);
            _moveVector = GetMoveVector(_dir);
            return true;
        }
        return false;
    }

    private void UpdateBall() {
        _ballPos += GameSettings.Current.BallSpeed * Time.deltaTime * _moveVector;
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
            TakeBonus();
            _currSegment.hasBonus = false;
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

    private void TakeBonus() {
        _mapView.TakeBonus(_map.Count);
        ++Score.Value;
    }

    private void CheckGameOver() {
        if (_ballPos.x < 0f || _ballPos.y < 0f
            || _ballPos.x > _currSegment.size.x || _ballPos.y > _currSegment.size.y) 
        {
            _ball.AnimateFall(MapView.LocalFromMap(_moveVector), GameSettings.Current.BallSpeed);
            GameController.StopGame();
        }
    }

    private static Vector2 GetMoveVector(int dir) {
        if (dir == Direction.FORWARD) {
            return Vector2.up;
        }
        return Vector2.right;
    }
}