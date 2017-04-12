using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;

public class AISearchMove : AIBasicsMovement
{
    private MyNumber _myNumber;

    public void Start()
    {
        var field = GameObject.Find("Field");
        _nodeController = field.GetComponent<NodeController>();
        _myNumber = GetComponent<MyNumber>();
        var node_manager = field.GetComponent<NodeManager>();
        _currentNode = node_manager.SearchOnNodeHuman(gameObject);
    }

    protected override void NextNodeSearch()
    {
        // まだ足跡がついてないノードを探す
        var candidate = CanMoveNode();

        // 周りのノードが全部足跡ついていたら自分の足跡をすべて消して探しなおす
        if (candidate.Count == 0)
        {
            _nodeController.ReFootPrint(_myNumber, _currentNode);
            candidate = CanMoveNode();
        }

        var next_node_num = Random.Range(0, candidate.Count);
        _nextNode = candidate[next_node_num];
    }

    List<Node> CanMoveNode()
    {
        return _currentNode.LinkNodes
            .Where(node => node.GetComponent<FootPrint>().Traces.Contains(_myNumber) == false)
            .Where(node => node.GetComponent<Wall>() == null)
            .ToList();
    }
}
