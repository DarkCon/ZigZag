using Game;
using UnityEngine;

public class FallowTo : MonoBehaviour {
    [SerializeField] private Transform _target;

    private Vector3 _offset; 

    private void Awake() {
        _offset = transform.position - _target.position;
        GameEvent.OnChangeStage += OnGameStageChanged;
    }

    private void OnDestroy() {
        GameEvent.OnChangeStage -= OnGameStageChanged;
    }

    private void OnGameStageChanged() {
        this.enabled = GameEvent.Current == GameStage.READY || GameEvent.Current == GameStage.START;
    }

    private void LateUpdate() {
        transform.position = _target.position + _offset;
    }
}