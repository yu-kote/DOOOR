using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using System;

public class AIChace : AITargetMove
{
    private int _endNodeDistance = 5;

    private GameObject _targetHuman;
    public GameObject TargetHuman { get { return _targetHuman; } set { _targetHuman = value; } }
    public void SetTargetHuman(GameObject target_node) { _targetHuman = target_node; }

    private bool _isChaceEnd;

    void Start()
    {
        // From RouteSearch
        RouteSearchSetup();
        // From TargetMove
        MoveSetup();

        // 道を可視化してみる
        //StartCoroutine(SearchRoadTestDraw(_currentNode));
    }

    public override void MoveSetup()
    {
        _currentNode = GetComponent<AIController>().CurrentNode;
        _isChaceEnd = false;
        _endNodeDistance = GetComponent<AIBeware>().SearchLimit;

        MoveReset();
    }

    Node ApproachNode()
    {
        if (Search())
            return _currentNode.GetComponent<NodeGuide>().NextNode(gameObject);
        return null;
    }

    bool Chace()
    {
        if (_isChaceEnd)
            return true;
        if (_targetHuman == null)
            return true;
        if (SearchCount > _endNodeDistance)
            return true;

        _targetNode = _targetHuman.GetComponent<AIController>().CurrentNode;

        Node next_node = null;
        next_node = ApproachNode();
        _roadPathManager.RoadGuideReset(gameObject);

        // 移動できるかどうか
        if (next_node == null)
            return false;

        if (next_node.GetComponent<Wall>() != null)
            return false;

        var door = next_node.GetComponent<Door>();
        if (door != null)
            //if (door._doorStatus == Door.DoorStatus.CLOSE)
            return true;

        // ドアの鍵が閉まっているかどうか
        if (IsDoorLock(next_node))
        {
            MoveReset();
            return false;
        }

        _nextNode = next_node;
        PrevNodeUpdate();

        return false;
    }

    protected override void NextNodeSearch()
    {
        _isChaceEnd = Chace();
    }

    void Update()
    {
        KillTarget();
        ChaceEnd();
    }

    void ChaceEnd()
    {
        if (_targetMoveEnd) return;

        if (//MoveComplete() &&
            _isChaceEnd)
        {
            _targetMoveEnd = true;

            if (IsDoorAround())
            {
                Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ =>
                {
                    if (_isChaceEnd)
                        SearchMoveStart();
                }).AddTo(gameObject);
                return;
            }
            SearchMoveStart();
        }
    }

    void KillTarget()
    {
        if (tag != "Killer")
            return;
        var humans = _currentNode.GetComponent<FootPrint>().HumansOnNode;
        if (humans.Count < 2) return;

        foreach (var human in humans)
        {
            if (human == null) continue;
            if (human.tag != "Victim") continue;
            if (_currentNode.GetComponent<Stairs>() &&
                human.GetComponent<AIController>().GetMovement().MoveComplete() == false)
                continue;
            // 追っている目標じゃなかったら殺さない
            if (_targetHuman != human)
                continue;
            _targetHuman = null;

            human.GetComponent<AIController>().BeKilled();
            break;
        }
    }

    // 周囲にドアがあるかどうか
    bool IsDoorAround()
    {
        foreach (var node in _currentNode.LinkNodes)
        {
            if (tag == "Killer" && node.GetComponent<Door>())
                return true;
        }
        return false;
    }
}
