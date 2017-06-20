using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGuide : MonoBehaviour
{

    // 次のノード
    private Dictionary<GameObject, Node> _nextPath = new Dictionary<GameObject, Node>();
    public Dictionary<GameObject, Node> NextPath { get { return _nextPath; } set { _nextPath = value; } }

    // 前のノード
    private Dictionary<GameObject, Node> _prevPath = new Dictionary<GameObject, Node>();
    public Dictionary<GameObject, Node> PrevPath { get { return _prevPath; } set { _prevPath = value; } }

    // 調べ済みかどうか
    private List<GameObject> _isSearch = new List<GameObject>();
    public List<GameObject> IsSearch { get { return _isSearch; } set { _isSearch = value; } }

    public void AddSearch(GameObject human)
    {
        if (_isSearch.Contains(human) == false)
            _isSearch.Add(human);
    }

    public bool SearchCheck(GameObject human)
    {
        return _isSearch.Contains(human);
    }

    public void SearchRemove(GameObject human)
    {
        _isSearch.Remove(human);
    }

    public void AddNextPath(GameObject human, Node node)
    {
        if (_nextPath.ContainsKey(human) == false)
            _nextPath.Add(human, node);
        else
            _nextPath[human] = node;
    }

    public bool NextPathCheck(GameObject human)
    {
        return _nextPath.ContainsKey(human);
    }

    public Node NextNode(GameObject human)
    {
        if (_nextPath.ContainsKey(human) == false)
            return null;
        return _nextPath[human];
    }

    public void NextPathRemove(GameObject human)
    {
        _nextPath.Remove(human);
    }

    public void AddPrevPath(GameObject human, Node node)
    {
        if (_prevPath.ContainsKey(human) == false)
            _prevPath.Add(human, node);
        else
            _prevPath[human] = node;
    }

    public bool PrevPathCheck(GameObject human)
    {
        return _prevPath.ContainsKey(human);
    }

    public Node PrevNode(GameObject human)
    {
        if (_prevPath.ContainsKey(human) == false)
            return null;
        return _prevPath[human];
    }

    public void PrevPathRemove(GameObject human)
    {
        _prevPath.Remove(human);
    }

    private void OnDestroy()
    {
        _nextPath.Clear();
        _prevPath.Clear();
        _isSearch.Clear();
    }
}
