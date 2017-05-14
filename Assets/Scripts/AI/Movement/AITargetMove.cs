using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;

public class AITargetMove : AIRouteSearch
{
    protected bool _targetMoveEnd = false;

    void Start()
    {
        RouteSearchSetup();
        MoveSetup();
    }

    public override void MoveSetup()
    {
        _currentNode = GetComponent<AIController>().CurrentNode;

        _targetMoveEnd = false;
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
        if (Search() == false)
        {
            Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ =>
            {
                SearchMoveStart();
            }).AddTo(this);
            return;
        }
    }

    void Update()
    {
        if (_targetMoveEnd) return;

        if (MoveComplete() &&
            _currentNode == _targetNode)
        {
            _targetMoveEnd = true;

            var ai_controller = GetComponent<AIController>();
            if (ai_controller.MoveMode == AIController.MoveEmotion.HURRY_UP)
                ai_controller.MoveMode = AIController.MoveEmotion.DEFAULT;

            SearchMoveStart();
        }
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
