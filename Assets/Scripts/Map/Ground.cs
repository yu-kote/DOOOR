using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    private NodeManager _nodeManager;


    void Start()
    {
        var field = GameObject.Find("Field");
        _nodeManager = field.GetComponent<NodeManager>();
    }

    void Update()
    {
        var pos = _nodeManager.GetNodesCenterPoint();
        transform.position = new Vector3(pos.x, 0, pos.z);
    }
}
