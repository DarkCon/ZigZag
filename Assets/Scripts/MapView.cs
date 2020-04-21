using System;
using System.Collections.Generic;
using UnityEngine;

public class MapView : MonoBehaviour {
    [SerializeField] private MapViewSegment _segmentTpl;
    [SerializeField] private float _maxLength = 100f;
    [SerializeField] private float _camBorderTop = 1f;
    [SerializeField] private float _camBorderBottom = 0.1f;

    private Func<Segment> _requestNextSegment;
    private readonly Queue<MapViewSegment> _segmentsPool = new Queue<MapViewSegment>();
    private readonly CyclicArray<MapViewSegment> _segments = new CyclicArray<MapViewSegment>();

    private Vector2 _lastSegmentPosMap = Vector2.zero;
    private Vector2 _lastSegmentSizeMap = Vector2.zero;
    
    private Vector3 _lastSegmentPosWorld = Vector3.zero;
    private Vector3 _firstSegmentPosWorld = Vector3.zero;
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
        while (_cam.WorldToViewportPoint(_lastSegmentPosWorld).y < _camBorderTop 
               && Vector3.Distance(camPos, _lastSegmentPosWorld) < _maxLength) 
        {
            ExpandMap();
        }

        while (_segments.Count > 0 && _cam.WorldToViewportPoint(_firstSegmentPosWorld).y < _camBorderBottom) {
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

        segmentView.segment.localScale = SetLocalFromMap(segmentView.segment.localScale, size);
        segmentView.transform.localPosition = SetLocalFromMap(segmentView.transform.localPosition, pos);
        
        segmentView.bonus.gameObject.SetActive(segmentInfo.hasBonus);
        segmentView.bonus.localPosition = SetLocalFromMap(segmentView.bonus.localPosition, segmentInfo.bonusPos - size / 2f);
        
        _lastSegmentPosMap = pos;
        _lastSegmentSizeMap = size;
        _lastSegmentPosWorld = segmentView.segment.position;
        if (_segments.Count == 0) {
            _firstSegmentPosWorld = GetFarCornerOfSegment(segmentView);
        }
        
        _segments.Enqueue(segmentView);
    }

    private void TrimMap() {
        var segment = _segments.Dequeue();
        ReturnSegment(segment);
        if (_segments.Count > 0) {
            var firstSegment = _segments.Peek();
            _firstSegmentPosWorld = GetFarCornerOfSegment(firstSegment);
        }
    }

    private static Vector3 GetFarCornerOfSegment(MapViewSegment segment) {
        return segment.transform.TransformPoint(
            segment.segment.localPosition + segment.segment.localScale / 2f);
    }

    private MapViewSegment TakeSegment() {
        MapViewSegment segment;
        if (_segmentsPool.Count > 0) {
            segment = _segmentsPool.Dequeue();
        } else {
            segment = Instantiate(_segmentTpl, _segmentTpl.transform.parent, false);
        }
        
        segment.gameObject.SetActive(true);
        return segment;
    }

    private void ReturnSegment(MapViewSegment segment) {
        segment.gameObject.SetActive(false);
        _segmentsPool.Enqueue(segment);
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
            var scale = segmentView.segment.localScale;
            var local = segmentView.transform.localPosition 
                - new Vector3(scale.x, 0f, scale.z) / 2f 
                + LocalFromMap(segmentPos);
            return transform.TransformPoint(local);
        }
        return Vector3.zero;
    }

    public float GetBonusSizeOnMap() {
        return _segmentTpl.bonus.localScale.z;
    }

    public void TakeBonus(int segmentIdFromLast) {
        var segmentView = _segments.GetFromLast(segmentIdFromLast);
        segmentView.bonus.gameObject.SetActive(false);
    }
}