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

    void Start()
    {
        var field = GameObject.Find("Field");
        _roadPathManager = field.GetComponent<RoadPathManager>();
        _nodeManager = field.GetComponent<NodeManager>();
        _myNumber = GetComponent<MyNumber>();
        _currentNode = _nodeManager.SearchOnNodeHuman(gameObject);
        _targetNode = _nodeManager.Nodes[2][30].GetComponent<Node>();
        WriteRoadPath(_currentNode);
    }

    void Update()
    {

    }

    void SearchRoad()
    {

    }

    void WriteRoadPath(Node current_node)
    {
        Debug.Log(current_node.gameObject.transform.position);
        if (current_node == _targetNode)
            return;
        var loadpath = current_node.gameObject.GetComponent<RoadPath>();
        foreach (var node in current_node.LinkNodes)
        //for (int i = 0; i < current_node.LinkNodes.Count - 1; i++)
        {
            //var node = current_node.LinkNodes[i];
            loadpath.Add(gameObject, node);
            //WriteRoadPath(node);
        }
    }
}
