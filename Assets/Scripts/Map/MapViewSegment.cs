using System;
using System.Collections;
using UnityEngine;

namespace Map {
    public class MapViewSegment : MonoBehaviour {
        public Transform segment;
        public Transform bonus;
        
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private float _fallSpeed = 1f;
        [SerializeField] private float _fallHeight = 2f;

        public Action<MapViewSegment> OnHided;

        private void OnDisable() {
            if (_particleSystem.isPlaying)
                _particleSystem.Stop();
            StopAllCoroutines();

            var pos = transform.localPosition;
            pos.y = 0f;
            transform.localPosition = pos;
        }

        public void TakeBonus() {
            bonus.gameObject.SetActive(false);
            _particleSystem.transform.position = bonus.position;
            _particleSystem.Play();
        }

        public void Hide() {
            StartCoroutine(HideAnimation());
        }

        private IEnumerator HideAnimation() {
            var startY = transform.localPosition.y;
            float currY;
            
            do {
                var pos = transform.localPosition;
                pos.y -= _fallSpeed * Time.deltaTime;
                currY = pos.y;
                transform.localPosition = pos;
                yield return null;
            } while (startY - currY < _fallHeight);
            
            OnHided?.Invoke(this);
        }
    }
}