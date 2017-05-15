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
        if (next_node.GetComponent<Wall>() != null ||
            next_node.GetComponent<Door>() != null)
            return false;

        // ドアの鍵が閉まっているかどうか
        if (IsDoorLock(next_node))
            return false;

        _nextNode = next_node;

        return false;
    }

    protected override void NextNodeSearch()
    {
        _isChaceEnd = Chace();
    }

    void Update()
    {
        if (_targetMoveEnd) return;

        if (//MoveComplete() &&
            _isChaceEnd)
        {
            _targetMoveEnd = true;

            var ai_controller = GetComponent<AIController>();
            if (ai_controller.MoveMode == AIController.MoveEmotion.HURRY_UP)
                ai_controller.MoveMode = AIController.MoveEmotion.DEFAULT;

            if (IsDoorAround())
            {
                Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ =>
                {
                    if (_isChaceEnd)
                        SearchMoveStart();
                }).AddTo(this);
                return;
            }
            SearchMoveStart();
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
