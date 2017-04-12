using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITargetMove : MonoBehaviour
{
    private RoadPathManager _roadPathManager;
    private NodeManager _nodeManager;
    private MyNumber _myNumber;

    private Node _targetNode;
    public Node TargetNode { get { return _targetNode; } set { _targetNode = value; } }

    private Node _currentNode;

    private int searchLimit = 1000;
    private int searchCount = 0;

    private GameObject _testSymbol;

    void Start()
    {
        var field = GameObject.Find("Field");
        _roadPathManager = field.GetComponent<RoadPathManager>();
        _nodeManager = field.GetComponent<NodeManager>();
        _myNumber = GetComponent<MyNumber>();
        _currentNode = _nodeManager.SearchOnNodeHuman(gameObject);
        _targetNode = _nodeManager.Nodes[2][30].GetComponent<Node>();

        _roadPathManager.RoadPathReset();

        if (WriteRoadPath(_currentNode) == false)
        {
            gameObject.AddComponent<AIMovement>();
            Destroy(GetComponent<AITargetMove>());
        }

        _currentNode = _nodeManager.SearchOnNodeHuman(gameObject);
        _testSymbol = Resources.Load<GameObject>("Prefabs/Map/Node/Symbol");

        SearchRoad(_currentNode);
    }

    void Update()
    {

    }

    void SearchRoad(Node current_node)
    {
        if (current_node == _targetNode)
            return;
        _testSymbol = Instantiate(_testSymbol);

        _testSymbol.transform.position = current_node.gameObject.transform.position;
        var nextnode = current_node.GetComponent<RoadPath>().Direction(gameObject);
        SearchRoad(nextnode);
    }

    bool WriteRoadPath(Node current_node)
    {
        searchCount++;
        if (searchCount > searchLimit)
        {
            Debug.Log("search limit");
            return false;
        }

        if (current_node == _targetNode)
        {
            Debug.Log("goal");
            return true;
        }

        var loadpath = current_node.gameObject.GetComponent<RoadPath>();
        for (int i = 0; i < current_node.LinkNodes.Count; i++)
        //foreach (var node in current_node.LinkNodes)
        {
            var node = current_node.LinkNodes[i];
            if (node.gameObject.GetComponent<RoadPath>()._isDone == true)
                continue;
            if (node.gameObject.GetComponent<Wall>() != null)
                continue;

            Debug.Log(current_node.gameObject.transform.position);
            loadpath.Add(gameObject, node);
            loadpath._isDone = true;
            if (WriteRoadPath(node))
                return true;
        }

        Debug.Log("search missing");
        return false;
    }
}
