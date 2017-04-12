using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NodeController : MonoBehaviour
{
    private NodeManager _nodeManager;

    void Start()
    {
        _nodeManager = GetComponent<NodeManager>();
    }

    // 足跡を消す
    public void EraseTraces(MyNumber mynumber)
    {
        var nodes = _nodeManager.Nodes;

        foreach (var y in nodes)
        {
            foreach (var node in y)
            {
                var footprint = node.GetComponent<FootPrint>();
                if (footprint != null)
                    footprint.Traces.Remove(mynumber);
            }
        }
    }

    // 足跡を消して今いる位置に足跡をつける
    public void ReFootPrint(MyNumber mynumber, Node current_node)
    {
        EraseTraces(mynumber);
        // 今乗っているノードが再検索されないように足跡を付け直す
        current_node.GetComponent<FootPrint>().AddTrace(gameObject);
    }

}
