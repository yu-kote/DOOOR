using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Wall : AttributeBase
{
    private NodeManager _nodeManager;
    public NodeManager NodeManager
    {
        get { return _nodeManager; }
        set { _nodeManager = value; }
    }

    private Node _myNode;
    public Node MyNode
    {
        get { return _myNode; }
        set { _myNode = value; }
    }

    private string _directionTag = "";

    void Start()
    {
        var x = _myNode.CellX;
        var y = _myNode.CellY;
        if (x - 2 >= 0)
            if (_nodeManager.Nodes[y][x - 2].GetComponent<Door>())
                _directionTag = "Right";
        if (x + 2 < _nodeManager.Nodes[y].Count())
            if (_nodeManager.Nodes[y][x + 2].GetComponent<Door>())
                _directionTag = "Left";

        if (_directionTag != "")
            _directionTag += "Room";
        CreateAttribute("Wall" + _directionTag);
    }
}
