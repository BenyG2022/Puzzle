using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveLoadUnity;
using UnityEngine.SceneManagement;
namespace PuzzleTestTask
{
    public class PlayZone : MonoBehaviour
    {
        [SerializeField] private Graph _graph = null;


        private void Awake()
        {
            if (_graph == null)
            {
                Debug.LogError("There is no Graph gameObject attached to SerializeField in PlayZone class. Exit play mode and setup the reference.");
                GameplayManager.instance.gameIsPaused = true;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController player) == true)
            {
                GameplayManager.instance.localPlayer.currentGraph = _graph;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out PlayerController player) == true)
            {
                GameplayManager.instance.localPlayer.currentGraph = null;
            }
        }
    }
}
