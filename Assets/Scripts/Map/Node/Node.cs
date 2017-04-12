using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private int cell_x;
    public int CellX { get { return cell_x; } set { cell_x = value; } }
    private int cell_y;
    public int CellY { get { return cell_y; } set { cell_y = value; } }

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

    /// <summary>
    /// つながっているノードに引数のコンポーネントがあるかどうかを返す関数
    /// </summary>
    public T LinkNodeComponentCheck<T>()
    {
        foreach (var nodes in _linkNodes)
        {
            var component = nodes.gameObject.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
        }
        return default(T);
    }
}
