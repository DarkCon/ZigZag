using UnityEngine;

namespace Game {
    public class GameController : MonoBehaviour {
        
        private void Awake() {
            //GameEvent.OnChangeStage += OnChangeStage;
        }

        private void OnDestroy() {
            //GameEvent.OnChangeStage -= OnChangeStage;
        }

        private void Start() {
            GameEvent.Current = GameStage.PREPARE;
        }

        private void OnChangeStage() {
            /*switch (GameEvent.Current) {
                case GameStage.PREPARE:
                    break;
                case GameStage.READY:
                    break;
                case GameStage.START:
                    break;
                case GameStage.STOP:
                    break;
            }*/
        }

        public static void SetGameReady() {
            GameEvent.Current = GameStage.READY;
        }

        public static void StartGame() {
            GameEvent.Current = GameStage.START;
        }

        public static void StopGame() {
            GameEvent.Current = GameStage.STOP;
        }

        public static void RestartGame() {
            GameEvent.Current = GameStage.READY;
        }
    }
}