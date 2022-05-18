using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SaveLoadUnity;
namespace PuzzleTestTask
{




    /// <summary>
    /// for handling mouse hover, mouse left button click.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [System.Serializable]
        private class SceneWall
        {
            public Transform wall = null;
            public AsyncOperation operation = null;
            public string sceneName = "";
        }

        private Transform _playerTransform = null;
        private Camera _camera = null;

        [Header("Lights")]
        [SerializeField] private Transform _dirLight = null;
        [SerializeField] private float _flashLightRangeMultiplier = 1.5f;
        [SerializeField] private Transform _laserDotMarker = null;
        private Transform _laserDotMarkerLight = null;
        private Light _laserDotMarkerLightProperties = null;


        [Header("Vertexes Materials")]
        [SerializeField] private Material _unActive = null;
        [SerializeField] private Material _active = null;
        [SerializeField] private Material _queried = null;

        [Header("Scenes Handling")]
        [SerializeField] private float _distanceToGameOnWallThreeshold = 22.5f;
        [SerializeField] private SceneWall _north = default;
        [SerializeField] private SceneWall _south = default;
        [SerializeField] private SceneWall _west = default;
        [SerializeField] private SceneWall _east = default;



        public Material getProperScarabMaterial(Scarab.Type type)
        {
            switch (type)
            {
                case Scarab.Type.None:
                    return null;
                case Scarab.Type.UnActive:
                    return _unActive;
                case Scarab.Type.Active:
                    return _active;
                case Scarab.Type.Queried:
                    return _queried;
                default:
                    return null;
            }
        }


        private Graph _currentGraph = null;
        public Graph currentGraph
        {
            get
            {
                if (_currentGraph == null)
                {
                    Debug.LogError("Current Graph-class gameObject is null.You should not try to get it, before it is setted.");
                }
                return _currentGraph;
            }
            set
            {
                if (value == null)
                {
                    SaveLoad.SaveWithIndex(SceneManager.GetSceneByName(_currentGraph.gameObject.scene.name).buildIndex);
                    _currentGraph.changeBackground(Background.CurrentView.Obfuscated);
                    _laserDotMarker.position = Vector3.zero;
                    _currentGraph = null;

                }
                else
                {
                    _currentGraph = value;
                    if (SaveLoad.LoadWithIndex(SceneManager.GetSceneByName(_currentGraph.gameObject.scene.name).buildIndex) == true)
                    {
                        Debug.Log("Loading true.");
                        _currentGraph.updateLoaded();
                    }
                    else
                    {
                        Debug.LogWarning("Loading false");
                    }
                    _currentGraph.changeBackground(Background.CurrentView.Game);
                }
            }
        }




        private void Awake()
        {
            GameplayManager.instance.localPlayer = this;
            _camera = this.GetComponentInChildren<Camera>();
            _playerTransform = this.transform;
            _laserDotMarkerLight = _laserDotMarker.Find("Point Light");
            _laserDotMarkerLightProperties = _laserDotMarkerLight.GetComponent<Light>();
        }

        void Update()
        {
            if (GameplayManager.instance.gameIsPaused == false)
            {
                _dirLight.LookAt(_playerTransform);
                if (_currentGraph != null)
                {

                    Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.05f));
                    RaycastHit hit = default;
                    Vector3 targetPosition = ray.GetPoint(5000.0f);

                    if (Physics.Raycast(ray, out hit))
                    {
                        targetPosition = hit.point;
                        Scarab pointedVertex = hit.collider.GetComponent<Scarab>();
                        if (hit.collider != null)
                        {
                            if (pointedVertex != null)
                            {
                                currentGraph.queryConnection(pointedVertex);
                            }
                            else if (currentGraph.firstOne != null && pointedVertex == null)
                            {
                                pointedVertex = currentGraph.checkIfHoveringAimOnNextAvailableConnection(targetPosition);
                                if (pointedVertex != null)
                                {
                                    currentGraph.queryConnection(pointedVertex);
                                }
                            }

                            if (pointedVertex != null && Input.GetMouseButtonUp(0) == true)
                            {
                                if (pointedVertex == currentGraph.lastOne)
                                {
                                    currentGraph.undoConnection();
                                }
                                else
                                {
                                    currentGraph.createConnection(pointedVertex);
                                }
                            }
                            else if (pointedVertex == null)
                            {
                                currentGraph.cancelQueryConnection();
                            }
                            _laserDotMarker.position = targetPosition;
                            _laserDotMarker.LookAt(this._playerTransform);
                            _laserDotMarkerLight.position = _laserDotMarker.position + _laserDotMarker.forward * (_laserDotMarker.position - _playerTransform.position).magnitude * 0.5f;
                            _laserDotMarkerLight.LookAt(_laserDotMarker);
                            _laserDotMarkerLightProperties.range = (targetPosition - _playerTransform.position).magnitude * _flashLightRangeMultiplier;
                        }
                    }
                }
                loadUnloadScenes(new SceneWall[] { _north, _south, _west, _east });
            }
        }

        private void loadUnloadScenes(SceneWall[] scenes)
        {
            for (int i = 0; i < scenes.Length; i++)
            {
                checkDistanceAndLoadOrUnload(scenes[i]);
            }
        }
        private void checkDistanceAndLoadOrUnload(SceneWall sceneWall)
        {
            float distance = (_playerTransform.position - sceneWall.wall.position).magnitude;
            if (distance > _distanceToGameOnWallThreeshold)
            {
                if (SceneManager.GetSceneByName(sceneWall.sceneName).isLoaded == true)
                {
                    if (sceneWall.operation == null || sceneWall.operation.isDone == true)
                    {
                        sceneWall.operation = SceneManager.UnloadSceneAsync(sceneWall.sceneName);
                        SaveLoad.clear();
                    }
                }
            }
            else
            {
                if (SceneManager.GetSceneByName(sceneWall.sceneName).isLoaded == false)
                {
                    if (sceneWall.operation == null || sceneWall.operation.isDone == true)
                    {
                        sceneWall.operation = SceneManager.LoadSceneAsync(sceneWall.sceneName, LoadSceneMode.Additive);
                    }
                }
            }

        }

    }

}