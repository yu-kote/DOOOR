using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;

public class AITargetMove : AIRouteSearch
{
    private bool _targetMoveEnd = false;

    List<GameObject> _targetSymbols = new List<GameObject>();

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
        if (GetComponent<AIChace>())
        {
            Destroy(this);
            return;
        }
        // 目標地点までたどり着けなかった場合このスクリプトを消して普通の移動を開始させる
        if (Search() == false)
        {
            SearchMoveStart();
            return;
        }
        TargetNodeMarkPop();
    }

    void Update()
    {
        if (_targetMoveEnd) return;

        if (MoveComplete() &&
            _currentNode == _targetNode)
        {
            _targetMoveEnd = true;
            SearchMoveStart();
        }
    }

    protected override void NextNodeSearch()
    {
        _nextNode = _currentNode.GetComponent<NodeGuide>().NextNode(gameObject);
    }

    // 音を聞いて向かっている時にどこに向かっているかの印をつける
    private void TargetNodeMarkPop()
    {
        if (GetComponent<MyNumber>().Name != "Killer")
            return;

        if (_targetSymbols.Count > 0)
            return;

        if (GetComponent<AIController>().MoveMode != AIController.MoveEmotion.REACT_SOUND)
            return;

        var target_symbol = Resources.Load<GameObject>("Prefabs/UI/TargetNode");

        target_symbol = Instantiate(target_symbol);
        target_symbol.transform.localPosition = _targetNode.transform.position;
        target_symbol.transform.localEulerAngles = Vector3.zero;
        target_symbol.transform.eulerAngles = Vector3.zero;

        target_symbol.transform.localPosition += new Vector3(0, 2, 0);

        var angle = GameObject.Find("Player").transform.eulerAngles;
        target_symbol.transform.localEulerAngles = new Vector3(90, 180, 0) + angle;

        _targetSymbols.Add(target_symbol);
    }

    private void OnDestroy()
    {
        foreach (var item in _targetSymbols)
            Destroy(item);
        _targetSymbols.Clear();
        SymbolDestroy();
    }
}
