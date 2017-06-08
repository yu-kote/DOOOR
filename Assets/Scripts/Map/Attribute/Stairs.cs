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

            var offset = 0.5f;
            var surface_num = _nodeManager.WhichSurfaceNum(_cell_x);
            if (surface_num == 0)
                _attribute.transform.localPosition += new Vector3(0, 0, -offset);
            else if (surface_num == 1)
                _attribute.transform.localPosition += new Vector3(offset, 0, 0);
            else if (surface_num == 2)
                _attribute.transform.localPosition += new Vector3(0, 0, offset);
            else if (surface_num == 3)
                _attribute.transform.localPosition += new Vector3(-offset, 0, 0);
        }




    }
}
