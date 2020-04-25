using System;

namespace Data {
    public static class Score {
        public static event Action OnChange;
        
        private static int _scoreValue;

        public static int Value {
            get => _scoreValue;
            set {
                if (_scoreValue != value) {
                    _scoreValue = value;
                    OnChange?.Invoke();
                }
            }
        }
    }
}