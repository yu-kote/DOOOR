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
        var nodes = _nodeManager.Nodes.Cast<GameObject>();

        foreach (var node in nodes)
        {
            var footprint = node.GetComponent<FootPrint>();
            footprint.Traces.Remove(mynumber);
        }
    }
}
