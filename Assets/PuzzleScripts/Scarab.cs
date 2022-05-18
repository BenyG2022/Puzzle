using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveLoadUnity;
using PuzzleTestTask;
public class Scarab : MonoBehaviour
{
    public List<Scarab> canConnectThose = new List<Scarab>();
    public List<Scarab> connectedWithThose = new List<Scarab>();
    public enum Type { None = 0, UnActive, Active, Queried };
    private Type _typeState = Type.UnActive;
    [SerializeField] private MeshRenderer _rend = null;


    private void Awake()
    {
        _rend = this.GetComponent<MeshRenderer>();
        this.SerializeMember(nameof(_typeState));
    }
    public void changeTypeState(Type type)
    {
        _rend.material = GameplayManager.instance.localPlayer.getProperScarabMaterial(type);
        _typeState = type;
    }
    public Type getScarabType() => _typeState;
    public void toggleVisibility(bool toggle)
    {
        this._rend.enabled = toggle;
        foreach (Transform item in this.gameObject.transform)
        {
            item.gameObject.GetComponent<LineRenderer>().enabled = toggle;
        }
        this.GetComponent<SphereCollider>().enabled = toggle;
        this.GetComponent<LineRenderer>().enabled = toggle;
    }

}
