using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIRunAway : AIBasicsMovement
{
    private RoadPathManager _roadPathManager;

    private float _endDistance = 20;
    public float EndDistance { get { return _endDistance; } set { _endDistance = value; } }

    private GameObject _targetHuman;
    public GameObject TargetNode { get { return _targetHuman; } set { _targetHuman = value; } }
    public void SetTargetNode(GameObject target_node) { _targetHuman = target_node; }

    private bool _isEscape = false;

    void Start()
    {
        var field = GameObject.Find("Field");
        _roadPathManager = field.GetComponent<RoadPathManager>();
        _nodeController = field.GetComponent<NodeController>();

        MoveSetup();
    }

    public override void MoveSetup()
    {
        _currentNode = GetComponent<AIController>().CurrentNode;
        _isEscape = false;

        MoveReset();
    }

    bool RunAway()
    {
        if (_isEscape) return true;
        if (_targetHuman == null) return true;
        var vec = _targetHuman.transform.position - _currentNode.transform.position;
        var distance = vec.magnitude;

        // 逃げ切ったら終わり
        if (distance > _endDistance)
            return true;

        // つながっているノードを見る
        // ある方向に逃げた場合どのぐらいの距離逃げることが出来るのかを割り出す
        // それを比べて、逃げるノードを選択する

        for (int i = 0; i < _currentNode.LinkNodes.Count; i++)
        {

            _roadPathManager.RoadPathReset(gameObject);
        }



        // 離れる方のノードに逃げるため、短い順にソートしてLastを選ぶ
        //AITargetMove.SortByNodeLength(
        //    _targetHuman.GetComponent<AIController>().CurrentNode,
        //    _currentNode.LinkNodes);

        //if (_currentNode.LinkNodes.Last().GetComponent<Wall>() != null)
        //    return false;
        //_nextNode = _currentNode.LinkNodes.Last();
        return false;
    }

    protected override void NextNodeSearch()
    {
        _isEscape = RunAway();
    }

    void Update()
    {
        if (MoveComplete() && _isEscape)
        {
            gameObject.AddComponent<AISearchMove>();
            Destroy(this);
        }
    }
}
