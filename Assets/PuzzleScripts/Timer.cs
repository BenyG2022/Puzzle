using System.Collections.Generic;
using UnityEngine;

namespace PuzzleTestTask
{
    public class Timer : MonoBehaviour
    {
        private class TimedEvent
        {
            public float timeToExecute;
            public Callback method;
        }

        public delegate void Callback();
        private List<TimedEvent> _events;


        public void add(Callback method, float inSeconds)
        {
            _events.Add(new TimedEvent { method = method, timeToExecute = Time.time + inSeconds });
        }



        private void Awake()
        {
            _events = new List<TimedEvent>();
        }
   


        // Update is called once per frame
        void Update()
        {
            if (_events.Count > 0)
            {
                for (int i = 0; i < _events.Count; i++)
                {
                    var timedEvent = _events[i];
                    if (timedEvent.timeToExecute <= Time.time)
                    {
                        timedEvent.method();
                        _events.Remove(timedEvent);
                    }
                }
            }
        }
    }
}
