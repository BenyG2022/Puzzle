using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PuzzleTestTask
{

    public class GameplayManager
    {

        private GameObject _gameObject;

        private bool _gameIsPaused = false;
        public bool gameIsPaused
        {
            get
            {
                return _gameIsPaused;
            }
            set
            {
                _gameIsPaused = value;
                if (_gameIsPaused == true)
                {
                    Time.timeScale = 0.0f;
                }
                else
                {
                    Time.timeScale = 1.0f;
                }
            }
        }



        private static GameplayManager _instance;

        public static GameplayManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameplayManager();
                    _instance._gameObject = new GameObject("_gameManager");
                    _instance._gameObject.AddComponent<Timer>();
                }
                return _instance;
            }
        }

     


        private PlayerController _localPlayer;
        public PlayerController localPlayer
        {
            get
            {
                return _localPlayer;
            }
            set
            {
                _localPlayer = value;
            }
        }
        private Timer _timer;
        public Timer timer
        {
            get
            {
                if (_timer == null)
                {
                    _timer = _gameObject.GetComponent<Timer>();
                }
                return _timer;
            }
        }
     


      


    }
}