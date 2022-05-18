using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleTestTask
{
    public class Background : MonoBehaviour
    {
        [SerializeField] private GameObject _game;
        [SerializeField] private GameObject _win;
        [SerializeField] private GameObject _lose;

        public enum CurrentView { None = 0, Game, Win, Lose, Obfuscated };
        public CurrentView view = CurrentView.None;




        public void setBackgroundTo(CurrentView v)
        {
            _game.SetActive(false);
            _win.SetActive(false);
            _lose.SetActive(false);
            view = v;
            switch (view)
            {
                case CurrentView.None:
                    Debug.LogError("CurrentView setted to initial state None.");
                    break;
                case CurrentView.Game:
                    _game.SetActive(true);
                    break;
                case CurrentView.Win:
                    _win.SetActive(true);
                    break;
                case CurrentView.Lose:
                    _lose.SetActive(true);
                    break;
                case CurrentView.Obfuscated:
                    break;
                default:
                    break;
            }
        }
    }
}
