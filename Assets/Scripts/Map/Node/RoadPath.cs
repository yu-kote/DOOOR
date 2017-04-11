using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPath : MonoBehaviour
{
    private Dictionary<MyNumber, Node> _roadPath = new Dictionary<MyNumber, Node>();

    public void Add(GameObject human, Node node)
    {
        var mynumber = human.GetComponent<MyNumber>();
        if (_roadPath.ContainsKey(mynumber) == false)
            _roadPath.Add(mynumber, node);
        else
            _roadPath[mynumber] = node;
    }

    public Node Direction(GameObject human)
    {
        return _roadPath[human.GetComponent<MyNumber>()];
    }

    public void Remove(GameObject human)
    {
        _roadPath.Remove(human.GetComponent<MyNumber>());
    }
}
