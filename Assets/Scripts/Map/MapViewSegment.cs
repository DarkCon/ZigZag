using UnityEngine;

namespace Map {
    public class MapViewSegment : MonoBehaviour {
        public Transform segment;
        public Transform bonus;
        
        [SerializeField] private ParticleSystem _particleSystem;

        public void TakeBonus() {
            bonus.gameObject.SetActive(false);
            _particleSystem.transform.position = bonus.position;
            _particleSystem.Play();
        }
    }
}