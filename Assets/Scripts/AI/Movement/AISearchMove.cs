using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using System;

public class AISearchMove : AIBasicsMovement
{
    private MyNumber _myNumber;
    private RoadPathManager _roadPathManager;
    Node _newNode;

    public void Start()
    {
        Speed = GetComponent<AIController>().DefaultSpeed;
        GetComponent<AIController>().MoveMode = AIController.MoveEmotion.DEFAULT;

        MoveSetup();
    }

    public override void MoveSetup()
    {
        var field = GameObject.Find("Field");
        _nodeController = field.GetComponent<NodeController>();
        _myNumber = GetComponent<MyNumber>();

        var node_manager = field.GetComponent<NodeManager>();
        _currentNode = node_manager.SearchOnNodeHuman(gameObject);

        _roadPathManager = field.GetComponent<RoadPathManager>();
        _roadPathManager.RoadGuideReset(gameObject);

        _newNode = null;
        MoveReset();
    }

    protected override void StartNextNodeSearch()
    {
    }

    protected override void NextNodeSearch()
    {
        Node next_node = null;
        // まだ足跡がついてないノードをつながっているノードから探す
        var candidate = CanMoveNode();

        // 周りのノードが全部足跡ついていたら足跡を辿ってついていないところを探す
        if (candidate.Count == 0)
        {
            var target_node = SearchUnexploredNode(_currentNode);
            // 足跡がついていないところを見つける
            if (target_node)
                _newNode = target_node;
            else
            {
                // すべて足跡がついていたら足跡を消して今いる場所に足跡をつける
                _nodeController.ReFootPrint(gameObject, _currentNode);
                candidate = CanMoveNode();
            }
        }
        else
        {
            // つながっているノードの候補を決める
            var next_node_num = UnityEngine.Random.Range(0, candidate.Count);
            next_node = candidate[next_node_num];
        }
        _nextNode = next_node;
    }

    List<Node> CanMoveNode()
    {
        return _currentNode.LinkNodes
            .Where(node => node.GetComponent<FootPrint>().Traces.Contains(_myNumber) == false)
            .Where(node => node.GetComponent<Wall>() == null)
            .Where(node =>
            {
                // ドアがロックされていたら通れない
                var door = node.GetComponent<Door>();
                if (door == null) return true;
                if (door._doorStatus != Door.DoorStatus.CLOSE)
                    if (door.IsDoorLock())
                        return false;

                // 殺人鬼の時にドアが開いてなかったら通れなくする
                if (tag != "Killer") return true;
                if (door._doorStatus != Door.DoorStatus.CLOSE) return true;

                return false;
            })
            .ToList();
    }

    Node SearchUnexploredNode(Node current_node)
    {
        foreach (var node in current_node.LinkNodes)
        {
            if (_currentNode == node)
                continue;
            // 調べていたらtrue
            var road_path = node.GetComponent<NodeGuide>();
            if (road_path.NextPathCheck(gameObject))
                continue;
            // 壁
            if (node.GetComponent<Wall>())
                continue;
            // ドア
            var door = node.GetComponent<Door>();
            if (door)
                if (door._doorStatus == Door.DoorStatus.CLOSE)
                {
                    if (tag == "Killer")
                        continue;
                    if (door.IsDoorLock())
                        continue;
                }

            road_path.AddNextPath(gameObject, node);

            var foot_print = node.GetComponent<FootPrint>();
            if (foot_print.TraceCheck(gameObject) == false)
                return node;

            var new_node = SearchUnexploredNode(node);
            if (new_node)
                return new_node;
        }
        return null;
    }

    void Update()
    {
        if (MoveComplete() &&
            _newNode)
        {
            GetComponent<AIController>().MoveMode = AIController.MoveEmotion.DEFAULT;
            _roadPathManager.RoadGuideReset(gameObject);

            var mover = gameObject.AddComponent<AITargetMove>();
            // どこを目指すかを教える
            mover.SetTargetNode(_newNode);
            mover.Speed = GetComponent<AIController>().DefaultSpeed;
            Destroy(this);
        }
    }
}
