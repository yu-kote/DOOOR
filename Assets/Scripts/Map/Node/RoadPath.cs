using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPath : MonoBehaviour
{
    // ダイクストラ法 http://www.deqnotes.net/acmicpc/dijkstra/

    private Dictionary<MyNumber, Node> _path = new Dictionary<MyNumber, Node>();
    public Dictionary<MyNumber, Node> Path
    {
        get { return _path; }
        set { _path = value; }
    }

    // 調べ済みかどうか
    public bool _isDone = false;


    public void Add(GameObject human, Node node)
    {
        var mynumber = human.GetComponent<MyNumber>();
        if (_path.ContainsKey(mynumber) == false)
            _path.Add(mynumber, node);
        else
            _path[mynumber] = node;
    }

    public Node Direction(GameObject human)
    {
        var mynumber = human.GetComponent<MyNumber>();
        if (_path.ContainsKey(mynumber) == false)
            return null;
        return _path[mynumber];
    }

    public void Remove(GameObject human)
    {
        _path.Remove(human.GetComponent<MyNumber>());
    }
}
