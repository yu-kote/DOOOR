using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIRunAway : AIBasicsMovement
{
    [SerializeField]
    private float _endDistance = 20;
    public float EndDistance { get { return _endDistance; } set { _endDistance = value; } }

    private Node _targetNode;
    public Node TargetNode { get { return _targetNode; } set { _targetNode = value; } }
    public void SetTargetNode(Node target_node) { _targetNode = target_node; }

    private bool _isEscape = false;

    void Start()
    {
        MoveSetup();
    }

    public override void MoveSetup()
    {
        var field = GameObject.Find("Field");
        _nodeController = field.GetComponent<NodeController>();
        _currentNode = GetComponent<AIController>().CurrentNode;
        _isEscape = false;

        _nodeController.ReFootPrint(gameObject, _currentNode);
    }

    bool RunAway()
    {
        var vec = _targetNode.transform.position - _currentNode.transform.position;
        var distance = vec.magnitude;

        // 逃げ切ったら終わり
        if (distance > _endDistance)
            return true;

        // 離れる方のノードに逃げるため、短い順にソートしてLastを選ぶ
        AITargetMove.SortByNodeLength(_targetNode, _currentNode.LinkNodes);
        if (_currentNode.LinkNodes.Last().GetComponent<Wall>() != null)
            return false;
        _nextNode = _currentNode.LinkNodes.Last();
        return false;
    }

    protected override void NextNodeSearch()
    {
        _isEscape = RunAway();
    }

    void Update()
    {
        if (_isEscape)
        {
            gameObject.AddComponent<AISearchMove>();
            Destroy(this);
        }
    }
}
