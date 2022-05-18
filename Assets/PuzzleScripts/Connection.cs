using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveLoadUnity;
using PuzzleTestTask;
/// <summary>
/// Shows line renderer between scarabs. Proper color for settled and queried.
/// Can't query if settled.
/// Calculate if mouse position is hovering the connection line. Ignore if settled.
/// </summary>
//[System.Serializable]
public class Connection
{

    private LineRenderer _rend = null;
    private Scarab _lineStart = null;
    private Scarab _lineEnd = null;
    public Scarab start { get { return _lineStart; } }
    public Scarab end { get { return _lineEnd; } }
    /// <summary>
    /// -1 equals unactive
    /// </summary>
    public int queueOrder { get; private set; } = -1;
    public Connection(Scarab start, Scarab end)
    {
        _lineStart = start;
        _lineEnd = end;
        _rend = _lineStart.gameObject.GetComponent<LineRenderer>();
        if (_rend != null && _rend.positionCount > 0)
        {
            _rend = new GameObject("AddedByScriptLineRendererGO").AddComponent<LineRenderer>() as LineRenderer;
            _rend.gameObject.transform.SetParent(start.transform);
        }
        this.SerializeMember(nameof(queueOrder));
    }
    public void cancelConnection()
    {
        _rend.positionCount = 0;
        queueOrder = -1;
    }
    public void setQueueOrder(int order) => queueOrder = order;
    public void startConnection()
    {
        _rend.positionCount = 2;

        _rend.SetPositions(new Vector3[] { _lineStart.transform.position, _lineEnd.transform.position });
    }
}
