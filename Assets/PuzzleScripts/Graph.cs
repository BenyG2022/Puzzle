using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace PuzzleTestTask
{
    public class Graph : MonoBehaviour
    {
        private Background _backgroundManager = null;

        public Scarab firstOne = null;
        public Scarab lastOne = null;
        private Scarab _queriedOne = null;

        private List<Scarab> _availableVertexes = null;
        public List<Scarab> settledScarabs = null;

        private Connection _queriedConnection = null;
        public List<Connection> madeConnections = null;
        private List<Connection> _availableConnections = null;

        [SerializeField] private float _linesWidthHover = 0.64f;


        private void Awake()
        {
            Debug.Log("Graph Awake");
            _backgroundManager = this.GetComponentInParent<Background>();
            _availableVertexes = new List<Scarab>();
            foreach (Transform vert in this.transform)
            {
                _availableVertexes.Add(vert.GetComponent<Scarab>());
            }
            _availableConnections = new List<Connection>();
            foreach (Scarab scarab in _availableVertexes)
            {
                foreach (Scarab connectableScarab in scarab.canConnectThose)
                {
                    _availableConnections.Add(new Connection(scarab, connectableScarab));
                }
            }

            madeConnections = new List<Connection>();
            settledScarabs = new List<Scarab>();
            changeBackground(Background.CurrentView.Obfuscated);

        }

        public void updateLoaded()
        {
            List<Connection> activeOnes = new List<Connection>();
            foreach (Connection connection in _availableConnections)
            {
                if (connection.queueOrder > -1)
                {
                    activeOnes.Add(connection);
                }
            }
            activeOnes.Sort((x, y) => x.queueOrder.CompareTo(y.queueOrder));

            for (int i = 0; i < activeOnes.Count; i++)
            {
                if (firstOne == null)
                {
                    settledScarabs.Add(lastOne = firstOne = activeOnes[0].start);

                }
                activeOnes[i].startConnection();
                //_queriedOne = activeOnes[i].start;
                madeConnections.Add(activeOnes[i]);
                settledScarabs.Add(lastOne = activeOnes[i].end);
            }

            //foreach (Connection connection in activeOnes)
            //{
            //    if (firstOne == null)
            //    {
            //        lastOne = firstOne = connection.start;
            //        settledScarabs.Add(connection.start);
            //    }
            //    lastOne = connection.end;
            //    settledScarabs.Add(connection.end);
            //    connection.startConnection();
            //}
            settledScarabs.ForEach(x => x.changeTypeState(Scarab.Type.Active));
        }
        public void changeBackground(Background.CurrentView view)
        {

            if (view != _backgroundManager.view)
            {
                bool showVertexes = view != Background.CurrentView.Obfuscated;
                foreach (Scarab vertex in _availableVertexes)
                {
                    //vertex.gameObject.SetActive(showVertexes);
                    vertex.toggleVisibility(showVertexes);
                }
                _backgroundManager.setBackgroundTo(view);
            }
        }

        public void undoConnection()
        {
            lastOne.changeTypeState(Scarab.Type.UnActive);
            if (firstOne == lastOne)
            {
                restartPuzzle();
                settledScarabs.Clear();
                madeConnections.Clear();
                firstOne = lastOne = null;
            }
            else
            {
                madeConnections[madeConnections.Count - 1].cancelConnection();
                madeConnections.RemoveAt(madeConnections.Count - 1);

                if (settledScarabs.Count > 1)
                {
                    lastOne = settledScarabs[settledScarabs.Count - 2];
                }
                settledScarabs.RemoveAt(settledScarabs.Count - 1);
            }
            cancelQueryConnection();
        }
        public void createConnection(Scarab s)
        {
            if ((lastOne != null && lastOne.canConnectThose.Contains(s) == true) || firstOne == null)
            {
                Debug.Log("createConnection scarab.name = " + s.name);
                s.changeTypeState(Scarab.Type.Active);
                settledScarabs.Add(s);
                _queriedOne = null;
                if (firstOne == null)
                {
                    firstOne = s;
                }
                else
                {
                    _queriedConnection.setQueueOrder(madeConnections.Count);
                    madeConnections.Add(_queriedConnection);
                    _queriedConnection = null;
                }
                lastOne = s;

                bool cantSettleMore = true;
                foreach (Scarab vert in lastOne.canConnectThose)
                {
                    if (checkIfConnectionHasBeenMade(vert, lastOne) == false)
                    {
                        cantSettleMore = false;
                        break;
                    }
                }
                if (cantSettleMore == true)
                {
                    changeBackground(madeConnections.Count == _availableConnections.Count ? Background.CurrentView.Win : Background.CurrentView.Lose);
                    GameplayManager.instance.localPlayer.GetComponent<EasySurvivalScripts.PlayerMovement>().enabled = false;
                    GameplayManager.instance.timer.add(() =>
                    {
                        restartPuzzle();
                        GameplayManager.instance.localPlayer.transform.position = new Vector3(0.0f, 7.0f, 0.0f);
                        GameplayManager.instance.localPlayer.currentGraph = null;
                        GameplayManager.instance.localPlayer.GetComponent<EasySurvivalScripts.PlayerMovement>().enabled = true;
                    }, 2.0f);

                }
            }
        }

        public void queryConnection(Scarab s)
        {
            if (s != _queriedOne && _queriedOne == null && lastOne != null && lastOne.canConnectThose.Contains(s) == true && checkIfConnectionHasBeenMade(lastOne, s) == false)
            {
                if (s.connectedWithThose.Contains(lastOne) == true)
                {
                    Debug.Log("Already connected with " + s.name);
                    return;
                }
                Debug.Log(s.gameObject.name + " queried...");
                _queriedOne = s;
                _queriedOne.changeTypeState(Scarab.Type.Queried);
                _queriedConnection = _availableConnections.Find(x => x.start == lastOne && x.end == _queriedOne);
                _queriedConnection.startConnection();
            }
            else if (firstOne == null)
            {
                _queriedOne = s;
                _queriedOne.changeTypeState(Scarab.Type.Queried);
            }
        }

        public void cancelQueryConnection()
        {
            if (_queriedConnection != null)
            {
                _queriedConnection.cancelConnection();
            }
            if (_queriedOne != null)
            {
                _queriedOne.changeTypeState(Scarab.Type.UnActive);
            }
            _queriedOne = null;
            _queriedConnection = null;
        }


        public void obfuscateGraph()
        {
            _backgroundManager.setBackgroundTo(Background.CurrentView.Obfuscated);
        }

        public Scarab checkIfHoveringAimOnNextAvailableConnection(Vector3 point)
        {
            foreach (Scarab vert in lastOne.canConnectThose)
            {
                if (checkIfConnectionHasBeenMade(lastOne, vert) == true)
                {
                    continue;
                }
                if (_linesWidthHover > (findNeareastPointOnLine(lastOne.transform.position, vert.transform.position, point) - point).magnitude)
                {
                    return vert;
                }
            }
            return null;
        }
        private Vector3 findNeareastPointOnLine(Vector3 origin, Vector3 end, Vector3 point)
        {
            Vector3 heading = end - origin;
            float magnitudeMax = heading.magnitude;
            heading.Normalize();

            Vector3 lhs = point - origin;
            float dotP = Vector3.Dot(lhs, heading);
            dotP = Mathf.Clamp(dotP, 0.0f, magnitudeMax);
            return origin + heading * dotP;
        }

        private bool checkIfConnectionHasBeenMade(Scarab first, Scarab second)
        {
            foreach (Connection connection in madeConnections)
            {
                if ((connection.start == first && connection.end == second) || (connection.start == second && connection.end == first))
                {
                    return true;
                }
            }
            return false;
        }

        private void restartPuzzle()
        {

            _availableVertexes.ForEach(x => x.changeTypeState(Scarab.Type.UnActive));
            madeConnections.ForEach(x => x.cancelConnection());
            //madeConnections = new List<Connection>();
            //firstOne = lastOne = _queriedOne = null;
            //_queriedConnection = null;
        }


    }
}
