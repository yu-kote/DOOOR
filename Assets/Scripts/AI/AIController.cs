using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private Node _currentNode;
    public Node CurrentNode { get { return _currentNode; } set { _currentNode = value; } }

    private NodeManager _nodeManager;

    void Start()
    {
        var field = GameObject.Find("Field");
        _nodeManager = field.GetComponent<NodeManager>();
        _currentNode = _nodeManager.SearchOnNodeHuman(gameObject);
    }

    void Update()
    {

    }
}
