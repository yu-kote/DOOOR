using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using System;

public class AISearchMove : AIBasicsMovement
{
    private RoadPathManager _roadPathManager;
    Node _newNode;

    private AIBeware _aiBeware;
    private int _wallSearchCount;

    public void Start()
    {
        Speed = GetComponent<AIController>().DefaultSpeed;
        GetComponent<AIController>().MoveMode = AIController.MoveEmotion.DEFAULT;
        _aiBeware = GetComponent<AIBeware>();

        MoveSetup();
    }

    public override void MoveSetup()
    {
        var field = GameObject.Find("Field");
        _nodeController = field.GetComponent<NodeController>();

        var node_manager = field.GetComponent<NodeManager>();
        _currentNode = node_manager.SearchOnNodeHuman(gameObject);

        _roadPathManager = field.GetComponent<RoadPathManager>();
        _roadPathManager.RoadGuideReset(gameObject);

        _wallSearchCount = 0;
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
        if (candidate == null)
            return;
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
            // 階段、ドア、部屋以外の普通の道は常に進む先に壁があるかどうかを調べる
            var avoid_stairs_node =
                candidate.Where(node =>
                    {
                        if (node.GetComponent<Wall>() ||
                            node.GetComponent<Stairs>() ||
                            node.GetComponent<Door>() ||
                            node.GetComponent<Kyukeispace>())
                            return false;
                        return true;
                    }).ToList();

            if (avoid_stairs_node.Count() <= 1 && avoid_stairs_node.FirstOrDefault())
            {
                _wallSearchCount = 0;
                if (SearchWall(avoid_stairs_node.First()))
                {
                    // 壁を探索できるに見つけたら足跡をつける
                    UntilTheWallAddTrace(avoid_stairs_node.First());
                    NextNodeSearch();
                }
                RemoveNextPath(avoid_stairs_node.First());
            }
            // つながっているノードの候補を決める
            var next_node_num = UnityEngine.Random.Range(0, candidate.Count);
            next_node = candidate[next_node_num];
        }
        _nextNode = next_node;
    }

    List<Node> CanMoveNode()
    {
        if (_currentNode == null) return null;
        return _currentNode.LinkNodes
            .Where(node => node.GetComponent<FootPrint>().TraceCheck(gameObject) == false)
            .Where(node => node.GetComponent<Wall>() == null)
            .Where(node =>
            {
                // ドアがロックされていたら通れない
                var door = node.GetComponent<Door>();
                if (door == null)
                    return true;

                if (IsDoorLock(node))
                {
                    if (tag == "Victim" && MoveComplete())
                    {
                        // ドアが開かないモーションをさせる
                        GetComponent<VictimAnimation>()
                        .ChangeAnimation(VictimAnimationStatus.OPEN_DOOR, 1.5f);
                        GetComponent<HumanAnimController>().Rotation(node.gameObject);
                    }
                    return false;
                }

                // 殺人鬼
                if (tag != "Killer")
                    return true;

                return false;
            }).Where(node =>
            {
                if (tag == "Killer")
                    return true;
                // 階段がロックされていたら入れない
                if (IsStairsLock(node))
                    return false;
                return true;
            }).ToList();
    }

    // 未踏の地を探す
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

    // 探索できる範囲まで壁があるかどうか調べる
    bool SearchWall(Node next_node)
    {
        _wallSearchCount++;
        if (_wallSearchCount >= _aiBeware.SearchLimit)
            return false;

        foreach (var node in next_node.LinkNodes)
        {
            if (_currentNode == node)
                continue;
            // 調べていたらtrue
            var road_path = node.GetComponent<NodeGuide>();
            if (road_path.NextPathCheck(gameObject))
                continue;
            // 自分が階段にいて、調べた先が階段だった場合は探索しない
            if (_currentNode.GetComponent<Stairs>())
                if (node.GetComponent<Stairs>())
                    continue;
            if (node.GetComponent<Stairs>())
                return false;
            // ドア
            if (node.GetComponent<Door>())
                return false;
            // 壁
            if (node.GetComponent<Wall>())
                return true;

            road_path.AddNextPath(gameObject, node);

            var is_wall_hit = SearchWall(node);
            if (is_wall_hit)
                return true;
        }
        return false;
    }

    void UntilTheWallAddTrace(Node node)
    {
        if (node == null) return;
        // 足跡をつける
        node.GetComponent<FootPrint>().AddTrace(gameObject);

        // 次のノードをもらう
        var guide = node.GetComponent<NodeGuide>();
        var next_node = guide.NextNode(gameObject);
        // 案内を消す
        guide.NextPathRemove(gameObject);
        if (next_node == null)
            return;
        UntilTheWallAddTrace(next_node);
    }

    void RemoveNextPath(Node node)
    {
        if (node == null) return;
        // 次のノードをもらう
        var guide = node.GetComponent<NodeGuide>();
        var next_node = guide.NextNode(gameObject);
        // 案内を消す
        guide.NextPathRemove(gameObject);
        if (next_node == null)
            return;
        RemoveNextPath(next_node);
    }

    void Update()
    {
        if (MoveComplete() && _newNode)
        {
            TargetMoveStart(_newNode);
        }
    }

    public void TargetMoveStart(Node target_node)
    {
        GetComponent<AIController>().MoveMode = AIController.MoveEmotion.DEFAULT;
        _roadPathManager.RoadGuideReset(gameObject);
        CanMove = false;

        CallBack(0.3f, () =>
        {
            if (GetComponent<AITargetMove>())
                return;
            var mover = gameObject.AddComponent<AITargetMove>();
            // どこを目指すかを教える
            mover.SetTargetNode(target_node);
            mover.Speed = GetComponent<AIController>().DefaultSpeed;
            Destroy(this);
        });
    }
}
