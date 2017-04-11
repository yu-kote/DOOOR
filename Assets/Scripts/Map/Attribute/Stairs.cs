using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : AttributeBase
{
    void Start()
    {
        var node = gameObject.GetComponent<Node>();
        var node_attribute = node.LinkNodeComponentCheck<Stairs>();
        if (node_attribute.IsInstanceAttribute() == false)
            CreateAttribute("Stairs");
    }
}
