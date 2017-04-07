using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    // ダイクストラ法 http://www.deqnotes.net/acmicpc/dijkstra/
    // 確定かどうか
    private bool _isDone;
    public bool IsDone { get { return _isDone; } set { _isDone = value; } }

    // このノードへの現時点での最小コスト
    private int _cost;
    public int Cost { get { return _cost; } set { _cost = value; } }

    // このノードにつながっているノード
    private List<Node> _linkNodes = new List<Node>();
    public List<Node> LinkNodes { get { return _linkNodes; } set { _linkNodes = value; } }

    // ノードに伸びる線
    [SerializeField]
    private GameObject _linkLine;
    private List<NodeLinkLine> _linkLines = new List<NodeLinkLine>();
    public List<NodeLinkLine> LinkLines { get { return _linkLines; } set { _linkLines = value; } }


    void Start()
    {
        ConnectLines();
    }

    public void Link(Node node)
    {
        foreach (var link_nodes in _linkNodes)
            if (link_nodes == node)
                return;
        _linkNodes.Add(node);
        node.Link(this);
    }

    public void ConnectLines()
    {
        for (int i = 0; i < _linkNodes.Count; i++)
        {
            var prefab = Instantiate(_linkLine, transform);

            var line = prefab.GetComponent<NodeLinkLine>();
            line.SetLinkNode(_linkNodes[i]);
            _linkLines.Add(line);
        }
    }

    void Update()
    {

    }
}
