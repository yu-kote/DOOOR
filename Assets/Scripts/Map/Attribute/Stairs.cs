using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : AttributeBase
{
    void Start()
    {
        if (_isInstanceAttribute)
            CreateAttribute("Stairs");
    }
}
