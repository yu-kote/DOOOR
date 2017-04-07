using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;

public class AIMovement : MonoBehaviour
{
    private GameObject _field;
    private NodeController _nodeController;

    private MyNumber _myNumber;

    private Node _currentNode;
    public Node CurrentNode { get { return _currentNode; } set { _currentNode = value; } }

    private Node _nextNode = null;

    private Vector3 _moveDirection;
    private Vector3 _moveDistance;

    private bool _canMove = false;

    void Start()
    {
        _field = GameObject.Find("Field");
        _nodeController = _field.GetComponent<NodeController>();
        _myNumber = GetComponent<MyNumber>();

        NextNodeSearch();

    }

    void AddFootPrint()
    {
        var foot_print = _currentNode.GetComponent<FootPrint>();
        foot_print.StepIn(gameObject);
    }

    void LeaveFootPrint()
    {
        var foot_print = _currentNode.GetComponent<FootPrint>();
        foot_print.StepOut(gameObject);
    }

    void Update()
    {
        //    this.ObserveEveryValueChanged(movement => movement._nextNode != movement._currentNode)
        //.Subscribe(_ =>
        //{
        //    Debug.Log("ノードを移動します");

        //    var distance = _currentNode.transform.position - _nextNode.transform.position;
        //    _moveDistance = distance;
        //    _moveDirection = distance * 0.01f;
        //    LeaveFootPrint();
        //    _currentNode = _nextNode;
        //    AddFootPrint();
        //    _canMove = true;
        //});

        if (_nextNode != _currentNode)
        {
            var distance = _nextNode.transform.position - _currentNode.transform.position;
            _moveDistance = distance;
            _moveDirection = distance * 0.01f;
            LeaveFootPrint();
            _currentNode = _nextNode;
            AddFootPrint();
            _canMove = true;
        }

        Move();
    }

    void NextNodeSearch()
    {
        _canMove = false;
        // まだ足跡がついてないノードを探す
        var candidate = NotFootPrintNode();

        if (candidate == null)
        {
            _nodeController.EraseTraces(_myNumber);
            candidate = NotFootPrintNode();
        }

        var next_node_num = Random.Range(0, candidate.Count);

        Debug.Log(next_node_num);
        _nextNode = candidate[next_node_num];
    }

    List<Node> NotFootPrintNode()
    {
        return _currentNode.LinkNodes
            .Where(node => node.GetComponent<FootPrint>().Traces.Contains(_myNumber) == false)
            .ToList();
    }

    void Move()
    {
        if (_canMove == false) return;
        transform.position += _moveDirection;
        _moveDistance -= _moveDirection;

        //if (_moveDistance.x < 0.1f && _moveDistance.x > -0.1f)
        //    if (_moveDistance.y < 0.1f && _moveDistance.y > -0.1f)
        //        if (_moveDistance.z < 0.1f && _moveDistance.z > -0.1f)
        if (_moveDistance == Vector3.zero)
        {
            NextNodeSearch();
        }
    }

}
