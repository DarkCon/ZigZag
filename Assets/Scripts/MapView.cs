using System;
using System.Collections.Generic;
using UnityEngine;

public class MapView : MonoBehaviour {
    [SerializeField] private Transform _segmentTpl;
    [SerializeField] private float _maxLength = 100f;
    [SerializeField] private float _camBorderTop = 1f;
    [SerializeField] private float _camBorderBottom = 0.1f;

    private Func<Segment> _requestNextSegment;
    private readonly Queue<Transform> _segmentsPool = new Queue<Transform>();
    private readonly CyclicArray<Transform> _segments = new CyclicArray<Transform>();

    private Vector2 _lastSegmentPosMap = Vector2.zero;
    private Vector2 _lastSegmentSizeMap = Vector2.zero;
    
    private Vector3 _lastSegmentPosLocal = Vector3.zero;
    private Vector3 _firstSegmentPosLocal = Vector3.zero;
    private Camera _cam;

    private void Awake() {
        _segmentTpl.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Init(Camera cam, Func<Segment> requestNextSegment) {
        _cam = cam;
        _requestNextSegment = requestNextSegment;
        gameObject.SetActive(true);
    }

    private void Update() {
        var camPos = transform.InverseTransformPoint(_cam.transform.position);
        while (LocalPosToCameraView(_lastSegmentPosLocal).y < _camBorderTop 
            && Vector3.Distance(camPos, _lastSegmentPosLocal) < _maxLength) 
        {
            ExpandMap();
        }

        while (_segments.Count > 0 && LocalPosToCameraView(_firstSegmentPosLocal).y < _camBorderBottom) {
            TrimMap();
        }
    }

    private void ExpandMap() {
        var segmentInfo = _requestNextSegment();
        var segmentView = TakeSegment();

        var size = segmentInfo.size;
        var offset = Vector2.zero;
        if (_segments.Count > 0) {
            offset = segmentInfo.dir == Direction.FORWARD
                ? new Vector2(-size.x, size.y)
                : new Vector2(size.x, -size.y);
        }
        var pos = _lastSegmentPosMap + (_lastSegmentSizeMap + offset) / 2f;

        segmentView.localScale = SetLocalFromMap(segmentView.localScale, size);
        segmentView.localPosition = _lastSegmentPosLocal = SetLocalFromMap(segmentView.localPosition, pos);
        _lastSegmentPosMap = pos;
        _lastSegmentSizeMap = size;
        
        _segments.Enqueue(segmentView);
    }

    private void TrimMap() {
        var segment = _segments.Dequeue();
        ReturnSegment(segment);
        if (_segments.Count > 0) {
            _firstSegmentPosLocal = _segments.Peek().localPosition;
        }
    }

    private Transform TakeSegment() {
        Transform segment;
        if (_segmentsPool.Count > 0) {
            segment = _segmentsPool.Dequeue();
        } else {
            segment = Instantiate(_segmentTpl, _segmentTpl.parent, false);
        }
        
        segment.gameObject.SetActive(true);
        return segment;
    }

    private void ReturnSegment(Transform segment) {
        segment.gameObject.SetActive(false);
        _segmentsPool.Enqueue(segment);
    }

    private Vector3 LocalPosToCameraView(Vector3 local) {
        var world = transform.TransformPoint(local);
        return _cam.WorldToViewportPoint(world);
    }

    private static Vector3 SetLocalFromMap(Vector3 local, Vector2 map) {
        return new Vector3(map.x, local.y, map.y);
    }
    
    private static Vector3 LocalFromMap(Vector2 map) {
        return new Vector3(map.x, 0f, map.y);
    }

    public Vector3 MapSegmentPosToWorld(int segmentIdFromLast, Vector2 segmentPos) {
        if (segmentIdFromLast < _segments.Count) {
            var segmentView = _segments.GetFromLast(segmentIdFromLast);
            var scale = segmentView.localScale;
            var local = segmentView.localPosition 
                - new Vector3(scale.x, -scale.y, scale.z) / 2f 
                + LocalFromMap(segmentPos);
            return transform.TransformPoint(local);
        }
        return Vector3.zero;
    }
}