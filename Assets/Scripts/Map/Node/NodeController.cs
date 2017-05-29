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
                if (node == null)
                    continue;
                var footprint = node.GetComponent<FootPrint>();
                if (footprint != null)
                    footprint.EraseTrace(mynumber);
            }
        }
    }

    // 足跡をリセットする（出口だけは記憶しておく）
    public void ReFootPrint(GameObject human, Node current_node)
    {
        // 出口を知っている場合は出口に足跡を残しておく
        var is_exit_footprint = false;
        var exit_node = human.GetComponent<AIController>().ExitNode();
        if (exit_node)
            if (exit_node.GetComponent<FootPrint>().TraceCheck(human))
                is_exit_footprint = true;

        EraseTraces(human.GetComponent<MyNumber>());
        // 今乗っているノードが再検索されないように足跡を付け直す
        current_node.GetComponent<FootPrint>().AddTrace(human);

        if (is_exit_footprint)
            if (exit_node)
                exit_node.GetComponent<FootPrint>().AddTrace(human);
    }

}
