using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private Node _currentNode;
    public Node CurrentNode { get { return _currentNode; } set { _currentNode = value; } }
    private Node _nextNode;
    public Node NextNode { get { return _nextNode; } set { _nextNode = value; } }
    private Node _prevNode;
    public Node PrevNode { get { return _prevNode; } set { _prevNode = value; } }

    private NodeManager _nodeManager;
    private NodeController _nodeController;
    private RoadPathManager _roadPathManager;

    public enum MoveEmotion
    {
        DEFAULT,
        HURRY_UP,
    }

    [SerializeField]
    private MoveEmotion _moveMode = MoveEmotion.DEFAULT;
    public MoveEmotion MoveMode { get { return _moveMode; } set { _moveMode = value; } }
    private MoveEmotion _currentMoveMode = MoveEmotion.DEFAULT;


    [SerializeField]
    private float _defaultSpeed;
    public float DefaultSpeed { get { return _defaultSpeed; } set { _defaultSpeed = value; } }
    [SerializeField]
    private float _hurryUpSpeed;
    public float HurryUpSpeed { get { return _hurryUpSpeed; } set { _hurryUpSpeed = value; } }

    [SerializeField]
    private float _viewSpeed;

    void Start()
    {
        var field = GameObject.Find("Field");
        _nodeManager = field.GetComponent<NodeManager>();
        _nodeController = field.GetComponent<NodeController>();
        _roadPathManager = field.GetComponent<RoadPathManager>();

        _currentNode = _nodeManager.SearchOnNodeHuman(gameObject);
        GetMovement().Speed = _defaultSpeed;
    }

    void Update()
    {
        MoveSpeedChange();
        NodeUpdate();

        _viewSpeed = GetMovement().Speed;

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

            Destroy(human);
            break;
        }
    }

    private void MoveSpeedChange()
    {
        //if (_currentMoveMode == _moveMode)
        //    return;
        //_currentMoveMode = _moveMode;

        if (_moveMode == MoveEmotion.DEFAULT)
            GetMovement().Speed = _defaultSpeed;
        if (_moveMode == MoveEmotion.HURRY_UP)
            GetMovement().Speed = _hurryUpSpeed;
    }

    public AIBasicsMovement GetMovement()
    {
        AIBasicsMovement movement = null;
        if (GetComponent<AISearchMove>())
            movement = GetComponent<AISearchMove>();
        if (GetComponent<AITargetMove>())
            movement = GetComponent<AITargetMove>();
        if (GetComponent<AIRunAway>())
            movement = GetComponent<AIRunAway>();
        if (GetComponent<AIChace>())
            movement = GetComponent<AIChace>();
        return movement;
    }

    private bool IsHurryUp()
    {
        if (GetComponent<AISearchMove>())
            return false;
        if (GetComponent<AITargetMove>())
            return false;
        if (GetComponent<AIRunAway>())
            return true;
        if (GetComponent<AIChace>())
            return true;
        return false;
    }

    public void NodeUpdate()
    {
        var movement = GetMovement();
        if (movement == null) return;
        if (movement.CurrentNode &&
            _currentNode != movement.CurrentNode)
            _currentNode = movement.CurrentNode;

        if (movement.NextNode &&
            _nextNode != movement.NextNode)
            _nextNode = movement.NextNode;

        if (movement.PrevNode &&
            _prevNode != movement.PrevNode)
            _prevNode = movement.PrevNode;
    }

    private void OnDisable()
    {
        // この世界に残した跡をすべて消し去る
        _nodeController.EraseTraces(GetComponent<MyNumber>());
        _roadPathManager.RoadGuideReset(gameObject);
        _roadPathManager.SearchReset(gameObject);

        if (_currentNode != null)
            _currentNode.GetComponent<FootPrint>().EraseHumanOnNode(gameObject);
    }
}
