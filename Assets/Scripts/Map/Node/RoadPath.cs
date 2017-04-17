using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPath : MonoBehaviour
{
    // ダイクストラ法 http://www.deqnotes.net/acmicpc/dijkstra/

    private Dictionary<MyNumber, Node> _path = new Dictionary<MyNumber, Node>();
    public Dictionary<MyNumber, Node> Path { get { return _path; } set { _path = value; } }

    // 調べ済みかどうか
    // private Dictionary<MyNumber, bool> _isDone = new Dictionary<MyNumber, bool>();
    //public Dictionary<MyNumber, bool> IsDone { get { return _isDone; } set { _isDone = value; } }

    public void Add(GameObject human, Node node)
    {
        var mynumber = human.GetComponent<MyNumber>();
        if (_path.ContainsKey(mynumber) == false)
            _path.Add(mynumber, node);
        else
            _path[mynumber] = node;
    }

    public bool PathCheck(GameObject human)
    {
        var mynumber = human.GetComponent<MyNumber>();
        return _path.ContainsKey(mynumber);
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
