using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;

public class AITargetMove : AIRouteSearch
{
    private bool _arriveAtTarget = false;


    void Start()
    {
        RouteSearchSetup();
        MoveSetup();
    }

    public override void MoveSetup()
    {
        _currentNode = GetComponent<AIController>().CurrentNode;

        _arriveAtTarget = false;
        _roadPathManager.RoadGuideReset(gameObject);
        TargetMoveStart(_targetNode);

        MoveReset();
    }

    // 引数のノードに移動を始める
    private void TargetMoveStart(Node target_node)
    {
        _targetNode = target_node;
        if (_targetNode == null)
        {
            _targetNode = _currentNode;
            return;
        }

        // 目標地点までたどり着けなかった場合このスクリプトを消して普通の移動を開始させる
        //if (WriteRoadPath(_currentNode) == false)
        if (Search() == false)
        {
            Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ =>
            {
                gameObject.AddComponent<AISearchMove>();
                Destroy(this);
            });
            return;
        }

        // 道を可視化してみる
        StartCoroutine(SearchRoadTestDraw(_currentNode));
    }



    void Update()
    {
        if (_arriveAtTarget) return;

        if (MoveComplete() &&
            _currentNode == _targetNode)
        {
            _arriveAtTarget = true;

            var ai_controller = GetComponent<AIController>();
            if (ai_controller.MoveMode == AIController.MoveEmotion.HURRY_UP)
                ai_controller.MoveMode = AIController.MoveEmotion.DEFAULT;

            if (IsDoorAround())
            {
                Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ =>
                {
                    gameObject.AddComponent<AISearchMove>();
                    Destroy(this);
                });
                return;
            }
            Destroy(this);
            gameObject.AddComponent<AISearchMove>();
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

    protected override void NextNodeSearch()
    {
        _nextNode = _currentNode.GetComponent<NodeGuide>().NextNode(gameObject);
    }

    private void OnDestroy()
    {
        SymbolDestroy();
    }
}
