using System.Collections.Generic;
using UnityEngine.Events;

namespace Game {
    public static class GameEvent {
        public static event UnityAction OnPrepare {
            add => AddListener(GameStage.PREPARE, value);
            remove => RemoveListener(GameStage.PREPARE, value);
        }
        public static event UnityAction OnReady {
            add => AddListener(GameStage.READY, value);
            remove => RemoveListener(GameStage.READY, value);
        }
        public static event UnityAction OnStart {
            add => AddListener(GameStage.START, value);
            remove => RemoveListener(GameStage.START, value);
        }
        public static event UnityAction OnStop {
            add => AddListener(GameStage.STOP, value);
            remove => RemoveListener(GameStage.STOP, value);
        }
        public static event UnityAction OnChangeStage {
            add => _onChangedStage.AddListener(value);
            remove => _onChangedStage.RemoveListener(value);
        }
        
        public static GameStage Current {
            get => _current;
            set {
                _current = value;
                Invoke(value);
            }
        }

        private static GameStage _current = GameStage.LOADED;
        private static readonly Dictionary<GameStage, UnityEvent> _actions = new Dictionary<GameStage, UnityEvent>();
        private static readonly UnityEvent _onChangedStage = new UnityEvent();
        
        private static void AddListener(GameStage ev, UnityAction act) {
            if (_actions.TryGetValue(ev, out var unityEvent)) {
                unityEvent.AddListener(act);
            } else {
                unityEvent = new UnityEvent();
                unityEvent.AddListener(act);
                _actions[ev] = unityEvent;
            }

            if (Current == ev) {
                act.Invoke();
            }
        }

        private static void RemoveListener(GameStage ev, UnityAction act) {
            if (_actions.TryGetValue(ev, out var unityEvent)) {
                unityEvent.RemoveListener(act);
            }
        }

        private static void Invoke(GameStage ev) {
            _onChangedStage.Invoke();
            if (_actions.TryGetValue(ev, out var unityEvent)) {
                unityEvent.Invoke();
            }
        }
    }
}