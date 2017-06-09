using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGuide : MonoBehaviour
{

    // 次のノード
    private Dictionary<MyNumber, Node> _nextPath = new Dictionary<MyNumber, Node>();
    public Dictionary<MyNumber, Node> NextPath { get { return _nextPath; } set { _nextPath = value; } }

    // 前のノード
    private Dictionary<MyNumber, Node> _prevPath = new Dictionary<MyNumber, Node>();
    public Dictionary<MyNumber, Node> PrevPath { get { return _prevPath; } set { _prevPath = value; } }

    // 調べ済みかどうか
    private List<MyNumber> _isSearch = new List<MyNumber>();
    public List<MyNumber> IsSearch { get { return _isSearch; } set { _isSearch = value; } }

    public void AddSearch(GameObject human)
    {
        var mynumber = human.GetComponent<MyNumber>();
        if (_isSearch.Contains(mynumber) == false)
            _isSearch.Add(mynumber);
    }

    public bool SearchCheck(GameObject human)
    {
        var mynumber = human.GetComponent<MyNumber>();
        return _isSearch.Contains(mynumber);
    }

    public void SearchRemove(GameObject human)
    {
        _isSearch.Remove(human.GetComponent<MyNumber>());
    }

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

    private void OnDestroy()
    {
        _nextPath.Clear();
        _prevPath.Clear();
        _isSearch.Clear();
    }
}
