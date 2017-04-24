using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPath : MonoBehaviour
{
    
    // 次のノード
    private Dictionary<MyNumber, Node> _nextPath = new Dictionary<MyNumber, Node>();
    public Dictionary<MyNumber, Node> NextPath { get { return _nextPath; } set { _nextPath = value; } }

    public void AddNextPath(GameObject human, Node node)
    {
        var mynumber = human.GetComponent<MyNumber>();
        if (_nextPath.ContainsKey(mynumber) == false)
            _nextPath.Add(mynumber, node);
        else
            _nextPath[mynumber] = node;
    }

    public bool NextPathCheck(GameObject human)
    {
        var mynumber = human.GetComponent<MyNumber>();
        return _nextPath.ContainsKey(mynumber);
    }

    public Node NextNode(GameObject human)
    {
        var mynumber = human.GetComponent<MyNumber>();
        if (_nextPath.ContainsKey(mynumber) == false)
            return null;
        return _nextPath[mynumber];
    }

    public void NextPathRemove(GameObject human)
    {
        _nextPath.Remove(human.GetComponent<MyNumber>());
    }

    // 前のノード
    private Dictionary<MyNumber, Node> _prevPath = new Dictionary<MyNumber, Node>();
    public Dictionary<MyNumber, Node> PrevPath { get { return _prevPath; } set { _prevPath = value; } }

    public void AddPrevPath(GameObject human, Node node)
    {
        var mynumber = human.GetComponent<MyNumber>();
        if (_prevPath.ContainsKey(mynumber) == false)
            _prevPath.Add(mynumber, node);
        else
            _prevPath[mynumber] = node;
    }

    public bool PrevPathCheck(GameObject human)
    {
        var mynumber = human.GetComponent<MyNumber>();
        return _prevPath.ContainsKey(mynumber);
    }

    public Node PrevNode(GameObject human)
    {
        var mynumber = human.GetComponent<MyNumber>();
        if (_prevPath.ContainsKey(mynumber) == false)
            return null;
        return _prevPath[mynumber];
    }

    public void PrevPathRemove(GameObject human)
    {
        _prevPath.Remove(human.GetComponent<MyNumber>());
    }
}
