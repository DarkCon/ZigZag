using UnityEngine;

public class FallowTo : MonoBehaviour {
    [SerializeField] private Transform _target;

    private Vector3 _offset; 

    private void Awake() {
        _offset = transform.position - _target.position;
    }

    private void LateUpdate() {
        transform.position = _target.position + _offset;
    }
}