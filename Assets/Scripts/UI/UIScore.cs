using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class UIScore : MonoBehaviour {
        [SerializeField] private Text _txtScore;

        private void Awake() {
            Score.OnChange += OnScoreChange;
            OnScoreChange();
        }

        private void OnDestroy() {
            Score.OnChange -= OnScoreChange;
        }

        private void OnScoreChange() {
            _txtScore.text = Score.Value.ToString();
        }
    }
}