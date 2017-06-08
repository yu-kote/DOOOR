using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : AttributeBase
{
    // Left or Right
    private string _directionTag;
    public string DirectionTag
    {
        get { return _directionTag; }
        set { _directionTag = value; }
    }

    private int _cell_x;
    public int CellX
    {
        get { return _cell_x; }
        set { _cell_x = value; }
    }

    private NodeManager _nodeManager;

    void Start()
    {
        _nodeManager = GameObject.Find("Field").GetComponent<NodeManager>();
        if (_isInstanceAttribute)
        {
            CreateAttribute("Stairs" + _directionTag);

            var offset = 1f;
            _attribute.transform.localPosition += new Vector3(0, 0, -offset);

        }
    }
}
