using System;
using System.Linq;
using Data;
using Game;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class UIChooseDifficult : MonoBehaviour {
        [Serializable]
        private struct SettingsBtnBind {
            public GameSettings settings;
            public Button btn;
        }
        
        [SerializeField] private SettingsBtnBind[] _settingsBtnBinds;

        private void Awake() {
            GameSettings.Current = _settingsBtnBinds.First().settings;
            
            foreach (var bind in _settingsBtnBinds) {
                bind.btn.onClick.AddListener(() => OnBtnSetDifficult(bind.settings));
            }
        }

        private void OnBtnSetDifficult(GameSettings settings) {
            GameSettings.Current = settings;
            GameController.SetGameReady();
        }
    }
}