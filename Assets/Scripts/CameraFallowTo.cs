using Game;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFallowTo : MonoBehaviour {
    [SerializeField] private Transform _target;
    [SerializeField] private AnimationCurve _curveMagnetToCenter;

    private Vector3 _offset;
    private Camera _cam;

    private void Awake() {
        _cam = GetComponent<Camera>();
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
        var targetPos = _target.position;
        var camPosForCenter = targetPos + _offset;
        if (GameEvent.Current == GameStage.START) {
            var horizontalOffset = Vector3.Project(camPosForCenter - transform.position, transform.right);

            var viewPos = _cam.WorldToViewportPoint(targetPos);
            var horizontalMagnet = _curveMagnetToCenter.Evaluate(Mathf.Abs(viewPos.x - 0.5f) * 2f);

            transform.position = camPosForCenter - (1f - Time.deltaTime * horizontalMagnet) * horizontalOffset;
        } else {
            transform.position = camPosForCenter;
        }
    }
}