using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private Node _currentNode;
    public Node CurrentNode { get { return _currentNode; } set { _currentNode = value; } }

    private NodeManager _nodeManager;
    private NodeController _nodeController;
    private RoadPathManager _roadPathManager;

    public enum MoveEmotion
    {
        DEFAULT,
        HURRY_UP,
    }
    private MoveEmotion _moveMode = MoveEmotion.DEFAULT;
    public MoveEmotion MoveMode { get { return _moveMode; } set { _moveMode = value; } }

    [SerializeField]
    private float _defaultSpeed;
    public float DefaultSpeed { get { return _defaultSpeed; } set { _defaultSpeed = value; } }
    [SerializeField]
    private float _hurryUpSpeed;
    public float HurryUpSpeed { get { return _hurryUpSpeed; } set { _hurryUpSpeed = value; } }

    void Start()
    {
        var field = GameObject.Find("Field");
        _nodeManager = field.GetComponent<NodeManager>();
        _nodeController = field.GetComponent<NodeController>();
        _roadPathManager = field.GetComponent<RoadPathManager>();

        _currentNode = _nodeManager.SearchOnNodeHuman(gameObject);
    }

    void Update()
    {
        if (tag != "Killer") return;

        var humans = _currentNode.GetComponent<FootPrint>().HumansOnNode;
        if (humans.Count < 2) return;

        foreach (var human in humans)
        {
            if (human == null) continue;
            if (human.tag != "Victim") continue;
            Debug.Log(human.tag + "Destroy");

            // この世界に残した跡をすべて消し去る
            _nodeController.EraseTraces(human.GetComponent<MyNumber>());
            _roadPathManager.RoadGuideReset(human);
            _roadPathManager.SearchReset(human);

            _currentNode.GetComponent<FootPrint>().EraseHumanOnNode(human);

            Destroy(human);
            break;
        }
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
        return movement;
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (gameObject.tag != "Killer") return;
    //    if (other.gameObject.tag != "Victim") return;

    //    Debug.Log("Kill Enter" + other.gameObject.tag);
    //    //Destroy(other.gameObject);
    //}


    //private void OnCollisionStay(Collision collision)
    //{
    //    if (gameObject.tag != "Killer") return;
    //    if (collision.gameObject.tag != "Victim") return;

    //    Debug.Log("Kill Collder" + collision.gameObject.tag);
    //    //Destroy(collision.gameObject);
    //}

}
