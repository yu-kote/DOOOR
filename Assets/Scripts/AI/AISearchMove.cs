using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using System;

public class AISearchMove : AIBasicsMovement
{
    private MyNumber _myNumber;

    public void Start()
    {
        Speed = GetComponent<AIController>().DefaultSpeed;
        MoveSetup();
    }

    public override void MoveSetup()
    {
        var field = GameObject.Find("Field");
        _nodeController = field.GetComponent<NodeController>();
        _myNumber = GetComponent<MyNumber>();
        var node_manager = field.GetComponent<NodeManager>();
        _currentNode = node_manager.SearchOnNodeHuman(gameObject);
        MoveReset();
    }

    protected override void NextNodeSearch()
    {
        // まだ足跡がついてないノードを探す
        var candidate = CanMoveNode();

        // 周りのノードが全部足跡ついていたら自分の足跡をすべて消して探しなおす
        if (candidate.Count == 0)
        {
            _nodeController.ReFootPrint(gameObject, _currentNode);
            candidate = CanMoveNode();
        }

        var next_node_num = UnityEngine.Random.Range(0, candidate.Count);
        _nextNode = candidate[next_node_num];
    }

    List<Node> CanMoveNode()
    {
        return _currentNode.LinkNodes
            .Where(node => node.GetComponent<FootPrint>().Traces.Contains(_myNumber) == false)
            .Where(node => node.GetComponent<Wall>() == null)
            .Where(node =>
            {
                // 殺人鬼の時にドアがあったら通れなくする
                if (tag != "Killer") return true;
                if (node.GetComponent<Door>() == null) return true;
                return false;
            })
            .ToList();
    }
}
