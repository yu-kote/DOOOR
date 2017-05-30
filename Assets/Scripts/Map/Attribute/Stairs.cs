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
    
    void Start()
    {
        if (_isInstanceAttribute)
            CreateAttribute("Stairs" + _directionTag);
    }
}
